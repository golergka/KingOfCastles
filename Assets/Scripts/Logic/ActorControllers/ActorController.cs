using UnityEngine;
using System.Collections;

interface IActorListener {
	
}

public abstract class ActorController : DTRMComponent, IHealthStateListener {

	public override void DTRMStart() {

		// Component[] controllers = GetComponents(typeof(ActorController));
		// if (controllers.Length > 1)
		// 	Debug.LogError("There should be only one controller per gameObject!");
		// 	// TODO: disable component for DTRM system

	}

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
