﻿using UnityEngine;

public class Rotate : MonoBehaviour {
	public Vector3 speed = new Vector3(0, 1, 0);

	void Update () {
		transform.Rotate(speed);
	}
}
