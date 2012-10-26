using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DTRM : MonoBehaviour {

	public static DTRM singleton;

	void Start() {

		// get all the objects

		dtrmComponents = new List<DTRMComponent>();
		Object[] objects = FindObjectsOfType(typeof(DTRMComponent));

		foreach( Object obj in objects ) {

			dtrmComponents.Add( (DTRMComponent) obj );

		}

		singleton = this;

		// TODO: sort list by hash


		// assign IDs to objects in order

		// int idCounter = 1;
		// foreach(DTRMComponent component in dtrmComponents)
		// 	component.dtrmID = idCounter++;
		for (int i = 0; i < dtrmComponents.Count; i++)
			dtrmComponents[i].dtrmID = i;

		// call DTRMStart on each of objects
		foreach (DTRMComponent component in dtrmComponents)
			component.DTRMStart();

		//
		// Orders
		//

		ordersToSend = new OrderGroup( orderStepShift, thisPlayerID );

	}

	//
	// Object management
	//

	// !!!
	// dtrmComponents[i].dtrmID == i
	// !!!
	List<DTRMComponent> dtrmComponents;

	//
	// Players
	//

	private const int thisPlayerID = 1; // THIS IS PLACEHOLDER CODE!!! TODO : REMOVE НАХУЙ
	private const int maxPlayers = 10;
	public int activePlayers = 1; // TODO: IMPLEMENT

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

	private DTRMLong dtrmStep = new DTRMLong(0.01f);

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

			waitingForOrders = false;

			foreach(OrderGroup ordersToExecute in orderGroupsToExecute) {

				foreach(Order orderToExecute in ordersToExecute.orders ) {
					
					DTRMComponent receiver = dtrmComponents[orderToExecute.destinationID];
					
					if (receiver is IOrderReceiver) {

						IOrderReceiver orderReceiver = (IOrderReceiver) receiver;
						orderReceiver.ReceiveOrder(orderToExecute);

					} else {

						Debug.LogError("Receiver " + receiver.ToString() + "doesn't have IOrderReceiver interface!");

					}

				}

			}

		}

		// TODO: Send orders to other players!

		// include my orders
		orderQueue.PutOrders(ordersToSend);

		// increase step
		dtrmPreviousStepTime = dtrmTime;
		_dtrmTime += dtrmStep;
		_currentStep++;

		// execute update
		foreach( DTRMComponent component in dtrmComponents )
			if (component.gameObject.active)
				component.DTRMUpdate();

		// create new ordersToSend
		ordersToSend = new OrderGroup( currentStep + orderStepShift, thisPlayerID );

	}

	private bool playing = false;
	private float lastStepTime = 0;

	void Update() {

		if (!playing)
			return;

		if ( Time.time - lastStepTime > dtrmStep.ToFloat() ) {

			Step();
			lastStepTime = Time.time;

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

	//
	// Debug GUI
	//	

	public void OnGUI() {

		if ( GUI.Button( new Rect(10, 10, 150, 20), "Step" ) )
			Step();

		string playPauseLabel = playing ? "Pause" : "Play";

		if ( GUI.Button( new Rect(170, 10, 150, 20), playPauseLabel ) )
			playing = !playing;

		GUI.Label( new Rect(330, 10, 150, 20), GetHashCode().ToString() );

		if ( waitingForOrders )
			GUI.Label( new Rect(490, 10, 150, 20), "Waiting..." );

	}

	//
	//
	//

	public override int GetHashCode() {

		unchecked {

			int hash = 17;
			foreach (DTRMComponent component in dtrmComponents)
				hash = hash * 23 + component.GetHashCode();
			return hash;

		}

	}

}
