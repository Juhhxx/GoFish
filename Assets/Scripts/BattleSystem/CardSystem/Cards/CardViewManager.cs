using System;
using DG.Tweening;
using UnityEngine;

public class CardViewManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _cardDisplay;
    [SerializeField] private SpriteRenderer _cardFront;
    [SerializeField] private SpriteRenderer _cardBack;
    [SerializeField] private SpriteRenderer _cardOutline;

    [SerializeField] private Color _outlineColor = Color.white;
    [SerializeField] private Color _outlineSelectedColor = Color.yellow;

    [Header("Animations")]
    [SerializeField] private float _hoverScaleAmount;
    [SerializeField] private Vector3 _hoverMoveAmount;
    [SerializeField] private float _tweenDuration;
    [SerializeField] private float _tweenDeckSpawnDuration;

    private Vector3 _originPos;
    private bool _selected = false;

    public CardInstance Card { get; private set; }

    public void Start()
    {
        // _originPos = transform.position;
    }

    public void SetOrigin(Vector3 pos) => _originPos = pos;

    public void SetUpCard(CardInstance card)
    {
        _cardDisplay.sprite = card.DisplayImage;
        _cardFront.sprite = card.FrontImage;
        _cardBack.sprite = card.BackImage;
        _cardOutline.enabled = false;

        Card = card;

        gameObject.name = card.DisplayName;
    }

    public void SetSelected(bool selected)
    {
        _selected = selected;

        if (selected)
        {
            DoSelectedAnim();
        }
        else
        {
            DoUnselectAnim();
        }
    }

    // Animations

    public void DoCardMoveAnim(Vector3 finalPos)
    {
        transform.DOKill();
        transform.DOLocalMove(finalPos, _tweenDeckSpawnDuration);
    }

    public void DoCardHoverAnim()
    {

        transform.DOKill();
        transform.DOScale(Vector3.one * _hoverScaleAmount, _tweenDuration);
        transform.DOLocalMove(_originPos + _hoverMoveAmount, _tweenDuration);
        _cardOutline.enabled = true;
    }

    public void DoCardUnhoverAnim()
    {

        transform.DOKill();
        transform.DOScale(Vector3.one, _tweenDuration);
        transform.DOLocalMove(_originPos, _tweenDuration);
        if (!_selected) _cardOutline.enabled = false;
    }

    public void DoSelectedAnim()
    {
        transform.DOKill();
        _cardOutline.DOColor(_outlineSelectedColor, 0.2f);
    }

    public void DoUnselectAnim()
    {
        transform.DOKill();
        _cardOutline.DOColor(_outlineColor, 0.2f);
    }
}
