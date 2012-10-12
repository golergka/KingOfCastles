using UnityEngine;
using System.Collections;

interface ILegsListener {

	void OnTargetReach();
	void OnTargetUnreachable();

}

public abstract class Legs : MonoBehaviour {

	public float targetReachDistance = 0.1f;
	public float targetRigidbodyReachDistance = 1f;
	public float targetRigidbodyFollowDistance = 1f;
	public float targetRigidbodyFollowBumpDistance = 0.5f;

	private Component[] legsListeners;

	protected enum LegsState {

		Idle, // нет назначения, ноги остаются на месте
		MovingToPosition, // есть назначение — место в пространстве
		PursuingRigidbody, // есть назначение — rigidbody, до которого надо дойти
		FollowingRigidbody, // есть назначение — rigidbody, за которым постоянно следуем

	}

	protected LegsState legsState = LegsState.Idle;

	private Vector2 _targetPosition;
	public Vector2 targetPosition {

		get { return _targetPosition; }

		set {

			_targetPosition = value;
			legsState = LegsState.MovingToPosition;

		}
	}

	protected Rigidbody targetRigidbody;

	protected virtual void Start() {

		legsListeners = GetComponents(typeof(ILegsListener));
		legsState = LegsState.Idle;

	}

	public void PursueTarget(Rigidbody target) {

		legsState = LegsState.PursuingRigidbody;
		targetRigidbody = target;

	}

	public void FollowTarget(Rigidbody target) {

		legsState = LegsState.FollowingRigidbody;
		targetRigidbody = target;

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

				if ( (TerrainCoordinates.TerrainToGlobal(targetPosition) - transform.position).magnitude <
					targetReachDistance ) {

					legsState = LegsState.Idle;
					SendTargetReachMessage();
					StopMovement();

				}
				break;

			case LegsState.PursuingRigidbody:

				if ( ( targetRigidbody.position - rigidbody.position ).magnitude < targetReachDistance ) {

					legsState = LegsState.Idle;
					SendTargetReachMessage();
					StopMovement();

				}
				break;

			case LegsState.FollowingRigidbody:
				Debug.LogWarning("Shouldn't be checking target reach in this state!");
				break;

			default:
				Debug.LogError("Unknown state!");
				break;

		}

	}

	void OnDrawGizmosSelected() {

		Vector3 liftoff = new Vector3(0,1,0);

		switch (legsState) {

			case LegsState.Idle:
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere( rigidbody.position + liftoff, 0.1f );
				break;

			case LegsState.MovingToPosition:
				Gizmos.color = Color.green;
				Gizmos.DrawSphere( TerrainCoordinates.TerrainToGlobal(targetPosition) + liftoff, 0.1f );
				Gizmos.DrawLine( rigidbody.position, TerrainCoordinates.TerrainToGlobal(targetPosition) );
				break;

			case LegsState.PursuingRigidbody:
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere( targetRigidbody.position + liftoff, 0.1f );
				Gizmos.DrawLine( rigidbody.position, targetRigidbody.position );
				break;

			case LegsState.FollowingRigidbody:
				Gizmos.color = Color.cyan;
				Gizmos.DrawSphere( targetRigidbody.position + liftoff, 0.1f );
				Gizmos.DrawLine( rigidbody.position, targetRigidbody.position );
				break;

		}

	}

}
