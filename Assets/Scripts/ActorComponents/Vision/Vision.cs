using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface IVisionListener {

	void OnNoticed(Visible observee);
	void OnLost(Visible observee);
	
}

public class Vision : DTRMComponent {

	// visibles in sight
	[HideInInspector] public List<Visible> visiblesInSight = new List<Visible>();

	// invisibles in sight
	[HideInInspector] public List<Visible> invisiblesInSight = new List<Visible>();

	// vision settings
	public DTRMLong visionDistance = new DTRMLong(10);

	Component[] visionListeners;

	public override void DTRMStart() {

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

	public override void DTRMUpdate() {

		// TODO : OPTIMIZE!!! EXTREMELY SLOW AND STUPID IMPLEMENTATION

		List<Visible> newVisiblesInSight = new List<Visible>();
		List<Visible> newInvisiblesInSight = new List<Visible>();

		foreach(Visible visible in Visible.allVisibles ) {

			if ( myPosition.Distance(visible.myPosition) < visionDistance ) {

				if (visible.visible) {
					newVisiblesInSight.Add(visible);
				} else {
					newInvisiblesInSight.Add(visible);
				}

			}

		}

		foreach(Visible visible in newVisiblesInSight) { // ugly

			if (visiblesInSight.Contains(visible))
				continue;

			if (invisiblesInSight.Contains(visible))
				continue;

			SendNoticedMessage(visible);
			visible.inRangeOfVisions.Add(this);

		}

		foreach(Visible visible in visiblesInSight) { // very ugly

			if (newVisiblesInSight.Contains(visible))
				continue;

			if (newInvisiblesInSight.Contains(visible))
				continue;

			SendLostMessage(visible);
			visible.inRangeOfVisions.Remove(this);

		}

		foreach(Visible visible in invisiblesInSight) { // EXTREMELY ugly. OK. But gets job done

			if (newVisiblesInSight.Contains(visible))
				continue;

			if (newInvisiblesInSight.Contains(visible))
				continue;

			SendLostMessage(visible);
			visible.inRangeOfVisions.Remove(this);

		}

		visiblesInSight = newVisiblesInSight;
		invisiblesInSight = newInvisiblesInSight;

	}

	void OnDrawGizmos() {

		Gizmos.color = Color.red;
		foreach(Visible visible in invisiblesInSight)
			Gizmos.DrawLine(transform.position, visible.transform.position);

		Gizmos.color = Color.green;
		foreach(Visible visible in visiblesInSight)
			Gizmos.DrawLine(transform.position, visible.transform.position);

	}

}
