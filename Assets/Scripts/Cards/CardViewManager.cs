using UnityEngine;

public class CardViewManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _cardDisplay;
    [SerializeField] private SpriteRenderer _cardFront;
    [SerializeField] private SpriteRenderer _cardBack;

    public void SetUpCard(Sprite display, Sprite front, Sprite back)
    {
        _cardDisplay.sprite = display;
        _cardFront.sprite = front;
        _cardBack.sprite = back;
    }
}
