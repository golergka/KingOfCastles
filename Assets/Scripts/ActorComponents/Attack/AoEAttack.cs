using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface IAoEAttackListener {

	void OnAoEAttack(List<Health> victims);

}

[RequireComponent(typeof(Vision))]

public class AoEAttack : MonoBehaviour {

	public int damage;
	public float attackRange;

	private Component[] listeners;
	private Vision vision;

	void Start() {

		listeners = GetComponents(typeof(IAoEAttackListener));
		vision = GetComponent<Vision>();

	}

	// Boom! Bada-boom!!
	public void Attack() {

		List<Health> victims = new List<Health>();

		foreach(Visible target in vision.visiblesInSight) {

			if (!FoFSystem.AreEnemies(this, target))
				continue;

			Health targetHealth = target.GetComponent<Health>();

			if (targetHealth == null)
				continue;

			if ( (target.rigidbody.position - rigidbody.position).magnitude > attackRange )
				continue;

			targetHealth.InflictDamage( damage );

		}

		foreach(IAoEAttackListener listener in listeners)
			listener.OnAoEAttack(victims);

	}

}
