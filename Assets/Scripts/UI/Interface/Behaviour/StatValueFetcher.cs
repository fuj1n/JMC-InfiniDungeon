using UnityEngine;
using UnityEngine.UI;

public class StatValueFetcher : MonoBehaviour {
    public bool fetchAvailableStats = false;

    [ConditionalHide("fetchAvailableStats", true, true)]
    public PlayerData.Stats.Stat stat;
    [ConditionalHide("fetchAvailableStats", true, true)]
    public DataSource source;

    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void Update () {
        if (fetchAvailableStats)
        {
            text.text = PlayerData.Instance.UnusedStatPoints.ToString();
            return;
        }

        PlayerData.Stats stats = source == DataSource.RAW ? PlayerData.Instance.rawStats : PlayerData.Instance.CalculateStats();

        text.text = stats.GetStat(stat).ToString();
	}

    public enum DataSource
    {
        RAW,
        CALCULATED
    }
}
