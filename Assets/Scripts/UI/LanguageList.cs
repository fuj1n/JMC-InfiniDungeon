using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageList : MonoBehaviour {
    public GameObject languageSelector;

    private static I18n i18n = I18n.Get();

    private void Start()
    {
        AdvancedSelector.ResetState();

        foreach(I18n.Language lang in i18n.languages.Values)
        {
            GameObject ob = Instantiate(languageSelector, transform);
            AdvancedSelector adv = ob.GetComponent<AdvancedSelector>();

            adv.data = lang.id;
            Text txt = adv.GetComponentInChildren<Text>();
            txt.text = txt.text.Replace("#LangName", lang.name).Replace("#LangCode", lang.id);

            if (lang.id == i18n.currentLanguage.id)
                adv.Select();
        }

        AdvancedSelector.Subscribe("lang", Select);
    }

    private void Select(AdvancedSelector sel, AdvancedSelector.EventAction act)
    {
        if (act != AdvancedSelector.EventAction.SELECT || !i18n.languages.ContainsKey(sel.data))
            return;

        i18n.currentLanguage.Free();
        i18n.currentLanguage = i18n.languages[sel.data];
        i18n.SaveState();

        MainMenuActions mma = FindObjectOfType<MainMenuActions>();

        if (mma)
            mma.OpenOptions();
    }
}
