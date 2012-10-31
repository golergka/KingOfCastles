using UnityEngine;
using System.Collections;

public class InstantAttack : Attack {

	public override void AppointTarget( Health target ) {

		currentTarget = target;
		ApplyDamage();

	}

	public override void DropTarget() {

	}

	public override void DTRMUpdate() {
		
	}

}
