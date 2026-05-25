using System;
using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class EnemySpeechBubbleManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _rectTrans;
    [SerializeField] private TextMeshProUGUI _speechTmp;
    [SerializeField] private float _tweenSpeed;
    [SerializeField] private float _textSpeed;

    public void ToggleBubble(string text)
    {
        _speechTmp.text = "";
        StopAllCoroutines();
        if (text == "") DoHideBubbleAnim();
        else DoShowBubbleAnim(() => ShowText(text));
    }

    private void DoShowBubbleAnim(Action onEnd = null)
    {
        _speechTmp.alpha = 1f;
        _rectTrans.DOKill();
        _canvasGroup.DOKill();
        _speechTmp.DOKill();

        _rectTrans.DOScaleX(1f, _tweenSpeed).OnComplete(() => onEnd?.Invoke());
        _canvasGroup.DOFade(1f, _tweenSpeed);
    }

    private void DoHideBubbleAnim()
    {
        _rectTrans.DOKill();
        _canvasGroup.DOKill();
        _speechTmp.DOKill();

        _rectTrans.DOScaleX(0f, _tweenSpeed);
        _canvasGroup.DOFade(0f, _tweenSpeed);
        _speechTmp.DOFade(0f, _tweenSpeed * 0.25f);
    }

    private void ShowText(string text)
    {
        StartCoroutine(ShowTextCR(text));
    }

    private IEnumerator ShowTextCR(string text)
    {
        _speechTmp.text = text;

        // Force TMP to generate text info
        _speechTmp.ForceMeshUpdate();

        int totalVisibleCharacters = _speechTmp.textInfo.characterCount;

        _speechTmp.maxVisibleCharacters = 0;

        while (_speechTmp.maxVisibleCharacters < totalVisibleCharacters)
        {
            _speechTmp.maxVisibleCharacters++;

            yield return new WaitForSeconds(_textSpeed);
        }
    }
}
