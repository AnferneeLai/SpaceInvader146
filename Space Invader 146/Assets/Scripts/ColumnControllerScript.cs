using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnControllerScript : MonoBehaviour {

	public float min = 0;
	public float max = 10;

	void Start () {

	}

	void Update () {
		if (transform.childCount == 0)
			Destroy (gameObject);
	}


}
