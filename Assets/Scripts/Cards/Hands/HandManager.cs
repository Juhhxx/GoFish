using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private List<CardInstance> _hand;
    [SerializeField] private Deck _deck;
    private DeckInstance _deckI;
    
    public event Action<List<CardInstance>> OnHandChanged;

    [Button]
    private void UpdateHand()
    {
        AddCard(_deckI.GetRandomCard());
    }

    private void Start()
    {
        _deckI = _deck.Instantiate();
    }

    public void AddCard(CardInstance card)
    {
        _hand.Add(card);
        OnHandChanged.Invoke(_hand);
    }

    public void RemoveCard(CardInstance card)
    {
        _hand.Remove(card);
        OnHandChanged?.Invoke(_hand);
    }
}
