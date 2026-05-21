using System;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private DeckViewManager _deckView;

    [SerializeField] private DeckInstance _deckInst;
    public DeckInstance Deck => _deckInst;

    [SerializeField] private bool _isReady = false;
    public bool IsReady => _isReady;

    [SerializeField] private bool _canInteract = true;
    public bool CanInteract => _canInteract;

    public void ToggleDeck(bool onOff)
    {
        _canInteract = onOff;
        _deckView.ToggleWarning(onOff);
    }

    private void Start()
    {
        _deckView.ToggleWarning(false);
    }

    public void InitializeDeck(Deck deck)
    {
        _deckInst = deck.Instantiate();
        _deckView.CreateDeck(_deckInst, () => _isReady = true);
    }

    public Rank GiveCard(HandManager hand, bool toPLayer)
    {
        if (!_isReady) return Rank.None;

        CardInstance card = _deckInst.GetCard();

        if (card == null) return Rank.None;
        
        _deckView.RemoveCard(card, () => hand.AddCard(card), toPLayer);

        return card.Rank;
    }
}
 