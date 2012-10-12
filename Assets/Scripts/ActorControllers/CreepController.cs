using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Legs))]
[RequireComponent(typeof(Vision))]
[RequireComponent(typeof(Attack))]

public class CreepController : ActorController, IVisionListener, ILegsListener, IAttackListener, IHealthStateListener {

	// ссылки на компоненты
	private Legs legs;
	private Vision vision;
	private Attack attack;

	// Позиция на карте, являющаяся целью крипа. Смысл и использование отличается в зависимости от состояния.
	private Vector2 targetPosition;
	// Позиция на карте, использующаяся для патрулирования. При других приказах кроме Patrol не используется.
	private Vector2 patrolPosition;

	// Цель. Всегда находится в пределах видимости.
	private Health targetVictim;

	private enum ActiveOrder {

		Idle,
		/*
		 * Нет активного приказа, юнит никого не атакует.
		 * При появлении противника атакует и преследует.
		 * При исчезновении противника стоит на месте.
		 * targetPosition не используется.
		 */

		Hold,
		/*
		 * Держит позицию.
		 * При появлении противника атакует и преследует.
		 * При исчезновении противника возвращается на targetPosition.
		 */

		Move,
		/*
		 * Перемещается к цели targetPosition.
		 * Не реагирует на противников, targetVictim пустой.
		 * Когда приходит к цели, переходит в режим Idle.
		 */

		MoveAttack,
		/*
		 * Перемещается к цели targetPosition.
		 * При появлении противника атакует и преследует.
		 * При исчезновении противника продолжает движение к цели.
		 * При достижении цели переходит в Idle.
		 */

		Attack,
		/*
		 * Атакует указанного противника.
		 * При исчезновении противника переходит в режим Idle.
		 */

		Patrol,
		/*
		 * При включении приказа записывает текущее местоположение в patrolPosition.
		 * Передвигается к цели targetPosition.
		 * При появлении противника атакует и преследует.
		 * При исчезновении противника продолжает движение к цели.
		 * При достижении цели меняет местами patrolPosition и targetPosition и продолжает патрулиирование.
		 */

	};

	private ActiveOrder activeOrder = ActiveOrder.Idle;

	void Start() {

		legs = GetComponent<Legs>();
		vision = GetComponent<Vision>();
		attack = GetComponent<Attack>();

	}

	//
	// Внутренние функции
	//

	// Пытается найти цель из видимых.
	// Если находит, то идет к ней и атакует, но не меняет активный приказ.
	private bool TryFindTarget() {

		foreach( Visible potentialTarget in vision.visiblesInSight ) {

			Health target = potentialTarget.GetComponent<Health>();
			if (target == null)
				continue;

			if (attack.CheckTarget(target)) {

				attack.AppointTarget(target);
				legs.FollowTarget(target.rigidbody);
				return true;

			}

		}

		return false;

	}

	private void FindTargetOrCarryOn() {

		if ( !TryFindTarget() )
			legs.targetPosition = targetPosition;

	}

	//
	// Приказы
	//

	public void Move(Vector2 position) {

		activeOrder = ActiveOrder.Move;
		targetPosition = position;
		legs.targetPosition = targetPosition;
		targetVictim = null;

	}

	public void MoveAttack(Vector2 position) {

		activeOrder = ActiveOrder.MoveAttack;
		targetPosition = position;
		targetVictim = null;
		FindTargetOrCarryOn();

	}

	public void Attack(Health target) {

		if (!attack.CheckTarget(target))
			return;

		activeOrder = ActiveOrder.Attack;
		targetVictim = target;
		attack.AppointTarget(targetVictim);
		legs.FollowTarget(target.rigidbody);

	}

	public void Patrol(Vector2 position) {

		patrolPosition = TerrainCoordinates.GlobalToTerrain(rigidbody.position);
		activeOrder = ActiveOrder.Patrol;
		targetVictim = null;
		FindTargetOrCarryOn();

	}

	public void Stop() {

		activeOrder = ActiveOrder.Idle;
		legs.Stop();
		targetVictim = null;
		TryFindTarget();

	}

	public void Hold() {

		activeOrder = ActiveOrder.Hold;
		targetPosition = TerrainCoordinates.GlobalToTerrain(rigidbody.position);
		targetVictim = null;
		TryFindTarget();

	}

	//
	// IVisionListener
	//

	public void OnNoticed(Visible observee) {

		// если у нас уже есть текущая цель, то похуй кто там пришёл
		if (targetVictim != null)
			return;

		// если мы двигаемся не замечая никого, нам похуй
		if (activeOrder == ActiveOrder.Move)
			return;

		Health target = observee.GetComponent<Health>();

		if (target == null)
			return;

		if (!attack.CheckTarget(target))
			return;

		attack.AppointTarget(target);
		legs.FollowTarget(target.rigidbody);
		targetVictim = target;

	}

	public void OnLost(Visible observee) {

		// Нам похуй.
		// Если мы потеряли нашу текущую цель, то мы всё равно узнаем об этом от компонента Attack.

	}

	//
	// ILegsListener
	//

	public void OnTargetReach() {

		switch(activeOrder) {

			case ActiveOrder.Idle:

				Debug.LogWarning("Unexpected target reach event for state Idle!");
				break;

			case ActiveOrder.Hold:
				
				// Вернулись на место, отдыхаем.
				break;

			case ActiveOrder.Move:

				activeOrder = ActiveOrder.Idle;
				break;

			case ActiveOrder.MoveAttack:

				activeOrder = ActiveOrder.Idle;
				break;

			case ActiveOrder.Attack:

				Debug.LogWarning("Unexpected target reach event for state Attack!");
				break;

			case ActiveOrder.Patrol:

				Vector2 exchangePosition = targetPosition;
				targetPosition = patrolPosition;
				patrolPosition = exchangePosition;
				legs.targetPosition = targetPosition;
				break;

			default:
				Debug.LogError("Unknown order!");
				break;

		}

	}

	public void OnTargetUnreachable() {

		Debug.LogError("Implement OnTargetUnreachable event handler!");

	}

	//
	// IAttackListener
	//

	public void OnLostAppointedTarget() {

		targetVictim = null;

		switch(activeOrder) {

			case ActiveOrder.Idle:

				if (!TryFindTarget())
					legs.Stop();

				break;

			case ActiveOrder.Hold:

				FindTargetOrCarryOn();
				break;

			case ActiveOrder.Move:

				Debug.LogWarning("Unexpected lost appointed target event for move state!");
				break;

			case ActiveOrder.MoveAttack:

				FindTargetOrCarryOn();
				break;

			case ActiveOrder.Attack:

				activeOrder = ActiveOrder.Idle;
				TryFindTarget();

				break;

			case ActiveOrder.Patrol:
				
				FindTargetOrCarryOn();
				break;


			default:
				Debug.LogError("Unknown order!");
				break;


		}

	}

	public void OnApplyDamage() {

		// Нам похуй.

	}

	//
	// IHealthStateListener
	//

	public void OnFullHealth() {

		// Нам похуй снова.

	}

	public void OnZeroHealth() {

		gameObject.active = false;

	}

}
