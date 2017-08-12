using System;
using UnityEngine;

public class StatInterfaceButtons : MonoBehaviour {
    public void StatUp(string stat)
    {
        StatUp((PlayerData.Stats.Stat)Enum.Parse(typeof(PlayerData.Stats.Stat), stat));
    }

    public void StatUp(PlayerData.Stats.Stat stat)
    {
        if(PlayerData.Instance.UnusedStatPoints > 0)
            switch (stat)
            {
                case PlayerData.Stats.Stat.DEXTERITY:
                    PlayerData.Instance.rawStats.dexterity++;
                    break;
                case PlayerData.Stats.Stat.INTELLIGENCE:
                    PlayerData.Instance.rawStats.intelligence++;
                    break;
                case PlayerData.Stats.Stat.STRENGTH:
                    PlayerData.Instance.rawStats.strength++;
                    break;
                case PlayerData.Stats.Stat.VITALITY:
                    PlayerData.Instance.rawStats.vitality++;
                    break;
            }
    }
}
