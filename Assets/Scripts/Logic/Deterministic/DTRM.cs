using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(DTRMObjectManager))]

public class DTRM : MonoBehaviour {

	public static DTRM singleton;
	private DTRMObjectManager objectManager;

	void Start() {

		//
		// Getting components
		//

		objectManager = GetComponent<DTRMObjectManager>();
		objectManager.Gather();

		singleton = this;

	}

	//
	// Players & Network
	//

	private const int thisPlayerID = 1; // THIS IS PLACEHOLDER CODE!!! TODO : REMOVE НАХУЙ
	private const int maxPlayers = 10;
	public int activePlayers = 2; // TODO: IMPLEMENT
	public bool singlePlayer {

		get { return ( gameType == GameType.Single ); }

	}
	private string serverIP = "127.0.0.1";

	private enum GameType {

		Menu, // the game hasn't started yet
		Single, // it's a single player game
		Server, // player's a server
		Client, // player's a client

	}

	private GameType gameType = GameType.Menu;
	private string connectionError = "";

	private void OnServerInitialized() {

		gameType = GameType.Server;

	}

	private void OnConnectedToServer() {

		gameType = GameType.Client;

	}

	private void OnFailedToConnect(NetworkConnectionError connectionError) {

		this.connectionError = connectionError.ToString();

	}

	//
	// Time
	//

	private FixedPoint _dtrmTime = new FixedPoint(0);
	public FixedPoint dtrmTime {

		get { return _dtrmTime; }

	}

	private FixedPoint dtrmPreviousStepTime = new FixedPoint(0);
	public FixedPoint dtrmDeltaTime {

		get { return dtrmTime - dtrmPreviousStepTime; }

	}

	private const float TIME_STEP = 0.05f; // how much does it take to complete one step in real time
	private FixedPoint DTRM_STEP = new FixedPoint(TIME_STEP); // how much does it take to complete one step in dtrm time

	private int _currentStep = 0;
	public int currentStep {

		get { return _currentStep; }

	}

	// amount of steps the other players are allowed to lag behind
	public const int LAG_STEP = 5;
	private bool _waiting = false;
	public bool waiting { get { return _waiting; } }

	private void IncreaseTime(FixedPoint deltaTime) {

		dtrmPreviousStepTime = dtrmTime;
		_dtrmTime += deltaTime;
		_currentStep++;

	}

	private void Step() {

		if ( !ReadyForStep(currentStep + 1) ) {

			_waiting = true;
			return;

		}

		_waiting = false;
		IncreaseTime(DTRM_STEP);

		// putting orders that were collected

		// execute update
		objectManager.SendUpdate();

		// put hash code
		if (!singlePlayer)
			networkView.RPC( "PutHashCode", RPCMode.All, currentStep, Network.player, GetHashCode() );

	}

	private bool playing = false;
	private float lastStepTime = 0;

	void Update() {

		if (!playing)
			return;

		if (desync)
			return;

		if ( Time.time - lastStepTime > TIME_STEP ) {

			lastStepTime = Time.time;
			Step();

		}

	}

	//
	// Debug GUI
	//	

	public void OnGUI() {

		if (gameType == GameType.Menu)
			MenuGUI();
		else
			GameGUI();

	}

	private void MenuGUI() {

		if (GUI.Button (new Rect(10, 10, 150, 20), "Start Server") )
			Network.InitializeServer(32, 12345, false);

		if (GUI.Button (new Rect(10, 70, 150, 20), "Connect") ) {
		
			Network.Connect(serverIP, 12345);
			connectionError = "Connecting...";

		}

		serverIP = GUI.TextField(new Rect(10, 40, 150, 20), serverIP);

		GUI.Label(new Rect(170, 40, 150, 20), connectionError);

	}

	private void GameGUI() {

		GUI.Label( new Rect(330, 10, 150, 20), GetHashCode().ToString() );

		GUI.Label( new Rect(500, 10, 150, 20), currentStep.ToString() );

		// alarm messages
		Rect alarmRect = new Rect(Screen.width/2 - 75, Screen.height/2 - 10, 150, 20);

		if (desync) {
			GUI.Label( alarmRect, "Desync!");
			return;
		}

		if ( waiting ) {
			GUI.Label( alarmRect, "Waiting..." );
			return;
		}

		// standard display

		if ( GUI.Button( new Rect(10, 10, 150, 20), "Step" ) )
			Step();

		string playPauseLabel = playing ? "Pause" : "Play";

		if ( GUI.Button( new Rect(170, 10, 150, 20), playPauseLabel ) )
			playing = !playing;

		GUI.Label( new Rect( 10, 40, 150, 20), "IP: " + Network.player.ipAddress + ":" + Network.player.port.ToString() );

	}

	//
	// Hash control
	//

	private Dictionary<int, Dictionary<NetworkPlayer, int>> hashHistory = new Dictionary<int, Dictionary<NetworkPlayer, int>>();
	private bool desync = false;

	public override int GetHashCode() {

		return objectManager.GetHashCode();

	}

	[RPC]
	private void PutHashCode(int step, NetworkPlayer player, int hash) {

		if(!hashHistory.ContainsKey(step))
			hashHistory.Add( step, new Dictionary<NetworkPlayer, int>() );

		hashHistory[step].Add(player, hash);

		if (hashHistory[step].Count == activePlayers)
			CheckHashCode(step);

	}

	private void CheckHashCode(int step) {

		int myHash = hashHistory[step][Network.player];

		foreach(NetworkPlayer player in hashHistory[step].Keys) {

			int hisCash = hashHistory[step][player];

			if (hisCash != myHash) {

				Debug.LogError("Desync! Player: " + player.ToString() + " step: " + step + " my hash: " + myHash + " his hash: " + hisCash);
				desync = true;
				return;

			}

		}

	}

	private bool ReadyForStep(int step) {

		// if it's one of the first steps, we are certainly ready
		if (step < LAG_STEP)
			return true;

		if ( !hashHistory.ContainsKey(step - LAG_STEP + 1) )
			return false;

		// otherwise, we need to make sure that other players already sent us info about their previous step
		return ( hashHistory[step - LAG_STEP + 1].Count == activePlayers );

	}

}
