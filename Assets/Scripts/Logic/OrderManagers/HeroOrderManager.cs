using UnityEngine;
using System.Collections;

public class HeroOrderManager : OrderManager {
	
	public static HeroOrderManager singleton;
	
	private void Start() {
		
		singleton = this;
		
	}
	
	#region Order subclasses
	
	private class HeroOrder : Order {

		public enum HeroOrderType {
			
			Attack,
			Move,
			
		}
		
		public HeroOrderType orderType;
		public DTRMVector2 targetPosition;
		public Health targetVictim;
	
		public HeroOrder(HeroController destination, Health targetVictim) : base(destination) {
	
			orderType = HeroOrderType.Attack;
			this.targetVictim = targetVictim;
	
		}
	
		public HeroOrder(HeroController destination, DTRMVector2 targetPosition) : base(destination) {
	
			orderType = HeroOrderType.Move;
			this.targetPosition = targetPosition;
	
		}
		
		public override void Execute() {
			
			if (!( destination is HeroController )) {
				
				Debug.LogError("Invalid destination: " + destination.ToString() );
				return;
				
			}
			
			HeroController player = (HeroController) destination;
			
			switch(orderType) {
				
			case HeroOrder.HeroOrderType.Attack:
				player.GiveTarget(targetVictim);
				break;
				
			case HeroOrder.HeroOrderType.Move:
				player.GiveTarget(targetPosition);
				break;
				
			default:
				Debug.LogError("Unknown order type!");
				break;
				
			}
			
		}
		
	}
	
	private class CreateHeroOrder : Order {
	
		public NetworkPlayer owner;
		
		public CreateHeroOrder(NetworkPlayer owner) : base (null) {
			
			this.owner = owner;
			
		}
		
		public override void Execute() {
			
			HeroManager.singleton.NewHero(owner);
			
		}
		
	}
	
	#endregion
	
	#region Public send methods
	
	public void CreateOrder(NetworkPlayer owner) {
		
		networkView.RPC ("ReceiveCreateOrder", RPCMode.All, destinationStep, owner);
	}
	
	public void AttackOrder(Health targetVictim) {
		
		HeroController myHero = HeroManager.singleton.Hero (Network.player);
		networkView.RPC("ReceiveAttackOrder", RPCMode.All, destinationStep, targetVictim.dtrmID, myHero.dtrmID);
		
	}
	
	public void MoveOrder(DTRMVector2 targetPosition) {
		
		HeroController myHero = HeroManager.singleton.Hero (Network.player);
		networkView.RPC("ReceiveMoveOrder", RPCMode.All, destinationStep, targetPosition.ToVector3(), myHero.dtrmID);
		
	}
	
	#endregion
	
	#region Private receive methods
	
	[RPC]
	private void ReceiveAttackOrder(int destinationStep, int targetVictim, int heroID) {
		
		HeroController destination = (HeroController) ObjectManager.singleton.GetObject(heroID);
		HeroOrder newOrder = new HeroOrder (destination, (Health) ObjectManager.singleton.GetObject(targetVictim) );
		PutOrder( newOrder, destinationStep);
		
	}
	
	[RPC]
	private void ReceiveMoveOrder(int destinationStep, Vector3 targetPosition, int heroID) {
		
		HeroController destination = (HeroController) ObjectManager.singleton.GetObject(heroID);
		HeroOrder newOrder = new HeroOrder (destination, new DTRMVector2(targetPosition) );
		PutOrder( newOrder, destinationStep);
		
	}
	
	[RPC]
	private void ReceiveCreateOrder(int destinationStep, NetworkPlayer owner) {
		
		CreateHeroOrder newOrder = new CreateHeroOrder(owner);
		PutOrder ( newOrder, destinationStep );
		
	}
	
	#endregion
	
}
