using UnityEngine;
using System.Collections;

interface IActorListener {
	
}

public abstract class ActorController : MonoBehaviour, IHealthStateListener {

	//
	// IHealthStateListener
	//

	public void OnFullHealth() {

		// Нам похуй снова.

	}

	public void OnZeroHealth() {

		gameObject.active = false;

	}

}
