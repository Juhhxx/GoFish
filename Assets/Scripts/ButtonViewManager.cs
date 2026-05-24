using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonViewManager : MonoBehaviour
{
    [SerializeField] private Image _outline;
    [SerializeField] private float _hoverScaleAmount;
    [SerializeField] private float _punchForce;
    [SerializeField] private Color _outlineHoveredColor;
    [SerializeField] private Color _outlineUnhoveredColor;
    [SerializeField] private float _tweenDurations;

    // Animations
    public void DoHoveredAnim()
    {
        transform.DOKill();
        _outline?.DOKill();
        transform.DOScale(Vector3.one * _hoverScaleAmount, _tweenDurations);
        _outline.DOColor(_outlineHoveredColor, _tweenDurations);
    }

    public void DoUnhoveredAnim()
    {
        transform.DOKill();
        _outline?.DOKill();
        transform.DOScale(Vector3.one, _tweenDurations);
        _outline?.DOColor(_outlineUnhoveredColor, _tweenDurations);
    }

    public void DoClickedAnim()
    {
        transform.DOKill();
        transform.DOPunchScale(Vector3.one * _punchForce, _tweenDurations).OnComplete(() => DoUnhoveredAnim());
    }
}
