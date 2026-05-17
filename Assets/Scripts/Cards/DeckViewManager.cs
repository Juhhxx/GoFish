using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class DeckViewManager : MonoBehaviour
{
    // Temporary
    [SerializeField] private Deck _deck;
    [SerializeField] private GameObject _cardPrefab; 

    private List<GameObject> _createdCards = new List<GameObject>();

    [Button()]
    public void ShowDeck()
    {
        if (_deck == null) return;

        foreach (GameObject go in _createdCards)
        {
#if UNITY_EDITOR
            DestroyImmediate(go);
#else
            Destroy(go);
#endif
        }
        _createdCards.Clear();

        int i = 0;

        foreach (Card c in _deck.Cards)
        {
            var card = Instantiate(_cardPrefab, transform);
            card.transform.localPosition = new Vector3(0f, 0.005f * i, 0f);
            card.transform.rotation = Quaternion.Euler(-90f, 180f, 0f);

            card.GetComponent<CardViewManager>()?.SetUpCard(c.DisplayImage, _deck.DeckFront, _deck.DeckBack);
            _createdCards.Add(card);
            i++;
        }
    }
}
