using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Attack))]
[RequireComponent(typeof(Legs))]
[RequireComponent(typeof(Vision))]

public class HeroController : ActorController, IAttackListener, ILegsListener {

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
		
		attack = GetComponent<Attack>();
		legs = GetComponent<Legs>();
		vision = GetComponent<Vision>();

	}
	
	#region Controlling player
	
	public void GiveTarget(Health targetVictim) {
		
		Debug.Log ("Got target health!");

		if (targetVictim == null)
			return;

		if (!attack.CheckTarget(targetVictim))
			return;

		activeState = PlayerState.Attack;
		attack.AppointTarget(targetVictim);
		legs.FollowTarget(targetVictim.myPosition);

	}

	public void GiveTarget(DTRMVector2 target) {

		activeState = PlayerState.Move;
		attack.DropTarget();
		legs.targetPosition = target;

	}
	
	#endregion

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
