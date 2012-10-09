using UnityEngine;
using System.Collections;

interface ILegsListener {

	void OnTargetReach();
	void OnTargetUnreachable();

}

public abstract class Legs : MonoBehaviour {

}
