using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Legs))]

public class ProjectileController : ActorController, ILegsListener {

	private Health _target;
	public Health target {

		get { return _target; }

		set {

			_target = value;
			legs.PursueTarget(_target.myPosition);

		}

	}

	private Attack attack;
	private Legs legs;

	public override void DTRMStart() {

		base.DTRMStart();

		attack = GetComponent<Attack>();
		legs = GetComponent<Legs>();

	}

	//
	// ILegsListener
	//

	public void OnTargetReach() {

		if (attack != null)
			attack.AppointTarget(_target);

	}

	public void OnTargetUnreachable() {

	}

}
