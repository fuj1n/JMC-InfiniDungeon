using UnityEngine;
using UnityEngine.UI;

public class PlayerList : MonoBehaviour {
    public GameObject playerSelector;

    private static I18n i18n = I18n.Get();
    public static PlayerData[] playerData;

	void Start () {
        playerData = PlayerData.ReadAllData();
        
        foreach(PlayerData data in playerData)
        {
            GameObject ob = Instantiate(playerSelector, transform);
            AdvancedSelector adv = ob.GetComponent<AdvancedSelector>();

            adv.data = data.Snowflake;

            ob.transform.Find("Name").GetComponent<Text>().text = data.name;
            ob.transform.Find("LevelClass").GetComponent<Text>().text = "Level " + data.level + " " + i18n.Translate(data.ClassName);
        }
	}
}
