using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatButtonHide : MonoBehaviour {
    public GameObject[] buttons;

    private void Awake()
    {
        Update();
    }

    private void Update()
    {
        foreach (GameObject go in buttons)
            go.SetActive(PlayerData.Instance.UnusedStatPoints > 0);
    }
}
