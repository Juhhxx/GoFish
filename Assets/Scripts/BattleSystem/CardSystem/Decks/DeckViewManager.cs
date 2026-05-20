using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class DeckViewManager : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;

    // Animations
    [SerializeField] private float _deckDropHeight = 5f;
    [SerializeField] private float _deckDropAnimationTime = 0.2f;
    [SerializeField] private float _giveCardsAnimationTime = 0.2f;
    [SerializeField] private Transform _playerGiveCardPivot;
    [SerializeField] private Transform _enemyGiveCardPivot;

    private List<CardViewManager> _createdCards = new List<CardViewManager>();

    public void CreateDeck(DeckInstance deck)
    {
        foreach (CardViewManager cv in _createdCards)
        {
#if UNITY_EDITOR
            DestroyImmediate(cv.gameObject);
#else
            Destroy(cv.gameObject);
#endif
        }
        _createdCards.Clear();

        StartCoroutine(CreateDeckCR(deck));
    }

    private IEnumerator CreateDeckCR(DeckInstance deck)
    {
        int i = 0;

        foreach (CardInstance c in deck.Cards)
        {
            var card = Instantiate(_cardPrefab, transform);
            card.transform.localPosition = new Vector3(0f, _deckDropHeight, 0f);
            card.transform.rotation = Quaternion.Euler(-90f, 180f, 0f);

            Vector3 finalPos = new Vector3(0f, 0.005f * i, 0f);

            CardViewManager view = card.GetComponent<CardViewManager>();

            view?.SetUpCard(c);
            view?.DoCardMoveAnim(finalPos);

            _createdCards.Add(view);

            card.GetComponent<Collider>().enabled = false;

            SpriteRenderer[] sprs = card.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spr in sprs) spr.sortingOrder = -1;
            i++;

            yield return new WaitForSeconds(_deckDropAnimationTime);
        }
    }

    public void RemoveCard(CardInstance card, Action onEnd, bool player)
    {
        CardViewManager view = _createdCards.Find((c) => c.Card == card);

        onEnd += () => Destroy(view.gameObject);

        _createdCards.Remove(view);

        Vector3 pos = player ? _playerGiveCardPivot.position : _enemyGiveCardPivot.position;

        view.transform.DOMove(pos, _giveCardsAnimationTime).OnComplete(() => onEnd?.Invoke());

    }
}
