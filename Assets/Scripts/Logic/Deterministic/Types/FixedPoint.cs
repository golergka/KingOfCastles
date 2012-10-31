using UnityEngine;
using System.Collections;

public class FixedPoint {

	private long value = 0;
	private const long fixedPoint = 10000;

	public FixedPoint(int value = 0) {

		this.value = value * fixedPoint;

	}

	public FixedPoint(float value) {

		this.value = (long) (value * fixedPoint);

	}

	public float ToFloat() {

		return ( (float) value ) / fixedPoint;

	}

	public int ToInt() {

		return (int) ( value / fixedPoint );

	}
	
	public static implicit operator FixedPoint(float value) {
		
		return new FixedPoint(value);
		
	}

	//
	// operator (DTRMLong, DTRMLong)
	//
	#region FixedPoint operators

	public static FixedPoint operator +(FixedPoint a, FixedPoint b) {

		FixedPoint result = new FixedPoint();
		result.value = a.value + b.value;
		return result;

	}

	public static FixedPoint operator -(FixedPoint a, FixedPoint b) {

		FixedPoint result = new FixedPoint();
		result.value = a.value - b.value;
		return result;

	}

	public static FixedPoint operator *(FixedPoint a, FixedPoint b) {

		FixedPoint result = new FixedPoint();
		result.value = (a.value * b.value) / fixedPoint;
		return result;

	}

	public static FixedPoint operator /(FixedPoint a, FixedPoint b) {

		FixedPoint result = new FixedPoint();
		result.value = (a.value * fixedPoint) / b.value;
		return result;

	}

	public static bool operator <(FixedPoint a, FixedPoint b) {

		return (a.value < b.value);

	}

	public static bool operator >(FixedPoint a, FixedPoint b) {

		return (a.value > b.value);

	}

	public static bool operator <=(FixedPoint a, FixedPoint b) {

		return (a.value <= b.value);

	}

	public static bool operator >=(FixedPoint a, FixedPoint b) {

		return (a.value >= b.value);

	}

	public static bool operator ==(FixedPoint a, FixedPoint b) {

		return (a.value == b.value);

	}

	public static bool operator !=(FixedPoint a, FixedPoint b) {

		return (a.value != b.value);

	}
	#endregion

	//
	// operator (DTRMLong, long)
	//
	#region long operators
	
	public static FixedPoint operator +(FixedPoint a, long b) {

		FixedPoint result = new FixedPoint();
		result.value = a.value + ( b * fixedPoint );
		return result;

	}

	public static FixedPoint operator -(FixedPoint a, long b) {

		FixedPoint result = new FixedPoint();
		result.value = a.value - ( b * fixedPoint );
		return result;

	}

	public static FixedPoint operator *(FixedPoint a, long b) {

		FixedPoint result = new FixedPoint();
		result.value = a.value * b;
		return result;

	}

	public static FixedPoint operator /(FixedPoint a, long b) {

		FixedPoint result = new FixedPoint();
		result.value = a.value / b;
		return result;

	}

	public static bool operator <(FixedPoint a, long b) {

		return (a.value < b * fixedPoint);

	}

	public static bool operator >(FixedPoint a, long b) {

		return (a.value > b * fixedPoint);

	}

	public static bool operator <=(FixedPoint a, long b) {

		return (a.value <= b * fixedPoint);

	}

	public static bool operator >=(FixedPoint a, long b) {

		return (a.value >= b * fixedPoint);

	}

	public static bool operator ==(FixedPoint a, long b) {

		return (a.value == b * fixedPoint);

	}

	public static bool operator !=(FixedPoint a, long b) {

		return (a.value != b * fixedPoint);

	}
	
	#endregion
	
	//
	// operator (DTRMLong, int)
	//
	#region int operators

	public static FixedPoint operator +(FixedPoint a, int b) {

		FixedPoint result = new FixedPoint();
		result.value = a.value + ( b * fixedPoint );
		return result;

	}

	public static FixedPoint operator -(FixedPoint a, int b) {

		FixedPoint result = new FixedPoint();
		result.value = a.value - ( b * fixedPoint );
		return result;

	}

	public static FixedPoint operator *(FixedPoint a, int b) {

		FixedPoint result = new FixedPoint();
		result.value = a.value * b;
		return result;

	}

	public static FixedPoint operator /(FixedPoint a, int b) {

		FixedPoint result = new FixedPoint();
		result.value = a.value / b;
		return result;

	}

	public static bool operator <(FixedPoint a, int b) {

		return (a.value < b * fixedPoint);

	}

	public static bool operator >(FixedPoint a, int b) {

		return (a.value > b * fixedPoint);

	}

	public static bool operator <=(FixedPoint a, int b) {

		return (a.value <= b * fixedPoint);

	}

	public static bool operator >=(FixedPoint a, int b) {

		return (a.value >= b * fixedPoint);

	}

	public static bool operator ==(FixedPoint a, int b) {

		return (a.value == b * fixedPoint);

	}

	public static bool operator !=(FixedPoint a, int b) {

		return (a.value != b * fixedPoint);

	}
	#endregion

	//
	// Object overrides
	//
	#region standard overrides

	public override bool Equals(object o) {

		if (o == null)
			return false;

		FixedPoint dtrmLong = (FixedPoint) o;

		if (dtrmLong == null)
			return false;

		return (this == o);

	}

	public override int GetHashCode() {

		return value.GetHashCode();

	}

	public FixedPoint sqrt {

		get {

			if (this <= 0)
				return new FixedPoint(0);

			FixedPoint n = ( this / 2) + 1;
			FixedPoint n1 = ( n + (this / n) ) / 2;
			while (n1 < n) {
				n = n1;
				n1 = (n + (this / n)) /2;

			}
			return n;

		}

	}

	public override string ToString() {

		return this.ToFloat().ToString();

	}
	#endregion
	
}
