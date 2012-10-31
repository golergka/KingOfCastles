using UnityEngine;
using System.Collections;

interface IHealthChangeListener {

	void OnTakeDamage(int damageAmount);
	void OnTakeHealing(int healingAmount);

}

interface IHealthStateListener {

	void OnFullHealth();
	void OnZeroHealth();

}

public class Health : DTRMComponent {

	private int _healthPoints;
	public int healthPoints {

		get { return _healthPoints; }

		set {

			if (value < _healthPoints) {

				InflictDamage( _healthPoints - value );

			} else if (value > _healthPoints) {

				InflictHealing( value - _healthPoints );

			}

		}

	}

	private int _maxHealhPoints;
	public int maxHealthPoints {

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

	public int initialHealthPoints = 100;
	public int initialMaxHealthPoints = 100;

	// Temporary off
	// private Component[] healthChangeListeners;
	private Component[] healthStateListeners;

	public override void DTRMStart() {

		_healthPoints = initialHealthPoints;
		_maxHealhPoints = initialMaxHealthPoints;

		// Temporary off as no one uses that
		// healthChangeListeners = GetComponents(typeof(IHealthChangeListener));
		healthStateListeners = GetComponents(typeof(IHealthStateListener));

	}

	public void InflictDamage(int damageAmount) {

		if ( damageAmount == 0 ) {

			Debug.LogWarning("Received 0 damage!");
			return;
			
		}

		bool zeroHealth = false;

		if ( damageAmount >= _healthPoints ) {

			_healthPoints = 0;
			zeroHealth = true;

		} else {

			_healthPoints -= damageAmount;

		}

		// Temporary off as nobody uses that
		// foreach(Component listener in healthChangeListeners)
			// ((IHealthChangeListener)listener).OnTakeDamage(damageAmount);

		if (zeroHealth)
			foreach(Component listener in healthStateListeners)
				((IHealthStateListener)listener).OnZeroHealth();

	}

	public void InflictHealing(int healingAmount) {

		if ( healingAmount == 0 ) {

			Debug.LogWarning("Received 0 healing!");
			return;

		}

		if ( healingAmount >= _maxHealhPoints - _healthPoints ) {

			_healthPoints = _maxHealhPoints;
			foreach(Component listener in healthStateListeners)
				((IHealthStateListener)listener).OnFullHealth();

		}

		// foreach(Component listener in healthChangeListeners)
			// ((IHealthChangeListener)listener).OnTakeHealing(healingAmount);

	}

	private void OnDrawGizmos() {

		Gizmos.color = Color.Lerp( Color.red, Color.green, ( (float) _healthPoints / (float) _maxHealhPoints ) );
		Vector3 uplift = new Vector3(0f, 2f, 0f);
		Gizmos.DrawSphere( transform.position + uplift, 0.3F );

	}

	public override int GetHashCode() {

		unchecked {

			int hash = 17;
			hash = hash * 23 + _healthPoints.GetHashCode();
			hash = hash * 23 + _maxHealhPoints.GetHashCode();
			return hash;

		}

	}

	public override void DTRMUpdate() {
		
	}

}
