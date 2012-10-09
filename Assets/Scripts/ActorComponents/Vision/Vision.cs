using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface IVisionListener {

	void OnNoticed(Visible observee);
	void OnLost(Visible observee);
	
}

public class Vision : MonoBehaviour {

	// visibles in sight
	[HideInInspector] public List<Visible> visiblesInSight = new List<Visible>();

	// invisibles in sight
	[HideInInspector] public List<Visible> invisiblesInSight = new List<Visible>();

	// vision settings
	public float visionDistance = 5f;

	Component[] visionListeners;

	void Start() {

		GameObject triggerObject = new GameObject("visionTrigger");
		triggerObject.AddComponent<VisionDelegate>();
		visionListeners = GetComponents(typeof(IVisionListener));

	}

	private void SendNoticedMessage(Visible observee) {

		foreach(Component listener in visionListeners)
			( (IVisionListener)listener ).OnNoticed(observee);

	}

	private void SendLostMessage(Visible observee) {

		foreach(Component listener in visionListeners)
			( (IVisionListener)listener ).OnLost(observee);

	}

	public void ColliderEnter(Collider other) {

		Visible otherVisible = other.GetComponent<Visible>();

		if (otherVisible == null)
			return;

		otherVisible.inRangeOfVisions.Add(this);

		if (otherVisible.visible) {

			visiblesInSight.Add(otherVisible);
			SendNoticedMessage(otherVisible);

		} else {

			invisiblesInSight.Add(otherVisible);

		}

	}

	public void ColliderExit(Collider other) {

		Visible otherVisible = other.GetComponent<Visible>();

		if (otherVisible == null)
			return;

		otherVisible.inRangeOfVisions.Remove(this);

		if (otherVisible.visible) {

			visiblesInSight.Remove(otherVisible);
			SendLostMessage(otherVisible);

		} else {

			invisiblesInSight.Remove(otherVisible);

		}

	}

	public void ChangedVisibility(Visible visible) {

		if (visible.visible) {

			invisiblesInSight.Remove(visible);
			visiblesInSight.Add(visible);
			SendNoticedMessage(visible);

		} else {

			visiblesInSight.Remove(visible);
			invisiblesInSight.Add(visible);
			SendLostMessage(visible);

		}

	}

}
