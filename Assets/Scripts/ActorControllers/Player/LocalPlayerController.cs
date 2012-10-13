using UnityEngine;
using System.Collections;

// serves as a singleton for a localplayer
public abstract class LocalPlayerController : PlayerController {

	public static LocalPlayerController localPlayer;

	protected virtual void Start() {

		LocalPlayerController.localPlayer = this;

	}

	public abstract void GiveTarget(Rigidbody target);

	public abstract void GiveTarget(Vector2 target);

}
