using UnityEngine;
using System.Collections;

// serves as a singleton for a localplayer
public abstract class LocalPlayerController : PlayerController {

	public static LocalPlayerController localPlayer;

	public override void DTRMStart() {

		LocalPlayerController.localPlayer = this;

	}

	public abstract void GiveTarget(DTRMPosition target);

	public abstract void GiveTarget(DTRMVector2 target);

}
