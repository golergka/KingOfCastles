using UnityEngine;
using System.Collections;

// serves as a singleton for a localplayer
public abstract class LocalPlayerController : PlayerController {

	public static LocalPlayerController localPlayer;

	virtual void Start() {

		LocalPlayerController.localPlayer = this;

	}

}
