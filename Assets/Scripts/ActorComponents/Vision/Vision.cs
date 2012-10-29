using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface IVisionListener {

	void OnNoticed(Visible observee);
	void OnLost(Visible observee);
	
}

public class Vision : DTRMComponent {

	// visibles in sight
	// [HideInInspector] private List<Visible> visiblesInSight = new List<Visible>();
	private const int VISION_LIMIT = 100;
	[HideInInspector] private Visible[] visiblesInSight   = new Visible[VISION_LIMIT];
	[HideInInspector] private Visible[] invisiblesInSight = new Visible[VISION_LIMIT];

	public List<Visible> VisiblesInSight() {

		List<Visible> result = new List<Visible>();
		foreach(Visible v in visiblesInSight)
			if (v != null)
				result.Add(v);

		return result;

	}

	// // invisibles in sight
	// [HideInInspector] private List<Visible> invisiblesInSight = new List<Visible>();

	// vision settings
	private DTRMLong sqrVisionDistance = new DTRMLong(100);
	public DTRMLong visionDistance {

		get { return sqrVisionDistance.sqrt; }
		set { sqrVisionDistance = value*value; }

	}

	Component[] visionListeners;

	public override void DTRMStart() {

		visionListeners = GetComponents(typeof(IVisionListener));

	}

	//
	// Messaging
	//

	private void SendNoticedMessage(Visible observee) {

		foreach(Component listener in visionListeners)
			( (IVisionListener)listener ).OnNoticed(observee);

	}

	private void SendLostMessage(Visible observee) {

		foreach(Component listener in visionListeners)
			( (IVisionListener)listener ).OnLost(observee);

	}

	//
	// Array management
	//

	private void AddVisible(Visible visible) {

		for(int i = 0; i<VISION_LIMIT; i++) {
		
			if (visiblesInSight[i] == null) {
				visiblesInSight[i] = visible;
				return;
			}

		}
		
		Debug.LogError("No place for new visible!");

	}

	private void RemoveVisible(Visible visible) {

		for(int i = 0; i<VISION_LIMIT; i++) {

			if (visiblesInSight[i] == visible) {
				visiblesInSight[i] = null;
				return;
			}

		}

		Debug.LogError("Visible to remove not found!");

	}

	private void AddInvisible(Visible visible) {

		for(int i = 0; i<VISION_LIMIT; i++) {
			
			if (invisiblesInSight[i] == null) {
				invisiblesInSight[i] = visible;
				return;
			}

		}

		Debug.LogError("No place for new visible!");

	}

	private void RemoveInvisible(Visible visible) {

		for(int i = 0; i<VISION_LIMIT; i++) {

			if (invisiblesInSight[i] == visible) {
				invisiblesInSight[i] = null;
				return;
			}

		}

		Debug.LogError("Visible to remove not found!");

	}

	public void ChangedVisibility(Visible visible) {

		if (visible.visible) {

			RemoveInvisible(visible);
			AddVisible(visible);
			SendNoticedMessage(visible);

		} else {

			RemoveVisible(visible);
			AddInvisible(visible);
			SendLostMessage(visible);

		}

	}

	public override void DTRMUpdate() {

		List<Visible> neighbors = VisibleGrid.GetNeighbors(myPosition.position);

		// checking visibles in sight
		for(int i=0; i<VISION_LIMIT; i++) {
			
			Visible visible = visiblesInSight[i];

			if (visible == null)
				continue;

			if (myPosition.SqrDistance(visible.myPosition) > sqrVisionDistance ) {

				visiblesInSight[i] = null;
				visible.inRangeOfVisions.Remove(this);
				if (visible.visible)
					SendLostMessage(visible);

			}

			if (neighbors.Contains(visible))
				neighbors.Remove(visible);

		}

		// checking invisibles in sight
		for(int i=0; i<VISION_LIMIT; i++) {
			
			Visible visible = invisiblesInSight[i];

			if (visible == null)
				continue;

			if (myPosition.SqrDistance(visible.myPosition) > sqrVisionDistance ) {

				visible.inRangeOfVisions.Remove(this);
				invisiblesInSight[i] = null;

			}

			if (neighbors.Contains(visible))
				neighbors.Remove(visible);

		}

		// checking the rest of the neighbours
		foreach(Visible visible in neighbors) {

			if (myPosition.SqrDistance(visible.myPosition) > sqrVisionDistance)
				continue;

			visible.inRangeOfVisions.Add(this);

			if (visible.visible) {

				AddVisible(visible);
				SendNoticedMessage(visible);

			} else {

				AddInvisible(visible);

			}

		}

	}

	void OnDrawGizmos() {

		Gizmos.color = Color.red;
		foreach(Visible visible in invisiblesInSight)
			if (visible != null)
				Gizmos.DrawLine(transform.position, visible.transform.position);

		Gizmos.color = Color.green;
		foreach(Visible visible in visiblesInSight)
			if (visible != null)
				Gizmos.DrawLine(transform.position, visible.transform.position);

	}

}
