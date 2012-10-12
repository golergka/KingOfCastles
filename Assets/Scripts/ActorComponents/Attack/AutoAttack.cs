using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Vision))]

public class AutoAttack : Attack, IVisionListener {

	private float _attackRange;
	public float attackRange {

		get { return _attackRange; }

		set {

			if ( value < _attackRange ) {

				// если мы уменьшаем радиус, то надо проверить, подходит ли текущая цель под новый радиус

				_attackRange = value;
				if ( !CheckTarget(currentTarget) ) {
					LoseCurrentTarget();
					FindSomeTarget();
				}

			} else if ( currentTarget = null ) {

				// если мы увеличиваем радиус и у нас нет текущей цели, то может, теперь мы можем найти какую-нибудь?

				FindSomeTarget();

			}

		}

	}
	public float initialAttackRange; // исходный радиус атаки
	public float period; // период атаки

	private float lastAttackTime = 0f;

	private Vision vision;

	protected override void Start() {

		base.Start();

		_attackRange = initialAttackRange;
		vision = GetComponent<Vision>();

	}

	public void OnNoticed(Visible observee) {

		// если у нас уже есть цель, пофигу
		if ( currentTarget != null )
			return;

		Health target = observee.GetComponent<Health>();

		// если подходит, то атакуем
		if ( CheckTarget(target) )
			currentTarget = target;

	}

	public void OnLost(Visible observee) {

		Health target = observee.GetComponent<Health>();
		if ( target != null && target == currentTarget )
			LoseCurrentTarget();

	}

	// найти какую-нибудь подходящую цель из потенциальных
	private void FindSomeTarget() {

		foreach(Visible observee in vision.visiblesInSight) {

			Health target = observee.GetComponent<Health>();

			if (target != null && CheckTarget(target)) {
				currentTarget = target;
				break;
			}

		}

	}



	// true если цель можно сейчас атаковать
	public override bool CheckTarget( Health target ) {

		return base.CheckTarget(target) && ( rigidbody.position - target.rigidbody.position ).magnitude < attackRange;

	}

	private void LoseCurrentTarget() {

		if (appointedTarget) {

			appointedTarget = false;
			SendLostAppointedTargetMessage();

		}

		currentTarget = null;
		
	}

	// назначить цель снаружи
	public override void AppointTarget( Health target ) {

		// эти проверки нужны только для отладки, при релизе можно отключить

		if (target == null) {

			Debug.LogWarning("Received null target!");
			return;

		}

		if ( !CheckTarget(target) ) {

			Debug.LogWarning("Received illegal target!");
			return;

		}

		currentTarget = target;
		appointedTarget = true; // ставим флаг что цель назначена специально

	}

	void Update() {

		if ( Time.time - lastAttackTime > period ) {

			// валидна ли ещё текущая цель
			if (!CheckTarget(currentTarget))
				LoseCurrentTarget();

			if (currentTarget == null)
				FindSomeTarget();

			if (currentTarget == null)
				return;

			// а теперь применение урона

			ApplyDamage();
			lastAttackTime = Time.time;

		}

	}

}
