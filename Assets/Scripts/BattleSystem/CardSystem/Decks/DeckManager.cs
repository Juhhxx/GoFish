using NaughtyAttributes;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private Deck _deck;
    [SerializeField] private HandManager _playerHand;
    [SerializeField] private HandManager _enemyHand;
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
        CardInstance card = _deckInst.GetCard();

        if (card == null) return;
        
        _deckView.RemoveCard(card, () => _playerHand.AddCard(card), true);
    }

    [Button]
    public void GiveEnemyCard()
    {
        CardInstance card = _deckInst.GetCard();

        if (card == null) return;

        _deckView.RemoveCard(card, () => _enemyHand.AddCard(card), false);
    }
}
 