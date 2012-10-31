using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Attack))]
[RequireComponent(typeof(Legs))]
[RequireComponent(typeof(Vision))]

public class PlayerOrder : Order {

		public enum PlayerOrderType {

			Attack,
			Move,

		}

		public PlayerOrderType playerOrderType;
		public DTRMVector2 position;
		public DTRMPosition target;

		public PlayerOrder(DTRMPosition target) : base(LocalPlayerController.localPlayer.dtrmID) {

			playerOrderType = PlayerOrderType.Attack;
			this.target = target;

		}

		public PlayerOrder(DTRMVector2 position) : base(LocalPlayerController.localPlayer.dtrmID) {
			
			playerOrderType = PlayerOrderType.Move;
			this.position = position;

		}

}

public class RTSLocalPlayerController : LocalPlayerController, IAttackListener, ILegsListener, IOrderReceiver {

	public GameObject targetPositionMark;
	public float targetPositionMarkLift = 1f;

	private Attack attack;
	private Legs legs;
	private Vision vision;

	private enum PlayerState {

		Idle,
		Attack,
		Move,

	}

	private PlayerState activeState = PlayerState.Idle;

	private Health targetVictim;
	private DTRMVector2 targetPosition;

	public override void DTRMStart() {

		base.DTRMStart();
		
		if (targetPositionMark != null)
			targetPositionMark.active = false;

		attack = GetComponent<Attack>();
		legs = GetComponent<Legs>();
		vision = GetComponent<Vision>();

	}

	// public override void GetOrder(params object[] orderDetails) {

	// 	if(orderDetails[0] is DTRMPosition) {

	// 		DTRMPosition newTarget = (DTRMPosition) orderDetails[0];
	// 		GiveTarget(newTarget);
	// 		return;

	// 	}

	// 	if (orderDetails[0] is DTRMVector2) {

	// 		DTRMVector2 newTarget = (DTRMVector2) orderDetails[0];
	// 		GiveTarget(newTarget);
	// 		return;

	// 	}

	// 	Debug.LogError("Incorrect order: no correct target type");

	// }

	private void TryFindTarget() {

		foreach( Visible potentialTarget in vision.VisiblesInSight() ) {

			Health target = potentialTarget.GetComponent<Health>();
			if (target == null)
				continue;

			if (attack.CheckTarget(target)) {

				attack.AppointTarget(target);
				return;

			}

		}

	}

	//
	// LocalPlayerController
	//

	public override void GiveTarget(DTRMPosition target) {

		if (target == null)
			return;

		Health targetHealth = target.GetComponent<Health>();
		if (targetHealth == null)
			return;

		if (!attack.CheckTarget(targetHealth))
			return;

		activeState = PlayerState.Attack;
		attack.AppointTarget(targetHealth);
		legs.FollowTarget(target);

	}

	public override void GiveTarget(DTRMVector2 target) {

		activeState = PlayerState.Move;
		attack.DropTarget();
		legs.targetPosition = target;

		targetPositionMark.active = true;
		targetPositionMark.transform.position = TerrainCoordinates.TerrainToGlobal(target.ToVector2(), targetPositionMarkLift);

	}

	//
	// IAttackListener
	//

	public void OnLostAppointedTarget() {

		if (activeState == PlayerState.Attack) {

			activeState = PlayerState.Idle;
			legs.Stop();

		}

		TryFindTarget();

	}

	public void OnApplyDamage() {

		// Нахуй

	}

	//
	// ILegsListener
	//

	public void OnTargetReach() {

		if (activeState == PlayerState.Move) {

			activeState = PlayerState.Idle;
			targetPositionMark.active = false;

		} else {

			Debug.LogWarning("Received unexpected target reach event for order other than moving!");

		}

		TryFindTarget();

	}

	public void OnTargetUnreachable() {

		// TODO!

		Debug.LogError("Implement OnTargetUnreachable!");

	}

	//
	// IOrderReceiver
	//

	public void ReceiveOrder(Order order) {

		if (!(order is PlayerOrder)) {

			Debug.LogError("Unknown order type!");
			return;

		}

		PlayerOrder playerOrder = (PlayerOrder) order;

		switch(playerOrder.playerOrderType) {

			case PlayerOrder.PlayerOrderType.Attack:

				GiveTarget(playerOrder.target);
				break;

			case PlayerOrder.PlayerOrderType.Move:

				GiveTarget(playerOrder.position);
				break;

			default:

				Debug.LogError("Unknown error type!");
				break;

		}

	}

}
