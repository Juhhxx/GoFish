using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private List<CardInstance> _hand;
    public List<CardInstance> Hand => _hand;

    public event Action<List<CardInstance>> OnHandChanged;

    private const int MAXSELECTEDCARDS = 4;
    private List<CardInstance> _selectedCards = new List<CardInstance>();
    public event Action<CardInstance, bool> OnHandSelectionChanged;

    public bool Contains(Rank rank)
    {
        return _hand.Any((card) => card.Rank == rank); 
    }

    public IReadOnlyList<CardInstance> GetSelectedCards()
    {
        return _selectedCards;
    }
    public IReadOnlyList<CardInstance> GetCardsOfRank(Rank rank)
    {
        return _hand.Where((card) => card.Rank == rank).ToList();
    }

    public void AddCard(CardInstance card)
    {
        _hand.Add(card);
        _hand.Sort();
        OnHandChanged?.Invoke(_hand);
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

            OnHandSelectionChanged?.Invoke(card, false);

            return;
        }

        if (_selectedCards.Count >= MAXSELECTEDCARDS)
            return;

        _selectedCards.Add(card);

        OnHandSelectionChanged?.Invoke(card, true);
    }

    public void ClearSelection()
    {
        var tmp = new List<CardInstance>(_selectedCards);

        foreach (CardInstance card in tmp)
        {
            ToggleCardSelection(card);
        }
    }

    // Call
    public bool CanCall()
    {
        if (_selectedCards.Count == 0 || _selectedCards.Count == MAXSELECTEDCARDS)
        {
            return false;
        }

        bool allSameRank = _selectedCards
            .GroupBy(card => card.Rank)
            .Count() == 1;
        
        return allSameRank;
    }

    public Rank GetCallRank()
    {
        return _selectedCards[0].Rank;
    }

    // Peixinhos
    public bool HasHalfPeixinho()
    {
        return _selectedCards.Count == 2 &&
            _selectedCards.All(card =>
                card.Rank == _selectedCards[0].Rank);
    }

    public bool HasHalfPeixinho(out List<List<CardInstance>> groups)
    {
        groups = _hand
            .GroupBy(card => card.Rank)
            .Where(group => group.Count() == 2)
            .Select(group => group.ToList())
            .ToList();

        return groups.Count > 0;
    }

    public bool HasPeixinho()
    {
        return _selectedCards.Count == 4 &&
            _selectedCards.All(card =>
                card.Rank == _selectedCards[0].Rank);
    }

    public bool HasPeixinho(out List<List<CardInstance>> groups)
    {
        groups = _hand
            .GroupBy(card => card.Rank)
            .Where(group => group.Count() == 4)
            .Select(group => group.ToList())
            .ToList();

        return groups.Count > 0;
    }

    public Rank GetPeixinhoRank()
    {
        return _selectedCards[0].Rank;
    }
}
