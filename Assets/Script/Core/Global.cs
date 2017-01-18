using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour {

}

[System.Serializable]
public class MinMax
{
	public float max;
	public float min;
	public float Rand{
		get {
			return Random.Range( min , max );
		}
	}
}
