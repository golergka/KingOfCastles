using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Legs))]
[RequireComponent(typeof(Vision))]
[RequireComponent(typeof(Attack))]

public class CreepController : ActorController, IVisionListener, ILegsListener, IAttackListener {

	// ссылки на компоненты
	private Legs legs;
	private Vision vision;
	private Attack attack;

	// Позиция на карте, являющаяся целью крипа. Смысл и использование отличается в зависимости от состояния.
	private DTRMVector2 targetPosition;
	// Позиция на карте, использующаяся для патрулирования. При других приказах кроме Patrol не используется.
	private DTRMVector2 patrolPosition;

	// Цель. Всегда находится в пределах видимости.
	private Health targetVictim;

	private enum CreepOrder {

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

	private CreepOrder activeOrder = CreepOrder.Idle;

	public override void DTRMStart() {

		legs = GetComponent<Legs>();
		vision = GetComponent<Vision>();
		attack = GetComponent<Attack>();

	}

	public override void DTRMUpdate() {

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
				legs.FollowTarget(target.GetComponent<DTRMPosition>());
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

	public void Move(DTRMVector2 position) {

		activeOrder = CreepOrder.Move;
		targetPosition = position;
		legs.targetPosition = targetPosition;
		targetVictim = null;

	}

	public void MoveAttack(DTRMVector2 position) {

		activeOrder = CreepOrder.MoveAttack;
		targetPosition = position;
		targetVictim = null;
		FindTargetOrCarryOn();

	}

	public void Attack(Health target) {

		if (!attack.CheckTarget(target))
			return;

		activeOrder = CreepOrder.Attack;
		targetVictim = target;
		attack.AppointTarget(targetVictim);
		legs.FollowTarget(target.myPosition);

	}

	public void Patrol(Vector2 position) {

		patrolPosition = myPosition.position;
		activeOrder = CreepOrder.Patrol;
		targetVictim = null;
		FindTargetOrCarryOn();

	}

	public void Stop() {

		activeOrder = CreepOrder.Idle;
		legs.Stop();
		targetVictim = null;
		TryFindTarget();

	}

	public void Hold() {

		activeOrder = CreepOrder.Hold;
		targetPosition = myPosition.position;
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
		if (activeOrder == CreepOrder.Move)
			return;

		Health target = observee.GetComponent<Health>();

		if (target == null)
			return;

		if (!attack.CheckTarget(target))
			return;

		attack.AppointTarget(target);
		legs.FollowTarget(target.myPosition);
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

			case CreepOrder.Idle:

				Debug.LogWarning("Unexpected target reach event for state Idle!");
				break;

			case CreepOrder.Hold:
				
				// Вернулись на место, отдыхаем.
				break;

			case CreepOrder.Move:

				activeOrder = CreepOrder.Idle;
				break;

			case CreepOrder.MoveAttack:

				activeOrder = CreepOrder.Idle;
				break;

			case CreepOrder.Attack:

				Debug.LogWarning("Unexpected target reach event for state Attack!");
				break;

			case CreepOrder.Patrol:

				DTRMVector2 exchangePosition = targetPosition;
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

			case CreepOrder.Idle:

				if (!TryFindTarget())
					legs.Stop();

				break;

			case CreepOrder.Hold:

				FindTargetOrCarryOn();
				break;

			case CreepOrder.Move:

				Debug.LogWarning("Unexpected lost appointed target event for move state!");
				break;

			case CreepOrder.MoveAttack:

				FindTargetOrCarryOn();
				break;

			case CreepOrder.Attack:

				activeOrder = CreepOrder.Idle;
				TryFindTarget();

				break;

			case CreepOrder.Patrol:
				
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

}
