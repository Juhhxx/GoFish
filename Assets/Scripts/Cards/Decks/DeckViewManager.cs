using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class DeckViewManager : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;

    // Animations
    [SerializeField] private float _deckDropHeight = 5f;
    [SerializeField] private float _deckDropAnimationTime = 0.2f;

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
            i++;

            yield return new WaitForSeconds(_deckDropAnimationTime);
        }
    }

    public void RemoveCard(int card, Action onEnd)
    {
        CardViewManager view = _createdCards[card];

        _createdCards.RemoveAt(card);
        Destroy(view.gameObject);
    }
}
