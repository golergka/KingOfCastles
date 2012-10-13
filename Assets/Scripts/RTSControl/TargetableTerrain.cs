using UnityEngine;
using System.Collections;

public class TargetableTerrain : MonoBehaviour {

	void OnMouseUpAsButton() {

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit) ) {

			LocalPlayerController.localPlayer.GiveTarget(TerrainCoordinates.GlobalToTerrain(hit.point));

		}

	}

}
