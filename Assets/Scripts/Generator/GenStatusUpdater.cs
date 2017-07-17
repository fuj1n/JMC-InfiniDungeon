using UnityEngine;
using UnityEngine.UI;

public class GenStatusUpdater : MonoBehaviour
{
    public string status = "Generating Dungeon";

    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();

        if (!text)
            enabled = false;
    }

    private void Update()
    {
        if (text)
            text.text = status;
    }
}
