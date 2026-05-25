using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : BattlerController
{
    // Enemy Decisions (Temporary)
    private int _maxMemory = 3;
    private List<Rank> _memory = new List<Rank>();

    public void AddRankMemory(Rank rank)
    {
        _memory.Add(rank);

        if (_memory.Count > _maxMemory) _memory.RemoveAt(_memory.Count - 1);
    }

    public Rank ChooseRank()
    {
        List<Rank> possibleRanks = HandManager.Hand
        .Select(card => card.Rank)
        .Where(rank => _memory.Contains(rank))
        .Distinct()
        .ToList();

        if (possibleRanks.Count > 0)
        {
            return possibleRanks[UnityEngine.Random.Range(0, possibleRanks.Count)];
        }

        return HandManager.Hand[UnityEngine.Random.Range(0, HandManager.Hand.Count)].Rank;
    }

    public bool TryChoosePeixinho(out List<CardInstance> cards, out bool isFull)
    {
        cards = null;
        isFull = false;

        // Full peixinho has priority
        if (HandManager.HasPeixinho(out var peixinhos))
        {
            cards = peixinhos[Random.Range(0, peixinhos.Count)];

            isFull = true;

            return true;
        }

        // 50% chance for half peixinho
        if (HandManager.HasHalfPeixinho(out var halfPeixinhos))
        {
            bool shouldPlay = Random.value < 0.5f;

            if (shouldPlay)
            {
                cards = halfPeixinhos[Random.Range(0, halfPeixinhos.Count)];

                return true;
            }
        }

        return false;
    }
}
