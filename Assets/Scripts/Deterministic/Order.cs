using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public interface IOrderReceiver {

	void ReceiveOrder(Order order );

}

public class Order {

	private int _destinationID;
	public int destinationID {
		get { return _destinationID; }
	}

	public Order(int destinationID) {

		this._destinationID = destinationID;

	}

}

public class OrderGroup {

	public List<Order> orders = new List<Order>();
	public int destinationStep;
	public int playerID;

	public OrderGroup(int destinationStep, int playerID) {

		this.destinationStep = destinationStep;
		this.playerID = playerID;

	}

}

public class OrderQueue {

	private Dictionary<int, List<OrderGroup>> queue = new Dictionary<int, List<OrderGroup>>();

	// returns null in case orders aren't ready yet
	public List<OrderGroup> GetOrders(int stepNumber) {

		List<OrderGroup> orderGroups = queue[stepNumber];
		
		if (orderGroups == null)
			return null;

		if (orderGroups.Count < DTRM.singleton.activePlayers)
			return null;

		return orderGroups;

	}

	public void PutOrders(OrderGroup orderGroup) {

		if (!queue.ContainsKey(orderGroup.destinationStep))
			queue.Add( orderGroup.destinationStep, new List<OrderGroup>() );

		queue[orderGroup.destinationStep].Add(orderGroup);

	}

}