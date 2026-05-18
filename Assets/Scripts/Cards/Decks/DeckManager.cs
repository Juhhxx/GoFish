using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private Deck _deck;
    [SerializeField] private HandManager _playerHand;
    [SerializeField] private DeckViewManager _deckView;

    private DeckInstance _deckInst;
    public DeckInstance Deck => _deckInst;

    private void Start()
    {
        _deckInst = _deck.Instantiate();
        _deckView.CreateDeck(_deckInst);
    }

    public void GivePlayerCard()
    {
        _deckView.RemoveCard(_deckInst.Cards.Count - 1, null);

        CardInstance card = _deckInst.GetCard();

        _playerHand.AddCard(card);

    }
}
 