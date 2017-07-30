using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour {

	private Transform player;

	private void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	private void Update () {
		transform.localEulerAngles = new Vector3(0F, 0F, -player.localEulerAngles.y);
	}
}
