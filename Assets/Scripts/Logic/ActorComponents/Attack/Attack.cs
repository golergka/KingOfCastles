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
public abstract class Attack : DTRMComponent {

	protected Health currentTarget; // текущая цель
	protected bool appointedTarget = false; // была ли текущая цель кем-то указана специально

	public int damage;

	private Component[] attackListeners;

	public override void DTRMStart() {

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

	public override int GetHashCode() {

		unchecked {

			int hash = 17;
			hash = hash * 23 + damage.GetHashCode();
			if (currentTarget != null)
				hash = hash * 23 + currentTarget.GetHashCode();
			hash = hash * 23 + appointedTarget.GetHashCode();
			return hash;

		}

	}

}
