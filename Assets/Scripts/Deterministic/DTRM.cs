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

		//
		// Orders
		//

		ordersToSend = new OrderGroup( orderStepShift, thisPlayerID );

	}

	//
	// Players & Network
	//

	private const int thisPlayerID = 1; // THIS IS PLACEHOLDER CODE!!! TODO : REMOVE НАХУЙ
	private const int maxPlayers = 10;
	public int activePlayers = 1; // TODO: IMPLEMENT
	public bool singlePlayer {

		get { return ( gameType == GameType.Single ); }

	}

	private enum GameType {

		Menu, // the game hasn't started yet
		Single, // it's a single player game
		Server, // player's a server
		Client, // player's a client

	}

	private GameType gameType = GameType.Single;

	//
	// Time
	//

	private DTRMLong _dtrmTime = new DTRMLong(0);
	public DTRMLong dtrmTime {

		get { return _dtrmTime; }

	}

	private DTRMLong dtrmPreviousStepTime = new DTRMLong(0);

	public DTRMLong dtrmDeltaTime {

		get { return dtrmTime - dtrmPreviousStepTime; }

	}

	private DTRMLong dtrmStep = new DTRMLong(0.05f);

	private int _currentStep = 0;
	public int currentStep {

		get { return _currentStep; }

	}

	private void Step() {

		// execute orders or wait for them
		if (currentStep > orderStepShift) {

			List<OrderGroup> orderGroupsToExecute = orderQueue.GetOrders(currentStep + 1);
			if ( orderGroupsToExecute == null ) {

				waitingForOrders = true;
				return;

			}

			// increase step number
			dtrmPreviousStepTime = dtrmTime;
			_dtrmTime += dtrmStep;
			_currentStep++;

			waitingForOrders = false;
			ExecuteOrders(orderGroupsToExecute);

		} else {

			// increase step number
			dtrmPreviousStepTime = dtrmTime;
			_dtrmTime += dtrmStep;
			_currentStep++;

		}

		// putting orders that were collected
		if (singlePlayer) {

			orderQueue.PutOrders(ordersToSend);

		} else {
		
			networkView.RPC("PutOrders", RPCMode.All, ordersToSend);

		}

		// execute update
		objectManager.SendUpdate();

		// create new ordersToSend
		ordersToSend = new OrderGroup( currentStep + orderStepShift, thisPlayerID );

		// put hash code
		if (!singlePlayer)
			networkView.RPC( "PutHashCode", RPCMode.All, currentStep, thisPlayerID, GetHashCode() );

	}

	private bool playing = false;
	private float lastStepTime = 0;

	void Update() {

		if (!playing)
			return;

		if ( Time.time - lastStepTime > dtrmStep.ToFloat() ) {

			lastStepTime = Time.time;
			Step();

		}

	}

	//
	// Orders
	//

	private const int orderStepShift = 5;
	private OrderGroup ordersToSend; // orders to collect and send during this frame
	private OrderQueue orderQueue = new OrderQueue();
	private bool waitingForOrders = false;

	public void PutOrder(Order order) {

		ordersToSend.orders.Add(order);

	}

	[RPC]
	private void PutOrders(OrderGroup orderGroup) {

		orderQueue.PutOrders(orderGroup);

	}

	private void ExecuteOrders(List<OrderGroup> orderGroupsToExecute) {

		foreach(OrderGroup ordersToExecute in orderGroupsToExecute) {

			foreach(Order orderToExecute in ordersToExecute.orders ) {
				
				DTRMComponent receiver = objectManager.GetObject(orderToExecute.destinationID);
				
				if (receiver is IOrderReceiver) {

					IOrderReceiver orderReceiver = (IOrderReceiver) receiver;
					orderReceiver.ReceiveOrder(orderToExecute);

				} else {

					Debug.LogError("Receiver " + receiver.ToString() + "doesn't have IOrderReceiver interface!");

				}

			}

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

	private void MenuGUI() { }

	private void GameGUI() {

		// alarm messages
		Rect alarmRect = new Rect(Screen.width - 75, Screen.height - 10, 150, 20);

		if (desync) {
			GUI.Label( alarmRect, "Desync!");
			return;
		}

		if ( waitingForOrders ) {
			GUI.Label( alarmRect, "Waiting..." );
			return;
		}

		// standard display

		if ( GUI.Button( new Rect(10, 10, 150, 20), "Step" ) )
			Step();

		string playPauseLabel = playing ? "Pause" : "Play";

		if ( GUI.Button( new Rect(170, 10, 150, 20), playPauseLabel ) )
			playing = !playing;

		GUI.Label( new Rect(330, 10, 150, 20), GetHashCode().ToString() );

	}

	//
	// Hash control
	//

	private Dictionary<int, Dictionary<int, int>> hashHistory = new Dictionary<int, Dictionary<int, int>>();
	private bool desync = false;

	public override int GetHashCode() {

		return objectManager.GetHashCode();

	}

	[RPC]
	private void PutHashCode(int step, int playerID, int hash) {

		if(!hashHistory.ContainsKey(step))
			hashHistory.Add( step, new Dictionary<int, int>() );

		hashHistory[step].Add(playerID, hash);

		if (hashHistory[step].Count == activePlayers)
			CheckHashCode(step);

	}

	private void CheckHashCode(int step) {

		int myHash = hashHistory[step][thisPlayerID];

		foreach(int playerID in hashHistory[step].Keys) {

			if (hashHistory[step][playerID] != myHash) {

				Debug.LogError("Desync! Player: " + playerID + " step: " + step);
				desync = true;
				return;

			}

		}

	}

}
