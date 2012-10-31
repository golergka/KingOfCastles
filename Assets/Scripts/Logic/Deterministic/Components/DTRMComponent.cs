using UnityEngine;
using System.Collections;

// [RequireComponent(typeof(DTRMPosition))]

public abstract class DTRMComponent : MonoBehaviour {

	private int _dtrmID;
	private bool _dtrmIDset = false;

	private DTRMPosition _myPosition = null;
	public DTRMPosition myPosition {

		get {

			if (_myPosition == null)
				_myPosition = GetComponent<DTRMPosition>();

			return _myPosition;

		}

	}

	public int dtrmID {

		get {

			if (!_dtrmIDset)
				Debug.LogError("Accessing not determined id: " + ToString());

			return _dtrmID;

		}

		set {

			if (_dtrmIDset) {
				
				Debug.LogError("Setting id that is already set!");

			} else {

				_dtrmIDset = true;
				_dtrmID = value;

			}

		}

	}

	public virtual void DTRMStart() { }

	public virtual void DTRMUpdate() { }

}
