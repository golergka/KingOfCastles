using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface IAutoAttackListener {

	// отсылается только когда AutoAttack теряет цель, выбранную специально
	void OnLooseApointedTarget();

}

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

	Health currentTarget; // текущая цель
	bool appointedTarget = false; // была ли текущая цель кем-то указана специально

	void Start() {

		_attackRange = initialAttackRange;
		autoAttackListeners = GetComponents(typeof(IAutoAttackListener));

	}

	private List<Health> potentialTargets = new List<Health>();

	public void OnNoticed(Visible observee) {

		// проверка свой-чужой
		if (observee.CompareTag(tag))
			return;

		// проверка на наличие здоровья, которое можно атаковать
		Health target = observee.GetComponent<Health>();
		if (target != null)
			potentialTargets.Add(target);

		// если у нас нет текущей цели и эта подходит, то атаковать
		if ( currentTarget == null && CheckTarget(currentTarget) )
			currentTarget = target;

	}

	// найти какую-нибудь подходящую цель из потенциальных
	private void FindSomeTarget() {

		foreach(Health target in potentialTargets) {

			if (CheckTargetRange(target)) {
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

	public void OnLost(Visible observee) {

		if (observee.CompareTag(tag))
			return;

		Health target = observee.GetComponent<Health>();
		if (target != null)
			potentialTargets.Remove(target);

		if ( target == currentTarget )
			LoseCurrentTarget();

	}

	// для использования на членах списка potentialTargets или currentTarget
	private bool CheckTargetRange( Health target ) {

		return ( transform.position - target.transform.position).magnitude < attackRange;

	}

	// true если цель можно сейчас атаковать
	// для использования снаружи, можно использовать на любой Health
	public bool CheckTarget( Health target ) {

		return
			(

				potentialTargets.Contains(target)
				&& 
				CheckTargetRange(target)

			);

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
			if (!CheckTargetRange(currentTarget))
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
