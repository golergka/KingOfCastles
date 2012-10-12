using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]

public class WalkingLegs : Legs {

	public float speed = 1f;

	// Use this for initialization
	override protected void Start () {

		base.Start();
	
	}

	private void MoveTowards(Vector3 position, float speedPercentage = 1f) {

		Vector3 desiredVelocity = (position - rigidbody.position).normalized;

		if (speedPercentage == 1f) // чуток чрезмерной оптимизации
			desiredVelocity *= speed;
		else
			desiredVelocity *= speed * speedPercentage;

		Vector3 velocityChange = desiredVelocity - rigidbody.velocity;
		rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

	}
	
	void FixedUpdate() {

		// если нам ничего не надо делать — то нам ничего не надо делать
		if (legsState == LegsState.Idle)
			return;

		// во всех случаях кроме преследования мы ещё должны проверить состояние, и оно может измениться
		if (legsState != LegsState.FollowingRigidbody)
			CheckTargetReach();

		switch(legsState) {

			case LegsState.MovingToPosition:

				MoveTowards(TerrainCoordinates.TerrainToGlobal(targetPosition));
				break;

			case LegsState.PursuingRigidbody:

				MoveTowards(targetRigidbody.position);
				break;

			case LegsState.FollowingRigidbody:

				float distance = (targetRigidbody.position - rigidbody.position).magnitude;

				if ( distance > targetRigidbodyFollowDistance ) {
				
					MoveTowards(targetRigidbody.position);

				} else if ( distance > targetRigidbodyFollowBumpDistance) {
					
					float speedMultipler = (distance - targetRigidbodyFollowBumpDistance) /
					(targetRigidbodyFollowDistance - targetRigidbodyFollowBumpDistance);

					MoveTowards(targetRigidbody.position, speedMultipler );

				} else {

					StopMovement();

				}

				break;

			default:
				Debug.LogError("Unknown state!");
				break;

		}

	}

	protected override void StopMovement() {

		rigidbody.AddForce(-rigidbody.velocity, ForceMode.VelocityChange);

	}

}
