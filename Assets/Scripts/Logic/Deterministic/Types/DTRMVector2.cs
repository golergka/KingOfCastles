using UnityEngine;
using System.Collections;

public class DTRMVector2 {

	public FixedPoint x;
	public FixedPoint y;

	public FixedPoint sqrMagnitude {

		get {

			return ( x*x + y*y );

		}

	}

	public FixedPoint magnitude {

		get {

			return sqrMagnitude.sqrt;

		}

	}

	public DTRMVector2 normalized {

		// TODO — optimize!

		get {

			return ( this / this.magnitude );

		}

	}

	public static DTRMVector2 zero {

		get {

			return new DTRMVector2(0,0);

		}

	}
	
	#region Constructors
	
	public DTRMVector2(FixedPoint x, FixedPoint y) {

		this.x = x;
		this.y = y;

	}

	public DTRMVector2(int x = 0, int y = 0) {

		this.x = new FixedPoint(x);
		this.y = new FixedPoint(y);

	}

	public DTRMVector2(float x, float y) {

		this.x = new FixedPoint(x);
		this.y = new FixedPoint(y);

	}

	public DTRMVector2(Vector2 vector) {

		this.x = new FixedPoint(vector.x);
		this.y = new FixedPoint(vector.y);

	}
	
	public DTRMVector2(Vector3 vector3) {
		
		this.x = new FixedPoint(vector3.x);
		this.y = new FixedPoint(vector3.y);
		
	}
	
	#endregion

	public Vector2 ToVector2 () {

		Vector2 result = Vector2.zero;
		result.x = x.ToFloat();
		result.y = y.ToFloat();
		return result;

	}
	
	public Vector3 ToVector3() {
		
		Vector3 result = Vector3.zero;
		result.x = x.ToFloat();
		result.y = y.ToFloat();
		return result;
		
	}

	//
	// Object overrides
	//

	public override bool Equals(object o) {

		if (o == null)
			return false;

		DTRMVector2 dtrmVector2 = (DTRMVector2) o;

		if (dtrmVector2 == null)
			return false;

		return (this == dtrmVector2);

	}

	public override int GetHashCode() {

		unchecked {

			int hash = 17;
			hash = hash * 23 + x.GetHashCode();
			hash = hash * 23 + y.GetHashCode();
			return hash;

		}

	}

	//
	// Operators
	//

	public static DTRMVector2 operator +(DTRMVector2 a, DTRMVector2 b) {

		return new DTRMVector2(a.x + b.x, a.y + b.y);

	}

	public static DTRMVector2 operator -(DTRMVector2 a, DTRMVector2 b) {

		return new DTRMVector2(a.x - b.x, a.y - b.y);

	}

	public static DTRMVector2 operator *(DTRMVector2 a, long b) {

		return new DTRMVector2(a.x * b, a.y * b);

	}

	public static DTRMVector2 operator /(DTRMVector2 a, long b) {

		return new DTRMVector2(a.x / b, a.y / b);

	}

	public static DTRMVector2 operator *(DTRMVector2 a, FixedPoint b) {

		return new DTRMVector2(a.x * b, a.y * b);

	}

	public static DTRMVector2 operator /(DTRMVector2 a, FixedPoint b) {

		return new DTRMVector2(a.x / b, a.y / b);

	}

	public static bool operator ==(DTRMVector2 a, DTRMVector2 b) {

		return (a.x == b.x) && (a.y == b.y);

	}

	public static bool operator !=(DTRMVector2 a, DTRMVector2 b) {

		return (a.x != b.x) || (a.y != b.y);

	}

}