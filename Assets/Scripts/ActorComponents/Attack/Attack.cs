using UnityEngine;
using System.Collections;

interface IAttackListener {

	// отсылается только когда AutoAttack теряет цель, выбранную специально
	void OnLostAppointedTarget();
	void OnApplyDamage();

}

// подклассы меняют алгоритм нахождения цели, но не расчёт урона
// работа с дальностью атаки ведётся в под-классах
// работа с периодом урона ведётся в под-классах
public abstract class Attack : MonoBehaviour {

	protected Health currentTarget; // текущая цель
	protected bool appointedTarget = false; // была ли текущая цель кем-то указана специально

	public int damage;

	private Component[] attackListeners;

	protected virtual void Start() {

		attackListeners = GetComponents(typeof(IAttackListener));

	}

	protected void SendLostAppointedTargetMessage() {

		foreach(Component listener in attackListeners)
				((IAttackListener)listener).OnLostAppointedTarget();

	}

	public abstract void AppointTarget( Health target );

	public abstract void DropTarget();

	protected void ApplyDamage() {

		currentTarget.InflictDamage( damage );
		foreach(Component listener in attackListeners)
				((IAttackListener)listener).OnApplyDamage();


	}

	public virtual bool CheckTarget( Health target ) {

		if ( target == null ) {

			Debug.LogWarning("Received null target!");
			return false;

		}

		return ( FoFSystem.AreEnemies(this, target) );

	}

}
