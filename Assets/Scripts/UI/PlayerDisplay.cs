using UnityEngine;

public class PlayerDisplay : MonoBehaviour
{
    GameObject currentModel;

    public void Switch(AdvancedSelector sel, AdvancedSelector.EventAction act)
    {
        if (act == AdvancedSelector.EventAction.DESELECT)
            return;

        PlayerData pd = MainMenuActions.FindCharacter(sel.data);

        if (pd == null)
            return;

        if (currentModel)
            Destroy(currentModel);

        currentModel = Instantiate(Resources.Load<GameObject>(PlayerData.prefabs[pd.playerClass]), transform);
        currentModel.transform.position = transform.position;

        foreach (MonoBehaviour beh in currentModel.GetComponentsInChildren<MonoBehaviour>())
            Destroy(beh);
    }
}
