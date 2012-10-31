using UnityEngine;
using System.Collections;

public class FoFSystem : MonoBehaviour {

	public static bool AreEnemies(Component c1, Component c2) {

		return !c1.CompareTag(c2.tag);

	}

}
