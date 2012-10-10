using UnityEngine;
using System.Collections;

interface IHealthChangeListener {

	void OnTakeDamage(uint damageAmount);
	void OnTakeHealing(uint healingAmount);

}

interface IHealthStateListener {

	void OnFullHealth();
	void OnZeroHealth();

}

public class Health : MonoBehaviour {

	private uint _healthPoints;
	public uint healthPoints {

		get { return _healthPoints; }

		set {

			if (value < _healthPoints) {

				InflictDamage( _healthPoints - value );

			} else if (value > _healthPoints) {

				InflictHealing( value - _healthPoints );

			}

		}

	}

	private uint _maxHealhPoints;
	public uint maxHealthPoints {

		get { return _maxHealhPoints; }

		set {

			// микро-оптимизация и одновременно способ не посылать сообщения о том что hp = max в такой ситуации
			if (maxHealthPoints == value)
				return;

			_maxHealhPoints = value;

			if ( _healthPoints >= _maxHealhPoints ) {
				
				_healthPoints = _maxHealhPoints;
				foreach(Component listener in healthStateListeners)
					((IHealthStateListener)listener).OnFullHealth();

			}

		}
	}

	private Component[] healthChangeListeners;
	private Component[] healthStateListeners;

	void Start() {

		healthChangeListeners = GetComponents(typeof(IHealthChangeListener));
		healthStateListeners = GetComponents(typeof(IHealthStateListener));

	}

	public void InflictDamage(uint damageAmount) {

		if ( damageAmount == 0 ) {
			Debug.LogWarning("Received 0 damage!");
			return;
		}

		if ( damageAmount >= _healthPoints ) {

			_healthPoints = 0;
			foreach(Component listener in healthStateListeners)
				((IHealthStateListener)listener).OnZeroHealth();

		}

		foreach(Component listener in healthChangeListeners)
			((IHealthChangeListener)listener).OnTakeDamage(damageAmount);

	}

	public void InflictHealing(uint healingAmount) {

		if ( healingAmount == 0 ) {

			Debug.LogWarning("Received 0 healing!");
			return;

		}

		if ( healingAmount >= _maxHealhPoints - _healthPoints ) {

			_healthPoints = _maxHealhPoints;
			foreach(Component listener in healthStateListeners)
				((IHealthStateListener)listener).OnFullHealth();

		}

		foreach(Component listener in healthChangeListeners)
			((IHealthChangeListener)listener).OnTakeHealing(healingAmount);

	}

}
