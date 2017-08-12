using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public static Func<bool> canPauseFunc = null;

    private GameObject sub;

    private void Awake()
    {
        canPauseFunc = null;

        sub = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        bool pausing = Time.timeScale != .0F;

        if (pausing && canPauseFunc != null && !canPauseFunc())
            return;

        Time.timeScale = pausing ? .0F : 1F;
        sub.SetActive(pausing);
    }

    public void SaveAndQuit()
    {
        PlayerData.Instance.SaveData();

        Time.timeScale = 1F;

        PlayerData.Instance = null;
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
