using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Deck", menuName = "Cards/Deck")]
public class Deck : ScriptableObject
{
    [field: SerializeField] public string DeckName;
    [field: SerializeField, ShowAssetPreview] public Sprite DeckFront;
    [field: SerializeField, ShowAssetPreview] public Sprite DeckBack;

    [SerializeField, ReorderableList] public List<Card> Cards;

    public DeckInstance Instantiate() => new DeckInstance(DeckName, DeckFront, DeckBack, Cards);

    public Card GetRandomCard()
    {
        return Cards[UnityEngine.Random.Range(0,Cards.Count)];
    }
}

[Serializable]
public class DeckInstance
{
    [field: SerializeField] public string DeckName;
    [field: SerializeField, ShowAssetPreview] public Sprite DeckFront;
    [field: SerializeField, ShowAssetPreview] public Sprite DeckBack;

    [SerializeField, ReorderableList] private List<CardInstance> _cards;
    public List<CardInstance> Cards => _cards;

    public DeckInstance(string name, Sprite front, Sprite back, List<Card> cards)
    {
        DeckName = name;
        DeckFront = front;
        DeckBack = back;

        _cards = new List<CardInstance>();
        foreach (Card card in cards) Cards.Add(card.Instantiate(this));

        _cards.Shuffle();

        Debug.Log($"Initialized Deck Size : {_cards.Count}");
    }

    public CardInstance GetRandomCard()
    {
        return _cards[UnityEngine.Random.Range(0,_cards.Count)];
    }

    public CardInstance GetCard()
    {
        CardInstance card;

        if (_cards.TryPop(out card)) return card;
        return null;
    }
}