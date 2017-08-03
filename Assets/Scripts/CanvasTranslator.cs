using UnityEngine;
using UnityEngine.UI;

public class CanvasTranslator : MonoBehaviour {

    private static I18n i18n = I18n.Get();

	private void Start () {
        foreach (Text t in GetComponentsInChildren<Text>(includeInactive: true))
            t.text = i18n.Translate(t.text);
	}
}
