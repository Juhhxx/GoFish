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

    [SerializeField] private float _callScaleAmount;
    [SerializeField] private Vector3 _callMoveAmount;

    [SerializeField] private Vector3 _peixinhoShakeAmount;

    [SerializeField] private float _tweenHoverDuration;
    [SerializeField] private float _tweenDeckSpawnDuration;
    [SerializeField] private float _tweenOutlineChangeDuration = 0.2f;
    [SerializeField] private float _tweenCallDuration = 0.2f;

    private Vector3 _originPos;
    private float _originRot;
    private bool _selected = false;
    public bool Selected => _selected;

    public CardInstance Card { get; private set; }

    public void Start()
    {
        // _originPos = transform.position;
    }

    public void SetOrigin(Vector3 pos, float rot)
    {
        _originPos = pos;
        _originRot = rot;
    }

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
        transform.DOScale(Vector3.one * _hoverScaleAmount, _tweenHoverDuration);
        transform.DOLocalMove(_originPos + _hoverMoveAmount, _tweenHoverDuration);
        _cardOutline.enabled = true;
    }

    public void DoCardUnhoverAnim()
    {
        transform.DOKill();
        transform.DOScale(Vector3.one, _tweenHoverDuration);
        transform.DOLocalMove(_originPos, _tweenHoverDuration);
        if (!_selected) _cardOutline.enabled = false;
    }

    public void DoSelectedAnim()
    {
        _cardOutline.DOColor(_outlineSelectedColor, 0.2f);
    }

    public void DoUnselectAnim()
    {
        _cardOutline.DOColor(_outlineColor, 0.2f);
    }

    public void DoCallAnim()
    {
        transform.DOKill();
        transform.DOScale(Vector3.one * _callScaleAmount, _tweenHoverDuration);
        transform.DOLocalMove(_originPos + _callMoveAmount, _tweenHoverDuration);
    }

    public void DoPeixinhoAnim()
    {
        transform.DOKill();
        transform.DOShakeRotation(0.2f, _peixinhoShakeAmount, randomness: 50).SetLoops(-1);
        transform.DOScale(Vector3.one * _callScaleAmount, _tweenHoverDuration);
    }

    public void DoStopAnimations(bool toggleOutline = false)
    {
        transform.DOKill();
        transform.DOLocalMove(_originPos, 0.2f);
        transform.DOScale(Vector3.one, 0.2f);
        transform.DOLocalRotate(new Vector3(0f, 0f, _originRot), 0.2f);
        if (toggleOutline) _cardOutline.enabled = false;
    }
}
