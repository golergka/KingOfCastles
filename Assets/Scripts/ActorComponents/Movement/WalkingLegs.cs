using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]

public class WalkingLegs : Legs {

	public float speed = 1f;
	public float closeTargetDistance = 1f;

	public float angularRotate = 0.02f;
	public float magnitudeRotate = 5f;

	// Use this for initialization
	override protected void Start () {

		base.Start();
	
	}
	
	void FixedUpdate() {
		
		CheckTargetReach();

		if (!moving)
			return; // могло измеинться при проверке

		Vector3 desiredVelocity = (TerrainCoordinates.TerrainToGlobal(target) - rigidbody.position).normalized * speed;
		Vector3 velocityChange = desiredVelocity - rigidbody.velocity;
		rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

	}

	protected override void StopMovement() {

		rigidbody.AddForce(-rigidbody.velocity, ForceMode.VelocityChange);

	}

}
