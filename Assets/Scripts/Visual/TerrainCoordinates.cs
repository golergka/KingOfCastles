using UnityEngine;
using System.Collections;

public class TerrainCoordinates : MonoBehaviour {

	static public Vector2 GlobalToTerrain(Vector3 coordinates) {

		return new Vector2(coordinates.x, coordinates.z);

	}

	static public Vector3 TerrainToGlobal(Vector2 coordinates, float lift = 0f) {

		Vector3 result = new Vector3( coordinates.x, 0, coordinates.y );
		result.y = Terrain.activeTerrain.SampleHeight(result) + lift;
		return result;

	}

	static public Vector3 ToTerrain(Vector3 coordinates, float lift = 0f) {

		Vector3 result = coordinates;
		result.y = Terrain.activeTerrain.SampleHeight(result) + lift;
		return result;

	}

}
