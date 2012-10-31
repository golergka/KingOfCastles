using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public interface IOrderReceiver {

	void ReceiveOrder(Order order );

}

public abstract class Order {

	private DTRMComponent _destination;
	public DTRMComponent destination {
		get { return _destination; }
	}

	public Order(DTRMComponent destination) {

		this._destination = destination;

	}
	
	public abstract void Execute();

}

[RequireComponent(typeof(NetworkView))]

public abstract class OrderManager : DTRMComponent {
	
	private Dictionary <int, List<Order>> orderQueue = new Dictionary<int, List<Order>> ();
	
	// используется для отсылки сообщений
	protected int destinationStep {
		
		get {
			
			return (DTRM.singleton.currentStep + DTRM.LAG_STEP);
			
		}
		
	}
	
	protected void PutOrder(Order order, int destinationStep) {
		
		if ( !orderQueue.ContainsKey(destinationStep) )
			orderQueue.Add (destinationStep, new List<Order>() );
		
		orderQueue[destinationStep].Add (order);
		
	}
	
	public override void DTRMUpdate () {
		
		base.DTRMUpdate ();
		
		int step = DTRM.singleton.currentStep;
		
		if ( !orderQueue.ContainsKey(step) )
			return;
		
		foreach(Order order in orderQueue[step])
			order.Execute();
		
	}

}