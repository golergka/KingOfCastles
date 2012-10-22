using UnityEngine;
using System.Collections;

public class DTRMLong {

	private long value = 0;
	private const long fixedPoint = 10000;

	public DTRMLong(int value = 0) {

		this.value = value * fixedPoint;

	}

	public DTRMLong(float value) {

		this.value = (long) (value * fixedPoint);

	}

	public float ToFloat() {

		return ( (float) value ) / fixedPoint;

	}

	public int ToInt() {

		return (int) ( value / fixedPoint );

	}

	//
	// operator (DTRMLong, DTRMLong)
	//

	public static DTRMLong operator +(DTRMLong a, DTRMLong b) {

		DTRMLong result = new DTRMLong();
		result.value = a.value + b.value;
		return result;

	}

	public static DTRMLong operator -(DTRMLong a, DTRMLong b) {

		DTRMLong result = new DTRMLong();
		result.value = a.value - b.value;
		return result;

	}

	public static DTRMLong operator *(DTRMLong a, DTRMLong b) {

		DTRMLong result = new DTRMLong();
		result.value = (a.value * b.value) / fixedPoint;
		return result;

	}

	public static DTRMLong operator /(DTRMLong a, DTRMLong b) {

		DTRMLong result = new DTRMLong();
		result.value = (a.value * fixedPoint) / b.value;
		return result;

	}

	public static bool operator <(DTRMLong a, DTRMLong b) {

		return (a.value < b.value);

	}

	public static bool operator >(DTRMLong a, DTRMLong b) {

		return (a.value > b.value);

	}

	public static bool operator <=(DTRMLong a, DTRMLong b) {

		return (a.value <= b.value);

	}

	public static bool operator >=(DTRMLong a, DTRMLong b) {

		return (a.value >= b.value);

	}

	public static bool operator ==(DTRMLong a, DTRMLong b) {

		return (a.value == b.value);

	}

	public static bool operator !=(DTRMLong a, DTRMLong b) {

		return (a.value != b.value);

	}

	//
	// operator (DTRMLong, long)
	//

	public static DTRMLong operator +(DTRMLong a, long b) {

		DTRMLong result = new DTRMLong();
		result.value = a.value + ( b * fixedPoint );
		return result;

	}

	public static DTRMLong operator -(DTRMLong a, long b) {

		DTRMLong result = new DTRMLong();
		result.value = a.value - ( b * fixedPoint );
		return result;

	}

	public static DTRMLong operator *(DTRMLong a, long b) {

		DTRMLong result = new DTRMLong();
		result.value = a.value * b;
		return result;

	}

	public static DTRMLong operator /(DTRMLong a, long b) {

		DTRMLong result = new DTRMLong();
		result.value = a.value / b;
		return result;

	}

	public static bool operator <(DTRMLong a, long b) {

		return (a.value < b * fixedPoint);

	}

	public static bool operator >(DTRMLong a, long b) {

		return (a.value > b * fixedPoint);

	}

	public static bool operator <=(DTRMLong a, long b) {

		return (a.value <= b * fixedPoint);

	}

	public static bool operator >=(DTRMLong a, long b) {

		return (a.value >= b * fixedPoint);

	}

	public static bool operator ==(DTRMLong a, long b) {

		return (a.value == b * fixedPoint);

	}

	public static bool operator !=(DTRMLong a, long b) {

		return (a.value != b * fixedPoint);

	}

	//
	// operator (DTRMLong, int)
	//

	public static DTRMLong operator +(DTRMLong a, int b) {

		DTRMLong result = new DTRMLong();
		result.value = a.value + ( b * fixedPoint );
		return result;

	}

	public static DTRMLong operator -(DTRMLong a, int b) {

		DTRMLong result = new DTRMLong();
		result.value = a.value - ( b * fixedPoint );
		return result;

	}

	public static DTRMLong operator *(DTRMLong a, int b) {

		DTRMLong result = new DTRMLong();
		result.value = a.value * b;
		return result;

	}

	public static DTRMLong operator /(DTRMLong a, int b) {

		DTRMLong result = new DTRMLong();
		result.value = a.value / b;
		return result;

	}

	public static bool operator <(DTRMLong a, int b) {

		return (a.value < b * fixedPoint);

	}

	public static bool operator >(DTRMLong a, int b) {

		return (a.value > b * fixedPoint);

	}

	public static bool operator <=(DTRMLong a, int b) {

		return (a.value <= b * fixedPoint);

	}

	public static bool operator >=(DTRMLong a, int b) {

		return (a.value >= b * fixedPoint);

	}

	public static bool operator ==(DTRMLong a, int b) {

		return (a.value == b * fixedPoint);

	}

	public static bool operator !=(DTRMLong a, int b) {

		return (a.value != b * fixedPoint);

	}

	//
	// Object overrides
	//

	public override bool Equals(object o) {

		if (o == null)
			return false;

		DTRMLong dtrmLong = (DTRMLong) o;

		if (dtrmLong == null)
			return false;

		return (this == o);

	}

	public override int GetHashCode() {

		return value.GetHashCode();

	}

	public DTRMLong sqrt {

		get {

			if (this <= 0)
				return new DTRMLong(0);

			DTRMLong n = ( this / 2) + 1;
			DTRMLong n1 = ( n + (this / n) ) / 2;
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

}
