using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour {

	private List<DTRMComponent> components = new List<DTRMComponent>();
	private DTRMComponent[] componentCache;
	
	public static ObjectManager singleton;
	
	public void Start() {
		
		singleton = this;
		
	}

	public void Gather() {

		Object[] objects = FindObjectsOfType(typeof(DTRMComponent));

		foreach( Object obj in objects )
			components.Add( (DTRMComponent) obj );

		for (int i = 0; i < components.Count; i++)
			components[i].dtrmID = i;
		
		componentCache = components.ToArray();
		
		foreach (DTRMComponent component in componentCache)
			component.DTRMStart();
		
		

	}
	
	public void InitObject(GameObject gObject) {
		
		foreach(DTRMComponent component in gObject.GetComponents<DTRMComponent>() )
			InitComponent(component);
		
	}
	
	public void InitComponent(DTRMComponent newObject) {
		
		newObject.dtrmID = components.Count;
		components.Add (newObject);
		componentCache = components.ToArray();
		newObject.DTRMStart();
		
	}

	public void SendUpdate() {

		foreach( DTRMComponent component in componentCache )
			if (component.gameObject.active)
				component.DTRMUpdate();

	}

	public override int GetHashCode() {

		unchecked {

			int hash = 17;
			foreach (DTRMComponent component in componentCache)
				hash = hash * 23 + component.GetHashCode();
			return hash;

		}

	}

	public DTRMComponent GetObject(int id) {

		return componentCache[id];

	}

}
