using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuActions : MonoBehaviour
{
    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenCharacterSelect()
    {
        AdvancedSelector.ResetState();
        SceneManager.LoadScene("CharacterScreen");
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene("OptionsScreen");
    }

    public void CreateCharacter_Do()
    {
        string name = GameObject.Find("NameField").GetComponent<InputField>().text;
        AdvancedSelector selectedClass = AdvancedSelector.FindSelected("selectedClass");

        if (!selectedClass)
            UIHelper.Alert("alerts.character.notype");
        else if (string.IsNullOrEmpty(name))
            UIHelper.Alert("alerts.character.noname");
        else
        {
            PlayerData.PlayerClass cls = (PlayerData.PlayerClass)Enum.Parse(typeof(PlayerData.PlayerClass), selectedClass.data);

            PlayerData pd = new PlayerData(name, cls);
            pd.SaveData();

            OpenCharacterSelect();
        }
    }

    public static PlayerData FindCharacter(string snowflake)
    {
        return (from d in PlayerList.playerData
                where d.Snowflake == snowflake
                select d).Single();
    }

    public void LoadCharacter()
    {
        AdvancedSelector selected = AdvancedSelector.FindSelected("playerSelector");

        if (!selected)
            UIHelper.Alert("alerts.character.nosel");
        else
        {
            PlayerData pd = FindCharacter(selected.data);

            PlayerData.Instance = pd;

            SceneManager.LoadScene("Test");
        }
    }

    public void DeleteCharacter()
    {
        AdvancedSelector selected = AdvancedSelector.FindSelected("playerSelector");

        if (!selected)
            UIHelper.Alert("alerts.character.nosel");
        else
        {
            PlayerData pd = FindCharacter(selected.data);

            pd.DeleteSave();

            OpenCharacterSelect();
        }
    }

    //TODO: confirm delete
    //private IEnumerator ConfirmDelete(PlayerData pd)
    //{

    //}

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
