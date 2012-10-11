using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface IAutoAttackListener {

	// отсылается только когда AutoAttack теряет цель, выбранную специально
	void OnLooseApointedTarget();

}

[RequireComponent(typeof(Vision))]

public class AutoAttack : MonoBehaviour, IVisionListener {

	private Component[] autoAttackListeners;

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
	public uint damage; // количество урона
	public float period; // период атаки

	private float lastAttackTime = 0f;

	private Health currentTarget; // текущая цель
	private bool appointedTarget = false; // была ли текущая цель кем-то указана специально

	private Vision vision;

	void Start() {

		_attackRange = initialAttackRange;
		autoAttackListeners = GetComponents(typeof(IAutoAttackListener));
		vision = GetComponent<Vision>();

	}

	public void OnNoticed(Visible observee) {

		// если у нас уже есть цель, пофигу
		if ( currentTarget != null )
			return;

		// если это не враг нам, пофигу
		if ( !FoFSystem.AreEnemies(this, observee) )
			return;

		// если на нём нет здоровья, которое можно атаковать, пофигу
		Health target = observee.GetComponent<Health>();
		if (target == null)
			return;

		// если подходит, то атакуем
		if ( CheckTarget(currentTarget) )
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

	// потерять текущую цель
	private void LoseCurrentTarget() {

		if (appointedTarget) { // если нам её назначили, то сбросить этот флаг и отправить сообщение

			appointedTarget = false;
			foreach(Component listener in autoAttackListeners)
				((IAutoAttackListener)listener).OnLooseApointedTarget();

		}

		currentTarget = null;

	}

	// true если цель можно сейчас атаковать
	public bool CheckTarget( Health target ) {

		if ( target == null ) {

			Debug.LogWarning("Received null target!");
			return false;

		}

		return ( transform.position - target.transform.position ).magnitude < attackRange;

	}

	// назначить цель снаружи
	public void AppointTarget( Health target ) {

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

			currentTarget.InflictDamage(damage);
			lastAttackTime = Time.time;

		}

	}

}
