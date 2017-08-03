using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHelper : MonoBehaviour {
    private static UIHelper instance;
    private static I18n i18n = I18n.Get();

    private GameObject alertInst;

    private static UIHelper GetInstance()
    {
        if(!instance)
        {
            GameObject go = new GameObject("UIHelper");
            Canvas c = go.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;

            instance = go.AddComponent<UIHelper>();
            DontDestroyOnLoad(instance);
        }

        return instance;
    }

    public static void Alert(string alert)
    {
        UIHelper inst = GetInstance();
            inst.StartCoroutine(inst._Alert(alert));
    }

    public IEnumerator _Alert(string alert)
    {
        if (alertInst)
            yield break;

        alertInst = Instantiate(Resources.Load<GameObject>("UI/Alert"), transform);

        alertInst.GetComponentInChildren<Text>().text = i18n.Translate(alert);

        yield return new WaitForSeconds(2.5F);

        Destroy(alertInst);
        alertInst = null;
    }
}
