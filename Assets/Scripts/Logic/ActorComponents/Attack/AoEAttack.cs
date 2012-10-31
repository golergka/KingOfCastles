using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface IAoEAttackListener {

	void OnAoEAttack(List<Health> victims);

}

[RequireComponent(typeof(Vision))]

public class AoEAttack : MonoBehaviour {

	// public int damage;
	// public float attackRange;

	// private Component[] listeners;
	// private Vision vision;

	// void Start() {

	// 	listeners = GetComponents(typeof(IAoEAttackListener));
	// 	vision = GetComponent<Vision>();

	// }

	// Boom! Bada-boom!!
	public void Attack() {

		// Turned off due to Vision refactor. Need to rewrite later.

		// List<Health> victims = new List<Health>();

		// foreach(Visible target in vision.visiblesInSight) {

		// 	if (!FoFSystem.AreEnemies(this, target))
		// 		continue;

		// 	Health targetHealth = target.GetComponent<Health>();

		// 	if (targetHealth == null)
		// 		continue;

		// 	if ( (target.rigidbody.position - rigidbody.position).magnitude > attackRange )
		// 		continue;

		// 	targetHealth.InflictDamage( damage );

		// }

		// foreach(IAoEAttackListener listener in listeners)
		// 	listener.OnAoEAttack(victims);

	}

	// public override int GetHashCode() {

	// 	unchecked { // thank you Jon Skeet!

	// 		int hash = 17;
	// 		hash = hash * 23 + base.GetHashCode();
	// 		hash = hash * 23 + damage.GetHashCode();
	// 		hash = hash * 23 + attackRange.GetHashCode();
	// 		return hash;

	// 	}

	// }

}
