using UnityEngine;
using System.Collections;

public class WalkingLegs : Legs {

	public int defaultSpeed = 1;

	public DTRMLong speed;

	private DTRMVector2 velocity = DTRMVector2.zero;

	private void MoveAccordingToSpeed() {

		myPosition.position += velocity * DTRM.singleton.dtrmDeltaTime;

	}

	private void MoveTowards(DTRMVector2 targetMovePosition) {

		DTRMVector2 desiredVelocity = (targetMovePosition - myPosition.position).normalized;
		desiredVelocity *= speed;
		velocity = desiredVelocity;

		MoveAccordingToSpeed();

	}

	private void MoveTowards(DTRMVector2 targetMovePosition, DTRMLong speedPercentage) {

		DTRMVector2 desiredVelocity = (targetMovePosition - myPosition.position).normalized;
		desiredVelocity *= speed;
		desiredVelocity *= speedPercentage;
		velocity = desiredVelocity;

		MoveAccordingToSpeed();

	}

	protected override void StopMovement() {

	}

	public override void DTRMStart() {

		base.DTRMStart();
		speed = new DTRMLong(defaultSpeed);

	}

	public override void DTRMUpdate() {

		// если нам ничего не надо делать — то нам ничего не надо делать
		if (legsState == LegsState.Idle)
			return;

		// во всех случаях кроме преследования мы ещё должны проверить состояние, и оно может измениться
		if (legsState != LegsState.FollowingTarget)
			CheckTargetReach();

		switch(legsState) {

			case LegsState.Idle:
				break;

			case LegsState.MovingToPosition:

				DTRMLong sqDistance = (targetPosition - myPosition.position).sqrMagnitude;

				if ( sqDistance > dtrmSqTargetPositionClose ) {

					MoveTowards(targetPosition);

				} else if ( sqDistance > dtrmSqTargetPositionReach ) {

					DTRMLong speedMultipler = (sqDistance - dtrmSqTargetPositionReach) /
						(dtrmSqTargetPositionClose - dtrmSqTargetPositionReach);

					MoveTowards(targetPosition, speedMultipler);

				}

				break;

			case LegsState.PursuingTarget:

				MoveTowards(targetActor.position);
				break;

			case LegsState.FollowingTarget:

				sqDistance = (targetActor.position - myPosition.position).sqrMagnitude;

				if ( sqDistance > dtrmSqTargetActorFollow ) {
				
					MoveTowards(targetActor.position);

				} else if ( sqDistance > dtrmSqTargetActorBump) {
					
					DTRMLong speedMultipler = (sqDistance - dtrmSqTargetActorBump) /
						(dtrmSqTargetActorFollow - dtrmSqTargetActorBump);

					MoveTowards(targetActor.position, speedMultipler );

				} else {

					StopMovement();

				}

				break;

			default:
				Debug.LogError("Unknown state!");
				break;

		}

	}

}
