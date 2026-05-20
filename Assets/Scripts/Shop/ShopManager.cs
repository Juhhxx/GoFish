using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Deck _currentDeck;
    [SerializeField] private GameObject _cardOfferPrefab;
    [SerializeField] private List<RectTransform> _offerPivot = new List<RectTransform>();
    private List<GameObject> _pastOffers = new List<GameObject>();
    [SerializeField] private UnityEngine.UI.Image _timerFill;
    private Timer _timer;
    [SerializeField] private float _refreshOffersTime;
    [SerializeField] private CardSettings _cardSettings;
    [SerializeField] private UnityEngine.UI.Button _dmgUpgradeButton;
    [SerializeField] private UnityEngine.UI.Button _healthUpgradeButton;
    [SerializeField] private UnityEngine.UI.Button _landPurchaseButton;
    [SerializeField] private PlayerController _playerController;
    public event Action<Card, int> OnCardBought;
    public event Action OnDMGUpgradeBought;
    public event Action OnHealthUpgradeBought;
    public event Action OnLandBought;
    private int _upgradePrice;
    private int _landPrice;
    private void Start()
    {
        _timer = new Timer(_refreshOffersTime);
        _timer.OnTimerDone += RefreshOffers;
        RefreshOffers();
    }

    private void Update()
    {
        _timer.CountTimer();
        _timerFill.fillAmount = _timer.CurrentTime / _refreshOffersTime;
    }

    private void RefreshOffers()
    {
        DestroyPastOffers();
        for (int i = 0; i < _offerPivot.Count; i++)
        {
            GameObject offer = Instantiate(_cardOfferPrefab, _offerPivot[i]);
            _pastOffers.Add(offer);
            CreateNewOffer(offer.GetComponent<ShopOfferManager>(), offer.GetComponent<UnityEngine.UI.Button>());
        }
    }
    
    private void DestroyPastOffers()
    {
        foreach (GameObject pastOffer in _pastOffers)
        {
            Destroy(pastOffer);
        }
        _pastOffers.Clear();
    }

    public void CreateNewOffer(ShopOfferManager shopOfferManager, UnityEngine.UI.Button offerButton)
    {
        int offerCardAmount = UnityEngine.Random.Range(1, 4)* 2;

        Card offerCard = _currentDeck.GetRandomCard();
        int offerCardValue = _cardSettings.GetValueByRank(offerCard.Rank);
        
        int price = CalculatePrice(offerCardValue, offerCardAmount);

        shopOfferManager.OfferSetUp(price, offerCardAmount, offerCard.DisplayImage);

        offerButton.onClick.AddListener(() => Buy(price, () => OnCardBought.Invoke(offerCard, offerCardAmount), null));
    }

    private int CalculatePrice(int givenValue, int givenAmount)
    {
        return givenValue + givenAmount;
    }

    private void UpgradeButtonSetup()
    {
        _dmgUpgradeButton.onClick.AddListener(() => Buy(_upgradePrice, () => OnDMGUpgradeBought.Invoke(), null));
        _healthUpgradeButton.onClick.AddListener(() => Buy(_upgradePrice, () => OnHealthUpgradeBought.Invoke(), null));
    }

    private void SetUpLandPurchase()
    {
        _landPurchaseButton.onClick.AddListener(() => Buy(_landPrice, () => OnLandBought.Invoke(), null));
    }

    private void Buy(int price, Action onBuy, Action onBrokeAss)
    {
        if (_playerController.Pearls >= price) 
        {
            onBuy.Invoke();
            _playerController.AlterMoney(-price);
        }
        else 
        {
            onBrokeAss.Invoke();
            Debug.Log("BROKE");
        }
    }
}
