using UnityEngine;
using System.Collections;

public class TargetableTerrain : MonoBehaviour {

	public Transform mousePointer;

	void OnMouseUpAsButton() {

		Debug.Log("Click! > " + Time.time);

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit) ) {

			DTRMVector2 targetPosition = new DTRMVector2( TerrainCoordinates.GlobalToTerrain(hit.point) );
			LocalPlayerController.localPlayer.GiveTarget( targetPosition );

		}

	}

	void Update() {

		if (mousePointer != null) {

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
				mousePointer.position = hit.point;

			if (Input.GetMouseButton(0)) {

				mousePointer.renderer.material.color = Color.red;

			} else {

				mousePointer.renderer.material.color = Color.white;

			}

		}

	}

}
