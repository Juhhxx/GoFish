using UnityEngine;

public class CardViewManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _cardDisplay;
    [SerializeField] private SpriteRenderer _cardFront;
    [SerializeField] private SpriteRenderer _cardBack;

    public CardInstance Card { get; private set; }

    public void SetUpCard(CardInstance card)
    {
        _cardDisplay.sprite = card.DisplayImage;
        _cardFront.sprite = card.FrontImage;
        _cardBack.sprite = card.BackImage;

        Card = card;
    }
}
