using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface IVisibleListener {

	void OnNoticedBy(Vision observer);
	void OnLostBy(Vision observer);

}

public class Visible : MonoBehaviour {

	// all visions that have me in range. They may actually not see me if I'm invisible
	public List<Vision> inRangeOfVisions = new List<Vision>();

	private bool _visible;
	public bool visible {

		get { return _visible; }
		set {

			if ( _visible == value )
				return;

			_visible = value;

			foreach(Vision vision in inRangeOfVisions)
				vision.ChangedVisibility(this);


		}

	}

	public bool visibleOnStart = true;

	void Start() {

		visible = visibleOnStart;

	}

	void OnDestroy() {

		visible = false;

	}

	void OnDisable() {

		visible = false;

	}

	void OnEnable() {

		visible = true;

	}

}
