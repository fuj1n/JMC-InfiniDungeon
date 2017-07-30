using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryClassController : MonoBehaviour {
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1) && EntityEnemy.selectedEnemy)
        {
            EntityEnemy.selectedEnemy.Damage(5F);
        }
	}
}
