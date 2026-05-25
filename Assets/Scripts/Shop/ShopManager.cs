using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Deck _currentDeck;
    [SerializeField] private DeckManager _deckManager;
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
    [SerializeField] private TextMeshProUGUI _playerMoneyTMP;
    private int _curentPearls;

    public event Action<CardInstance, int> OnCardBought;
    public event Action OnDMGUpgradeBought;
    public event Action OnHealthUpgradeBought;
    public event Action OnLandBought;
    [SerializeField] private int _upgradePrice;
    [SerializeField] private int _landPrice;

    private void Start()
    {
        _timer = new Timer(_refreshOffersTime);
        _timer.OnTimerDone += RefreshOffers;
        RefreshOffers();
        UpgradeButtonSetup();
        SetUpLandPurchase();

        _playerController.OnMoneyChanged += UpdateMoneyUI;
        _playerMoneyTMP.text = $"Pearls: {_playerController.Pearls}";
        _curentPearls = _playerController.Pearls;

        OnCardBought += _deckManager.InsertCards;

        OnHealthUpgradeBought += () =>
        {
            _playerController.Battler.RaiseStatLevel(BattlerStats.HP);
            _healthUpgradeButton.interactable = false;
        };

        OnDMGUpgradeBought += () =>
        {
            _playerController.Battler.RaiseStatLevel(BattlerStats.DMG);
            _dmgUpgradeButton.interactable = false;
        };
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

        CardInstance offerCard = _currentDeck.GetRandomCard().Instantiate(_currentDeck.Instantiate());
        int offerCardValue = _cardSettings.GetValueByRank(offerCard.Rank);
        
        int price = CalculatePrice(offerCardValue, offerCardAmount);

        shopOfferManager.OfferSetUp(price, offerCardAmount, offerCard.DisplayImage);

        offerButton.onClick.AddListener(() => Buy(price, 
                                        () =>
                                        {
                                            OnCardBought?.Invoke(offerCard, offerCardAmount);
                                            offerButton.interactable = false;
                                        }, 
                                        null));
    }

    private int CalculatePrice(int givenValue, int givenAmount)
    {
        return givenValue + givenAmount;
    }

    private void UpgradeButtonSetup()
    {
        _dmgUpgradeButton.onClick.AddListener(() => Buy(_upgradePrice, () => OnDMGUpgradeBought?.Invoke(), null));
        _dmgUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Damage Upgrade\n({_upgradePrice}p)";

        _healthUpgradeButton.onClick.AddListener(() => Buy(_upgradePrice, () => OnHealthUpgradeBought?.Invoke(), null));
        _healthUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Health Upgrade\n({_upgradePrice}p)";
    }

    private void SetUpLandPurchase()
    {
        _landPurchaseButton.onClick.AddListener(() => Buy(_landPrice, () => OnLandBought?.Invoke(), null));
    }

    private void Buy(int price, Action onBuy, Action onBrokeAss)
    {
        if (_playerController.Pearls >= price) 
        {
            onBuy?.Invoke();
            _playerController.AlterMoney(-price);
        }
        else 
        {
            onBrokeAss?.Invoke();
            Debug.Log("BROKE");
        }
    }

    private void UpdateMoneyUI(int money)
    {
        if (_updateMoneyCR != null) StopCoroutine(_updateMoneyCR);

        _updateMoneyCR = StartCoroutine(UpdateMoneyCR(money));
    }
    private Coroutine _updateMoneyCR = null;
    private float _waitTime = 0.1f;
    private IEnumerator UpdateMoneyCR(int targetValue)
    {
        while (_curentPearls != targetValue)
        {
            int distance = Mathf.Abs(targetValue - _curentPearls);

            int step = Mathf.Max(1, distance / 5);

            if (_curentPearls < targetValue) _curentPearls += step;
            else if (_curentPearls > targetValue) _curentPearls -= step;

            _playerMoneyTMP.text = "Pearls: " + _curentPearls;
            _playerMoneyTMP.transform.DOScale(Vector3.one * 1.25f, 0.1f).OnComplete(() => _playerMoneyTMP.transform.DOScale(Vector3.one, 0.1f));

            yield return new WaitForSeconds(_waitTime);
        }

        _updateMoneyCR = null;
    }
}
