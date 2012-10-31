using UnityEngine;
using System.Collections;

public class TargetableRigidbody : MonoBehaviour {

	void OnMouseUpAsButton() {

		Health health = GetComponent<Health>();

		if(health != null) {

			HeroOrderManager.singleton.AttackOrder(health);

		}

	}

}
