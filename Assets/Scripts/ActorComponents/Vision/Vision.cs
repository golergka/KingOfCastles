using UnityEngine;
using System.Collections;

interface IVisionListener {

	void OnNoticed(Visible observee);
	void OnLost(Visible observee);
	
}

public class Vision : MonoBehaviour {

}
