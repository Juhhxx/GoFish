using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Deck _currentDeck;
    [SerializeField] private GameObject _cardOfferPrefab;
    [SerializeField] private List<RectTransform> _offerPivot = new List<RectTransform>();
    [SerializeField] private Timer _timer;
    [SerializeField] private float _refreshOffersTime;
    [SerializeField] private CardSettings _cardSettings;
    public event Action<Card, int, int> OnCardBought;
    private void Start()
    {
        RefreshOffers();
    }
    private void RefreshOffers()
    {
        for (int i = 0; i < _offerPivot.Count; i++)
        {
            GameObject offer = Instantiate(_cardOfferPrefab, _offerPivot[i]);
            CreateNewOffer(offer.GetComponent<ShopOfferManager>(), offer.GetComponent<UnityEngine.UI.Button>());
        }
    }
    public void CreateNewOffer(ShopOfferManager shopOfferManager, UnityEngine.UI.Button offerButton)
    {
        int offerCardAmount = UnityEngine.Random.Range(1, 4)* 2;

        Card offerCard = _currentDeck.GetRandomCard();
        int offerCardValue = _cardSettings.GetValueByRank(offerCard.Rank);
        
        int price = CalculatePrice(offerCardValue, offerCardAmount);

        shopOfferManager.OfferSetUp(price, offerCardAmount, offerCard.DisplayImage);

        offerButton.onClick.AddListener(() => OnCardBought.Invoke(offerCard, offerCardAmount, price));
    }

    private int CalculatePrice(int givenValue, int givenAmount)
    {
        return givenValue + givenAmount;
    }
}
