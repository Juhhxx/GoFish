using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ShopOfferManager : MonoBehaviour
{
    [SerializeField] private Image _offerImage;
    [SerializeField] private Image _offerBackImage;
    [SerializeField] private TextMeshProUGUI _offerAmount;
    [SerializeField] private TextMeshProUGUI _offerPrice;
    [SerializeField] private float _hoverScaleAmount;
    [SerializeField] private float _punchForce;
    [SerializeField] private float _tweenDurations;

    public void OfferSetUp(int givenPrice, int givenAmount, Sprite givenOfferSprite)
    {
        _offerPrice.text = $"{givenPrice}p";
        _offerAmount.text = $"{givenAmount}x";
        _offerImage.sprite = givenOfferSprite;
        _offerBackImage.sprite = givenOfferSprite;
    }

    private void Start()
    {
        DoSpawnOfferAnim();
    }

    // Animations
    public void DoSpawnOfferAnim()
    {
        transform.DOKill();

        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void DoHoveredAnim()
    {
        transform.DOKill();
        transform.DOScale(Vector3.one * _hoverScaleAmount, _tweenDurations);
    }

    public void DoUnhoveredAnim()
    {
        transform.DOKill();
        transform.DOScale(Vector3.one, _tweenDurations);
    }

    public void DoClickedAnim()
    {
        transform.DOKill();
        transform.DOPunchScale(Vector3.one * _punchForce, _tweenDurations).OnComplete(() => DoUnhoveredAnim());
    }
}
