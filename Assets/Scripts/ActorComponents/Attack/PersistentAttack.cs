using UnityEngine;
using System.Collections;

public class PersistentAttack : Attack, IVisionListener {

	public float attackRange;
	public float period;

	private float lastAttackTime = 0f;

	public void OnNoticed(Visible observee) {

		// Я сделан из мяса.

	}

	public void OnLost(Visible observee) {

		Health target = observee.GetComponent<Health>();
		if ( target != null && target == currentTarget ) {

			currentTarget = null;
			SendLostAppointedTargetMessage();

		}

	}

	public override void AppointTarget( Health target ) {

		currentTarget = target;

	}
	
	// Update is called once per frame
	void Update () {

		if ( currentTarget == null )
			return;

		if (
			Time.time - lastAttackTime > period &&
			( rigidbody.position - currentTarget.rigidbody.position ).magnitude < attackRange
			) {

			ApplyDamage();
			lastAttackTime = Time.time;

		}
	
	}
}
