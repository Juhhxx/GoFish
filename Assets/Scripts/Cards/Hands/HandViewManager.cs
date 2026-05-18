using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HandViewManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HandManager _handManager;
    [SerializeField] private CardViewManager _cardPrefab;

    [Header("Layout")]
    [SerializeField] private float _spacing = 2f;
    [SerializeField] private float _curveAmount = 0.5f;
    [SerializeField] private float _maxRotation = 15f;

    [Header("Animation")]
    [SerializeField] private float _tweenDuration = 0.25f;

    private readonly List<CardViewManager> _cardViews = new List<CardViewManager>();

    private void Awake()
    {
        _handManager.OnHandChanged += RefreshHand;
    }

    private void OnDestroy()
    {
        if (_handManager != null)
            _handManager.OnHandChanged -= RefreshHand;
    }

    private void RefreshHand(List<CardInstance> hand)
    {
        SyncCards(hand);
        UpdateLayout();
    }

    private void SyncCards(List<CardInstance> hand)
    {
        var toDelete = new List<CardViewManager>();
        
        int i = 0;

        foreach (CardViewManager cardview in _cardViews)
        {
            if (cardview.Card != hand[i]) toDelete.Add(cardview);
            i++;
        }

        foreach (CardViewManager cv in toDelete)
        {
            _cardViews.Remove(cv);
        }

        int j = 0;

        foreach (CardInstance card in hand)
        {
            if (_cardViews.Count > i)
            {
                _cardViews[j].SetUpCard(card);
            }
            else
            {
                CardViewManager cardView = Instantiate(_cardPrefab, transform);

                cardView.SetUpCard(card);

                _cardViews.Add(cardView);
            }

            j++;
        }
    }

    private void UpdateLayout()
    {
        int count = _cardViews.Count;

        if (count == 0)
            return;

        float totalWidth = (count - 1) * _spacing;

        for (int i = 0; i < count; i++)
        {
            CardViewManager card = _cardViews[i];

            float t = count == 1 ? 0.5f : (float)i / (count - 1);

            float n = t * 2f - 1f;

            float x = n * totalWidth * 0.5f;

            float y = -Mathf.Pow(n, 2) * _curveAmount;

            float rotation = -n * _maxRotation;

            Vector3 targetPos = new Vector3(
                x,
                y,
                i * -0.01f
            );

            card.transform.DOKill();

            card.transform.DOLocalMove(targetPos, _tweenDuration);

            card.transform.DOLocalRotate(
                new Vector3(0, 0, rotation),
                _tweenDuration
            );
        }
    }
}