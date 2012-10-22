using UnityEngine;
using System.Collections;

interface IActorListener {
	
}

public abstract class ActorController : DTRMComponent, IHealthStateListener {

	// В этом классе этот метод используется только когда здоровье достигается нуля.
	// Но! Это не единственное место где он может быть использован в подклассах.
	// И тем более код в нём должен стать намного более сложным со временем. Кеши объектов, реинициализация...
	// Так что разумнее вынести его в отдельный метод.
	protected void Die() {

		gameObject.active = false;

	}

	//
	// IHealthStateListener
	//

	public void OnFullHealth() {

		// Нам похуй снова.

	}

	public void OnZeroHealth() {

		Die();

	}

}
