using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DTRM : MonoBehaviour {

	public static DTRM singleton;

	void Start() {

		// get all the objects

		dtrmComponents = new List<DTRMComponent>();
		Object[] objects = FindObjectsOfType(typeof(DTRMComponent));

		foreach( Object obj in objects ) {

			dtrmComponents.Add( (DTRMComponent) obj );

		}

		singleton = this;

		// TODO: sort list by hash


		// assign IDs to objects in order

		int idCounter = 1;
		foreach(DTRMComponent component in dtrmComponents)
			component.dtrmID = idCounter++;

		// call DTRMStart on each of objects
		foreach (DTRMComponent component in dtrmComponents)
			component.DTRMStart();

	}

	//
	// Object management
	//

	List<DTRMComponent> dtrmComponents;

	//
	// Time
	//

	private DTRMLong _dtrmTime = new DTRMLong(0);
	public DTRMLong dtrmTime {

		get { return _dtrmTime; }

	}

	private DTRMLong dtrmPreviousStepTime = new DTRMLong(0);

	public DTRMLong dtrmDeltaTime {

		get { return dtrmTime - dtrmPreviousStepTime; }

	}

	private DTRMLong dtrmStep = new DTRMLong(1);

	private void Step() {

		dtrmPreviousStepTime = dtrmTime;
		_dtrmTime += dtrmStep;
		foreach( DTRMComponent component in dtrmComponents )
			if (component.gameObject.active)
				component.DTRMUpdate();

	}

	public void OnGUI() {

		if ( GUI.Button( new Rect(10, 10, 150, 100), "Step" ) )
			Step();

	}

}
