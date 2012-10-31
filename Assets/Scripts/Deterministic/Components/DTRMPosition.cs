using UnityEngine;
using System.Collections;

public class DTRMPosition : DTRMComponent {

	public DTRMVector2 position = DTRMVector2.zero;
	public float lift = 0;

	public override void DTRMStart() {

		// TODO: change to a proper editor-based initialization!

		position = new DTRMVector2(transform.position.x, transform.position.z);

	}

	public FixedPoint Distance(DTRMPosition other) {

		return (position - other.position).magnitude;

	}

	public FixedPoint SqrDistance(DTRMPosition other) {

		return (position - other.position).sqrMagnitude;

	}

	public override int GetHashCode() {

		unchecked {

			return position.GetHashCode();

		}

	}

	void Update() {

		transform.position = TerrainCoordinates.TerrainToGlobal(position.ToVector2(), 0);

	}

}
