using UnityEngine;
using System.Collections;

interface IHealthListener {

	void OnTakeDamage(uint damageAmount);
	void OnTakeHealing(uint healingAmount);
	void OnFullHealth();
	void OnZeroHealth();

}

public class Health : MonoBehaviour {

}
