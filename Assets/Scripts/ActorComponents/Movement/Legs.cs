using UnityEngine;
using System.Collections;

interface ILegsListener {

	void OnTargetReach();
	void OnTargetUnreachable();

}

public abstract class Legs : MonoBehaviour {

	Component[] legsListeners;

	public virtual void Start() {

		legsListeners = GetComponents(typeof(ILegsListener));

	}

	private void SendTargetReachMessage() {

		foreach(Component listener in legsListeners)
			((ILegsListener)listener).OnTargetReach();

	}

	private void SendTargetUnreachableMessage() {

		foreach(Component listener in legsListeners)
			((ILegsListener).listener).OnTargetUnreachable();

	}

}
