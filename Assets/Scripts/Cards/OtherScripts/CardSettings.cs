using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardSettings", menuName = "Cards/CardSettings")]
public class CardSettings : ScriptableObject
{
    [SerializeField] private List<RankHability> _rankHabilities;
    [SerializeField] private List<HabilityValue> _habilityValues;
    
    public Hability GetHabilityByRank(Rank rank)
    {
        foreach (RankHability rh in _rankHabilities)
        {
            if (rh.Rank == rank) return rh.Hability;
        }

        return Hability.None;
    }

    public int GetValueByRank(Rank rank)
    {
        foreach (HabilityValue hv in _habilityValues)
        {
            Hability hability = GetHabilityByRank(rank);

            if (hv.Hability == hability) return hv.Value;
        }
        return 0;
    }
}

[System.Serializable]
public struct RankHability
{
    public Rank Rank;
    public Hability Hability;
}
[System.Serializable]
public struct HabilityValue
{
    public int Value;
    public Hability Hability;
}