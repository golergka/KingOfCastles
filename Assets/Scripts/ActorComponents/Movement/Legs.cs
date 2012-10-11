using UnityEngine;
using System.Collections;

interface ILegsListener {

	void OnTargetReach();
	void OnTargetUnreachable();

}

public abstract class Legs : MonoBehaviour {

	public float targetReachDistance = 0.1f;

	private Component[] legsListeners;

	protected bool moving = false;
	private Vector2 _target;
	public Vector2 target {

		get { return _target; }

		set {

			_target = value;
			moving = true;

		}
	}

	protected virtual void Start() {

		legsListeners = GetComponents(typeof(ILegsListener));

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

		if (!moving)
			return;

		if ( (TerrainCoordinates.TerrainToGlobal(target) - transform.position).magnitude < targetReachDistance ) {

			moving = false;
			SendTargetReachMessage();
			StopMovement();

		}

	}

}
