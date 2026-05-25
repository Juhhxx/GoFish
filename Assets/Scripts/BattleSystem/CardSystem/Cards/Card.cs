using System;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Cards/Card")]
public class Card : ScriptableObject
{
    [field: OnValueChanged("UpdateName")]
    [field: SerializeField] public Rank Rank { get; private set; }
    [field: OnValueChanged("UpdateName")]
    [field: SerializeField] public Suit Suit { get; private set; }

    [field: SerializeField, ReadOnly] public string DisplayName { get; private set; }
    [field: SerializeField, ShowAssetPreview] public Sprite DisplayImage { get; private set; }

    private void UpdateName()
    {
        DisplayName = Rank.ToString() + " of " + Suit.ToString() + "s";
    }

    public CardInstance Instantiate(DeckInstance deck) => 
    new CardInstance(Rank, Suit, DisplayName, DisplayImage, deck.DeckFront, deck.DeckBack);
}

[Serializable]
public class CardInstance : IComparable<CardInstance>
{
    [field: SerializeField, ReadOnly] public Rank Rank { get; private set; }
    [field: SerializeField, ReadOnly] public Suit Suit { get; private set; }
    [field: SerializeField, ReadOnly] public string DisplayName { get; private set; }
    public Sprite DisplayImage { get; private set; }
    public Sprite FrontImage { get; private set; }
    public Sprite BackImage { get; private set; }

    public int CompareTo(CardInstance other)
    {
        if (other == null)
            return 1;

        int rankComparison = other.Rank.CompareTo(Rank);

        if (rankComparison != 0)
            return rankComparison;

        return Suit.CompareTo(other.Suit);
    }

    public CardInstance(Rank rank, Suit suit, string name, Sprite display, Sprite front, Sprite back)
    {
        Rank = rank;
        Suit = suit;
        DisplayName = name;
        DisplayImage = display;
        FrontImage = front;
        BackImage = back;
    }
}
