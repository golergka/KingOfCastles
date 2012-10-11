using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Legs))]
[RequireComponent(typeof(Vision))]

public class CreepController : ActorController, IVisionListener {

	// ссылки на компоненты
	private Legs legs;
	private Vision vision;

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

	void Start() {

		legs = GetComponent<Legs>();
		vision = GetComponent<Vision>();

	}

	//
	// Приказы
	//

	public void Move(Vector2 position) {

	}

	public void MoveAttack(Vector2 position) {

	}

	public void Attack(Health target) {

	}

	public void Patrol(Vector2 position) {

	}

	public void Stop() {

	}

	public void Hold() {

	}

	// реализация IVisionListener

	public void OnNoticed(Visible observee) {

	}

	public void OnLost(Visible observee) {
		
	}

}
