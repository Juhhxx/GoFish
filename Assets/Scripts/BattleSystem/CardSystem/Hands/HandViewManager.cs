using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class HandViewManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HandManager _handManager;
    [SerializeField] private BattleViewManager _battleView;
    [SerializeField] private CardViewManager _cardPrefab;

    [Header("Layout")]
    [SerializeField] private float _spacing = 2f;
    [SerializeField] private float _curveAmount = 0.5f;
    [SerializeField] private float _maxRotation = 15f;

    [Header("Animation")]
    [SerializeField] private float _tweenDuration = 0.25f;

    private List<CardViewManager> _cardViews = new List<CardViewManager>();


    private void Awake()
    {
        _handManager.OnHandChanged += RefreshHand;
        _handManager.OnHandSelectionChanged += OnCardSelectionChanged;

        if (_battleView != null)
        {
            _battleView.OnCallButtonHovered += (onOff) => SelectionAnimationsHandle(true, onOff);
            _battleView.OnCallButtonPressed += () => DoStopSelectionAnimation(true);

            _battleView.OnPeixinhoButtonHovered += (onOff) => SelectionAnimationsHandle(false, onOff);
            _battleView.OnPeixinhoButtonPressed += () => DoStopSelectionAnimation(true);

            _battleView.OnHalfPeixinhoButtonHovered += (onOff) => SelectionAnimationsHandle(false, onOff);
            _battleView.OnHalfPeixinhoButtonPressed += () => DoStopSelectionAnimation(true);
        }
    }

    private void OnDestroy()
    {
        if (_handManager != null)
            _handManager.OnHandChanged -= RefreshHand;
            _handManager.OnHandSelectionChanged -= OnCardSelectionChanged;
    }


    private void OnCardSelectionChanged(CardInstance card, bool selected)
    {
        CardViewManager view = _cardViews.Find(v => v.Card == card);

        if (view != null)
        {
            view.SetSelected(selected);
        }
    }
    private List<CardViewManager> GetSelectedCards()
    {
        return _cardViews.Where((view) => view.Selected).ToList();
    }

    private void SelectionAnimationsHandle(bool isCall, bool onOff)
    {
        if (isCall && onOff) DoSelectionCallAnim();
        else if (!isCall && onOff) DoSelectionPeixinhoAnim();
        else DoStopSelectionAnimation();
    }

    private void DoSelectionCallAnim()
    {
        foreach(CardViewManager view in GetSelectedCards())
        {
            view.DoCallAnim();
        }
    }

    private void DoSelectionPeixinhoAnim()
    {
        foreach(CardViewManager view in GetSelectedCards())
        {
            view.DoPeixinhoAnim();
        }
    }
    private void DoStopSelectionAnimation(bool toggleOutline = false)
    {
        foreach(CardViewManager view in GetSelectedCards())
        {
            view.DoStopAnimations(toggleOutline);
        }
    }


    private void RefreshHand(List<CardInstance> hand)
    {
        RemoveDeletedCards(hand);

        CreateMissingCards(hand);

        SortCardViews(hand);

        UpdateLayout();
    }

    private void RemoveDeletedCards(List<CardInstance> hand)
    {
        for (int i = _cardViews.Count - 1; i >= 0; i--)
        {
            CardViewManager view = _cardViews[i];
            var col = view.GetComponent<Collider>();

            if (!hand.Contains(view.Card))
            {
                _cardViews.RemoveAt(i);
                col.enabled = false;

                var pos = view.transform.localPosition;
                pos.y = -2.5f;

                view.DoCardMoveAnim(pos, () => Destroy(view.gameObject));
            }
        }
    }

    private void CreateMissingCards(List<CardInstance> hand)
    {
        foreach (CardInstance card in hand)
        {
            bool exists = _cardViews.Exists(v => v.Card == card);

            if (exists)
                continue;

            CardViewManager cardView = Instantiate(_cardPrefab, transform);

            Vector3 pos = cardView.transform.localPosition;
            pos.y -= 5f;

            cardView.transform.localPosition = pos;

            cardView.SetUpCard(card);

            _cardViews.Add(cardView);

            Button button = cardView.GetComponent<Button>();

            button.InteractBegin += () => _handManager.ToggleCardSelection(card);
        }
    }

    private void SortCardViews(List<CardInstance> hand)
    {
        _cardViews.Sort((a, b) =>
        {
            int indexA = hand.IndexOf(a.Card);
            int indexB = hand.IndexOf(b.Card);

            return indexA.CompareTo(indexB);
        });
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
                i * -0.025f
            );

            card.SetOrigin(targetPos, rotation);

            card.transform.DOKill();

            card.transform.DOLocalMove(targetPos, _tweenDuration);

            card.transform.DOLocalRotate(
                new Vector3(0, 0, rotation),
                _tweenDuration
            );
        }
    }
}