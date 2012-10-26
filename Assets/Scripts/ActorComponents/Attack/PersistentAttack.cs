using UnityEngine;
using System.Collections;

public class PersistentAttack : Attack, IVisionListener {

	public DTRMLong attackRange = new DTRMLong(10);
	public DTRMLong period = new DTRMLong(1);

	private DTRMLong lastAttackTime = new DTRMLong();

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

	public override void DropTarget() {

		currentTarget = null;

	}
	
	// Update is called once per frame
	public override void DTRMUpdate () {

		if ( currentTarget == null )
			return;

		DTRMLong timePassed = DTRM.singleton.dtrmTime - lastAttackTime;

		if ( timePassed < period )
			return;

		DTRMLong targetDistance = ( myPosition.position - currentTarget.myPosition.position ).magnitude;

		if ( targetDistance > attackRange )
			return;

		ApplyDamage();
		lastAttackTime = DTRM.singleton.dtrmTime;
	
	}

	public override int GetHashCode() {

		unchecked {

			int hash = 17;
			hash = hash * 23 + base.GetHashCode();
			hash = hash * 23 + attackRange.GetHashCode();
			hash = hash * 23 + period.GetHashCode();
			return hash;

		}

	}
}
