using UnityEngine;
using System.Collections;

interface IVisibleListener {

	void OnNoticedBy(Vision observer);
	void OnLostBy(Vision observer);

}

public class Visible : MonoBehaviour {

}
