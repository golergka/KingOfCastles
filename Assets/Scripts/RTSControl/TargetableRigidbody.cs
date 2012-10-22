using UnityEngine;
using System.Collections;

public class TargetableRigidbody : MonoBehaviour {

	void OnMouseUpAsButton() {

		DTRMPosition position = GetComponent<DTRMPosition>();

		if(position != null)
			LocalPlayerController.localPlayer.GiveTarget(position);

	}

}
