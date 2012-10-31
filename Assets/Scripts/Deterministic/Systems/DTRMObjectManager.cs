using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DTRMObjectManager : MonoBehaviour {

	private List<DTRMComponent> components = new List<DTRMComponent>();

	public void Gather() {

		Object[] objects = FindObjectsOfType(typeof(DTRMComponent));

		foreach( Object obj in objects )
			components.Add( (DTRMComponent) obj );

		for (int i = 0; i < components.Count; i++)
			components[i].dtrmID = i;

		foreach (DTRMComponent component in components)
			component.DTRMStart();

	}

	public void SendUpdate() {

		foreach( DTRMComponent component in components )
			if (component.gameObject.active)
				component.DTRMUpdate();

	}

	public override int GetHashCode() {

		unchecked {

			int hash = 17;
			foreach (DTRMComponent component in components)
				hash = hash * 23 + component.GetHashCode();
			return hash;

		}

	}

	public DTRMComponent GetObject(int id) {

		return components[id];

	}

}
