using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
}
