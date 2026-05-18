using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardSettings", menuName = "Cards/CardSettings")]
public class CardSettings : ScriptableObject
{
    [SerializeField] private List<RankHability> _rankHabilities;
    
    public Hability GetHabilityByRank(Rank rank)
    {
        foreach (RankHability rh in _rankHabilities)
        {
            if (rh.Rank == rank) return rh.Hability;
        }

        return Hability.None;
    }
}

[System.Serializable]
public struct RankHability
{
    public Rank Rank;
    public Hability Hability;
}
