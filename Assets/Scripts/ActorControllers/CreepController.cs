using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Legs))]
[RequireComponent(typeof(Vision))]
[RequireComponent(typeof(Attack))]

public class CreepOrder : Order {

	public enum CreepOrderType {

		Stop,
		Hold,
		Move,
		MoveAttack,
		Attack,
		Patrol,

	}

	public CreepOrderType creepOrder;
	public DTRMVector2 position;
	public Health target;

	public CreepOrder(int destinationID, CreepOrderType orderType, DTRMVector2 position, Health target) : base (destinationID) {

		creepOrder = orderType;

		if (orderType == CreepOrderType.Move ||
			orderType == CreepOrderType.MoveAttack ||
			orderType == CreepOrderType.Patrol )
			this.position = position;

		if (orderType == CreepOrderType.Attack)
			this.target = target;

	}

}

public class CreepController : ActorController, IVisionListener, ILegsListener, IAttackListener, IOrderReceiver {

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

	private enum CreepState {

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

	private CreepState activeOrder = CreepState.Idle;

	public override void DTRMStart() {

		base.DTRMStart();

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

		// Debug.Log("Trying to find new target!");
		List<Visible> potentialTargets = vision.VisiblesInSight();
		// Debug.Log(potentialTargets.ToString());

		foreach( Visible potentialTarget in potentialTargets ) {

			if (potentialTarget == null)
				continue;

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

	private void Move(DTRMVector2 position) {

		activeOrder = CreepState.Move;
		targetPosition = position;
		legs.targetPosition = targetPosition;
		targetVictim = null;

	}

	private void MoveAttack(DTRMVector2 position) {

		activeOrder = CreepState.MoveAttack;
		targetPosition = position;
		targetVictim = null;
		FindTargetOrCarryOn();

	}

	private void Attack(Health target) {

		if (!attack.CheckTarget(target))
			return;

		activeOrder = CreepState.Attack;
		targetVictim = target;
		attack.AppointTarget(targetVictim);
		legs.FollowTarget(target.myPosition);

	}

	private void Patrol(DTRMVector2 position) {

		targetPosition = position;
		patrolPosition = myPosition.position;
		activeOrder = CreepState.Patrol;
		targetVictim = null;
		FindTargetOrCarryOn();

	}

	private void Stop() {

		activeOrder = CreepState.Idle;
		legs.Stop();
		targetVictim = null;
		TryFindTarget();

	}

	private void Hold() {

		activeOrder = CreepState.Hold;
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
		if (activeOrder == CreepState.Move)
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

			case CreepState.Idle:

				Debug.LogWarning("Unexpected target reach event for state Idle!");
				break;

			case CreepState.Hold:
				
				// Вернулись на место, отдыхаем.
				break;

			case CreepState.Move:

				activeOrder = CreepState.Idle;
				break;

			case CreepState.MoveAttack:

				activeOrder = CreepState.Idle;
				break;

			case CreepState.Attack:

				Debug.LogWarning("Unexpected target reach event for state Attack!");
				break;

			case CreepState.Patrol:

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

			case CreepState.Idle:

				if (!TryFindTarget())
					legs.Stop();

				break;

			case CreepState.Hold:

				FindTargetOrCarryOn();
				break;

			case CreepState.Move:

				Debug.LogWarning("Unexpected lost appointed target event for move state!");
				break;

			case CreepState.MoveAttack:

				FindTargetOrCarryOn();
				break;

			case CreepState.Attack:

				activeOrder = CreepState.Idle;
				TryFindTarget();

				break;

			case CreepState.Patrol:
				
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
	// IOrderReceiver
	//

	public void ReceiveOrder(Order order) {

		Debug.LogError("Wrong order type!");

	}

	public void ReceiveOrder(CreepOrder order) {

		switch (order.creepOrder) {

			case CreepOrder.CreepOrderType.Stop:
				Stop();
				break;

			case CreepOrder.CreepOrderType.Hold:
				Hold();
				break;

			case CreepOrder.CreepOrderType.Move:
				Move(order.position);
				break;

			case CreepOrder.CreepOrderType.MoveAttack:
				MoveAttack(order.position);
				break;

			case CreepOrder.CreepOrderType.Attack:
				Attack(order.target);
				break;

			case CreepOrder.CreepOrderType.Patrol:
				Patrol(order.position);
				break;

			default:
				Debug.LogError("Unknown order!");
				break;

		}

	}

	//
	//
	//

	private void OnDrawGizmos() {

		if (targetVictim == null)
			return;

		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, targetVictim.transform.position);

	}

}
