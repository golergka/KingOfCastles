using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (SphereCollider))]

public class VisionDelegate : MonoBehaviour {

	// collider radius
	public float radius {

		get {

			return sphereCollider.radius;

		}

		set {

			sphereCollider.radius = value;

		}

	}

	private SphereCollider sphereCollider;
	private Vision vision;
	private Collider parentCollider;

	void Start() {

		sphereCollider = GetComponent<SphereCollider>();
		sphereCollider.isTrigger = true;
		
		vision = transform.parent.GetComponent<Vision>();
		if (vision == null) {
			
			gameObject.active = false;
			Debug.LogWarning("Couldn't find parent's vision.");

		} else {
		
			this.radius = vision.visionDistance;

		}

		parentCollider = transform.parent.collider;

	}

	void OnTriggerEnter(Collider other) {

		if (other != parentCollider)
			vision.ColliderEnter(other);

	}

	void OnTriggerExit(Collider other) {

		if (other != parentCollider)
			vision.ColliderExit(other);

	}

}