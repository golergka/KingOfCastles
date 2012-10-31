using UnityEngine;
using System.Collections;

interface ILegsListener {

	void OnTargetReach();
	void OnTargetUnreachable();

}

[RequireComponent(typeof(DTRMPosition))]

public abstract class Legs : DTRMComponent {

	public FixedPoint dtrmSqTargetPositionReach = new FixedPoint(10);
	public FixedPoint dtrmSqTargetPositionClose = new FixedPoint(50);
	public FixedPoint dtrmSqTargetActorReach = new FixedPoint(10);
	public FixedPoint dtrmSqTargetActorFollow = new FixedPoint(100);
	public FixedPoint dtrmSqTargetActorBump = new FixedPoint(10);

	private Component[] legsListeners;

	protected enum LegsState {

		Idle, // нет назначения, ноги остаются на месте
		MovingToPosition, // есть назначение — место в пространстве
		PursuingTarget, // есть назначение — rigidbody, до которого надо дойти
		FollowingTarget, // есть назначение — rigidbody, за которым постоянно следуем

	}

	protected LegsState legsState = LegsState.Idle;

	private DTRMVector2 _targetPosition;
	public DTRMVector2 targetPosition {

		get { return _targetPosition; }

		set {

			_targetPosition = value;
			legsState = LegsState.MovingToPosition;

		}
	}

	protected DTRMPosition targetActor;

	public override int GetHashCode() {

		unchecked {

			int hash = 17;
			hash = hash * 23 + dtrmSqTargetPositionReach.GetHashCode();
			hash = hash * 23 + dtrmSqTargetActorReach.GetHashCode();
			hash = hash * 23 + dtrmSqTargetActorFollow.GetHashCode();
			hash = hash * 23 + dtrmSqTargetActorBump.GetHashCode();
			hash = hash * 23 + legsState.GetHashCode();
			return hash;

		}

	}

	public override void DTRMStart() {

		legsListeners = GetComponents(typeof(ILegsListener));
		legsState = LegsState.Idle;

	}

	public void PursueTarget(DTRMPosition target) {

		legsState = LegsState.PursuingTarget;
		targetActor = target;

	}

	public void FollowTarget(DTRMPosition target) {

		legsState = LegsState.FollowingTarget;
		targetActor = target;

	}

	public void Stop () {

		legsState = LegsState.Idle;
		StopMovement();

	}

	private void SendTargetReachMessage() {

		foreach(Component listener in legsListeners)
			((ILegsListener)listener).OnTargetReach();

	}

	protected void SendTargetUnreachableMessage() {

		foreach(Component listener in legsListeners)
			((ILegsListener)listener).OnTargetUnreachable();

	}

	protected abstract void StopMovement();

	protected void CheckTargetReach() {

		switch(legsState) {

			case LegsState.Idle:
				break;

			case LegsState.MovingToPosition:

				if ( (targetPosition - myPosition.position).sqrMagnitude < dtrmSqTargetPositionReach ) {

					legsState = LegsState.Idle;
					SendTargetReachMessage();
					StopMovement();

				}
				break;

			case LegsState.PursuingTarget:

				if ( ( targetActor.position - myPosition.position ).sqrMagnitude < dtrmSqTargetPositionReach ) {

					legsState = LegsState.Idle;
					SendTargetReachMessage();
					StopMovement();

				}
				break;

			case LegsState.FollowingTarget:
				Debug.LogWarning("Shouldn't be checking target reach in this state!");
				break;

			default:
				Debug.LogError("Unknown state!");
				break;

		}

	}

	void OnDrawGizmosSelected() {

		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere( transform.position, Mathf.Sqrt( dtrmSqTargetPositionReach.ToFloat() ) );

		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere( transform.position, Mathf.Sqrt( dtrmSqTargetActorReach.ToFloat() ) );

		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere( transform.position, Mathf.Sqrt( dtrmSqTargetActorFollow.ToFloat() ) );

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere( transform.position, Mathf.Sqrt( dtrmSqTargetActorBump.ToFloat() ) );

	}

	// void OnDrawGizmos() {

	// 	Vector3 liftoff = new Vector3(0,1,0);

	// 	switch (legsState) {

	// 		case LegsState.Idle:
	// 			Gizmos.color = Color.blue;
	// 			Gizmos.DrawSphere( rigidbody.position + liftoff, 0.1f );
	// 			break;

	// 		case LegsState.MovingToPosition:
	// 			Gizmos.color = Color.green;
	// 			Gizmos.DrawSphere( TerrainCoordinates.TerrainToGlobal(targetPosition) + liftoff, 0.1f );
	// 			Gizmos.DrawLine( rigidbody.position, TerrainCoordinates.TerrainToGlobal(targetPosition) );
	// 			break;

	// 		case LegsState.PursuingTarget:
	// 			Gizmos.color = Color.yellow;
	// 			Gizmos.DrawSphere( targetActor.position + liftoff, 0.1f );
	// 			Gizmos.DrawLine( rigidbody.position, targetActor.position );
	// 			break;

	// 		case LegsState.FollowingTarget:
	// 			Gizmos.color = Color.cyan;
	// 			Gizmos.DrawSphere( targetActor.position + liftoff, 0.1f );
	// 			Gizmos.DrawLine( rigidbody.position, targetActor.position );
	// 			break;

	// 	}

	// }

	public override void DTRMUpdate() {

	}

}
