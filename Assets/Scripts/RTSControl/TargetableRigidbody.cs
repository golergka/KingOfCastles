using UnityEngine;
using System.Collections;

public class TargetableRigidbody : MonoBehaviour {

	void OnMouseUpAsButton() {

		LocalPlayerController.localPlayer.GiveTarget(rigidbody);

	}

}
