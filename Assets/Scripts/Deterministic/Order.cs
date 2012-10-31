using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public interface IOrderReceiver {

	void ReceiveOrder(Order order );

}

public abstract class Order {

	private int _destinationID;
	public int destinationID {
		get { return _destinationID; }
	}

	public Order(int destinationID) {

		this._destinationID = destinationID;

	}

}

public abstract class OrderQueue {

}