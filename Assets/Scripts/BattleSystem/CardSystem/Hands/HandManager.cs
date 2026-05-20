using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private List<CardInstance> _hand;
    [SerializeField] private HandViewManager _handView;
    public event Action<List<CardInstance>> OnHandChanged;

    private const int MAXSELECTEDCARDS = 4;
    private List<CardInstance> _selectedCards = new List<CardInstance>();
    public event Action<CardInstance, bool> OnHandSelectionChanged;

    public event Action<bool, bool> OnPeixinhoUpdate;
    public event Action<bool, Rank> OnCallUpdate;

    public void AddCard(CardInstance card)
    {
        _hand.Add(card);
        _hand.Sort();
        OnHandChanged.Invoke(_hand);
    }

    public void RemoveCard(CardInstance card)
    {
        _hand.Remove(card);
        _hand.Sort();
        OnHandChanged?.Invoke(_hand);
    }

    public bool IsSelected(CardInstance card)
    {
        return _selectedCards.Contains(card);
    }

    public void ToggleCardSelection(CardInstance card)
    {
        if (_selectedCards.Contains(card))
        {
            _selectedCards.Remove(card);
            _selectedCards.Sort();
            CheckPeixinho();
            CheckCall();

            OnHandSelectionChanged?.Invoke(card, false);

            return;
        }

        if (_selectedCards.Count >= MAXSELECTEDCARDS)
            return;

        _selectedCards.Add(card);
        _selectedCards.Sort();
        CheckPeixinho();
        CheckCall();

        OnHandSelectionChanged?.Invoke(card, true);
    }

    public void CheckPeixinho()
    {
        var groups = _selectedCards
            .GroupBy(card => card.Rank)
            .Select(group => group.Count())
            .ToList();

        Debug.Log(groups);

        bool hasExactlyOnePair =    groups.Count(count => count == 2) == 1 &&
                                    groups.All(count => count <= 2);

        bool hasFourOfAKind =       _selectedCards.Count == 4 &&
                                    groups.Count == 1;

        // First check is if there is a peixinho, seconf is if there is a 4 card peixinho
        OnPeixinhoUpdate?.Invoke(hasExactlyOnePair || hasFourOfAKind, hasFourOfAKind);
    }

    public void CheckCall()
    {
        if (_selectedCards.Count == 0)
        {
            OnCallUpdate?.Invoke(false, Rank.Ace);
            return;
        }

        bool allSameRank = _selectedCards
            .GroupBy(card => card.Rank)
            .Count() == 1;
        
        OnCallUpdate?.Invoke(allSameRank, _selectedCards[0].Rank);
    }
}
