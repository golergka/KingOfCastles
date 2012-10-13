using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Attack))]
[RequireComponent(typeof(Legs))]
[RequireComponent(typeof(Vision))]

public class RTSLocalPlayerController : LocalPlayerController, IAttackListener, ILegsListener {

	public GameObject targetPositionMark;
	public float targetPositionMarkLift = 1f;

	private Attack attack;
	private Legs legs;
	private Vision vision;

	private enum PlayerOrder {

		Idle,
		Attacking,
		Moving,

	}

	private PlayerOrder activeOrder = PlayerOrder.Idle;

	private Health targetVictim;
	private Vector2 targetPosition;

	protected override void Start() {

		base.Start();
		
		if (targetPositionMark != null)
			targetPositionMark.active = false;

		attack = GetComponent<Attack>();
		legs = GetComponent<Legs>();
		vision = GetComponent<Vision>();

	}

	private void TryFindTarget() {

		foreach( Visible potentialTarget in vision.visiblesInSight ) {

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

	public override void GiveTarget(Rigidbody target) {

		if (target == null)
			return;

		Health targetHealth = target.GetComponent<Health>();
		if (targetHealth == null)
			return;

		if (!attack.CheckTarget(targetHealth))
			return;

		activeOrder = PlayerOrder.Attacking;
		attack.AppointTarget(targetHealth);
		legs.FollowTarget(target.rigidbody);

	}

	public override void GiveTarget(Vector2 target) {

		activeOrder = PlayerOrder.Moving;
		attack.DropTarget();
		legs.targetPosition = target;

		targetPositionMark.active = true;
		targetPositionMark.transform.position = TerrainCoordinates.TerrainToGlobal(target, targetPositionMarkLift);

	}

	//
	// IAttackListener
	//

	public void OnLostAppointedTarget() {

		if (activeOrder == PlayerOrder.Attacking) {

			activeOrder = PlayerOrder.Idle;
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

		if (activeOrder == PlayerOrder.Moving) {

			activeOrder = PlayerOrder.Idle;
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

}
