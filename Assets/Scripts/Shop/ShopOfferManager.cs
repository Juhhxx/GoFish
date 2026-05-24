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
}
