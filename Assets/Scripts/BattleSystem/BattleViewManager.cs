using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleViewManager : MonoBehaviour
{
    [SerializeField] private BattleManager _battleManager;

    [SerializeField] private TextMeshProUGUI _playerHpTmp;
    [SerializeField] private TextMeshProUGUI _playerDmgTmp;
    [SerializeField] private TextMeshProUGUI _playerDmgMultTmp;

    [SerializeField] private TextMeshProUGUI _enemyNameTmp;
    [SerializeField] private TextMeshProUGUI _enemyHpTmp;
    [SerializeField] private Image _enemyHpBar;
    [SerializeField] private TextMeshProUGUI _enemyDmgTmp;
    [SerializeField] private TextMeshProUGUI _enemyDmgMultTmp;


    [SerializeField] private UnityEngine.UI.Button _callButton;
    [SerializeField] private UnityEngine.UI.Button _pexButton;
    [SerializeField] private UnityEngine.UI.Button _halfPexButton;

    public event Action OnCallButtonPressed;
    public event Action<bool> OnCallButtonHovered;

    public event Action OnPeixinhoButtonPressed;
    public event Action<bool> OnPeixinhoButtonHovered;

    public event Action OnHalfPeixinhoButtonPressed;
    public event Action<bool> OnHalfPeixinhoButtonHovered;


    private void Start()
    {
        PointerButtonEvents callEvents = _callButton.GetComponent<PointerButtonEvents>();
        PointerButtonEvents pexEvents = _pexButton.GetComponent<PointerButtonEvents>();
        PointerButtonEvents halfPexEvents = _halfPexButton.GetComponent<PointerButtonEvents>();

        _callButton.interactable = false;
        _pexButton.interactable = false;
        _halfPexButton.interactable = false;

        // UI Buttons
        _battleManager.OnBattleStateChanged += RefreshBattleUI;

        OnCallButtonPressed += () => _battleManager.CallCard();
        OnPeixinhoButtonPressed += () => _battleManager.PlayPeixinho(true);
        OnHalfPeixinhoButtonPressed += () => _battleManager.PlayPeixinho(false);

        _callButton.onClick.AddListener(() => OnCallButtonPressed?.Invoke());
        callEvents.OnPointerEnterEvent.AddListener(() => OnCallButtonHovered?.Invoke(true));
        callEvents.OnPointerExitEvent.AddListener(() => OnCallButtonHovered?.Invoke(false));

        _pexButton.onClick.AddListener(() => OnPeixinhoButtonPressed?.Invoke());
        pexEvents.OnPointerEnterEvent.AddListener(() => OnPeixinhoButtonHovered?.Invoke(true));
        pexEvents.OnPointerExitEvent.AddListener(() => OnPeixinhoButtonHovered?.Invoke(false));

        _halfPexButton.onClick.AddListener(() => OnHalfPeixinhoButtonPressed?.Invoke());
        halfPexEvents.OnPointerEnterEvent.AddListener(() => OnHalfPeixinhoButtonHovered?.Invoke(true));
        halfPexEvents.OnPointerExitEvent.AddListener(() => OnHalfPeixinhoButtonHovered?.Invoke(false));

        // UI Values
        _playerHpTmp.text = _battleManager.Player.CurrentHp + "/" + _battleManager.Player.MaxHP;
        _playerDmgTmp.text = _battleManager.Player.Damage.ToString();
        _playerDmgMultTmp.text = $"{_battleManager.Player.Mult:f0}";

        _battleManager.Player.OnHPChanged += (hp, max) => UpdateValue(_playerHpTmp, hp, $"/{max}");
        _battleManager.Player.OnDamageChanged += (dmg) => UpdateValue(_playerDmgTmp, dmg);
        _battleManager.Player.OnMultChanged += (mult) => UpdateValue(_playerDmgMultTmp, mult);

        _enemyNameTmp.text = _battleManager.Enemy.DisplayName;
        _enemyHpTmp.text = _battleManager.Enemy.CurrentHp + "/" + _battleManager.Enemy.MaxHP;
        _enemyDmgTmp.text = _battleManager.Enemy.Damage.ToString();
        _enemyDmgMultTmp.text = $"{_battleManager.Enemy.Mult:f0}";

        _battleManager.Enemy.OnHPChanged += (hp, max) => UpdateValue(_enemyHpTmp, hp, $"/{max}");
        _battleManager.Enemy.OnHPChanged += (hp, max) => UpdateBar(_enemyHpBar, hp, max);
        _battleManager.Enemy.OnDamageChanged += (dmg) => UpdateValue(_enemyDmgTmp, dmg);
        _battleManager.Enemy.OnMultChanged += (mult) => UpdateValue(_enemyDmgMultTmp, mult);
    }

    private void RefreshBattleUI()
    {
        _callButton.interactable = _battleManager.CanPlayerCall();

        _halfPexButton.interactable = _battleManager.CanPlayerPlayHalfPeixinho();

        _pexButton.interactable =  _battleManager.CanPlayerPlayPeixinho();
    }

    private void UpdateValue(TextMeshProUGUI tmp, int value, string suffix = "")
    {
        tmp.text = value.ToString() + suffix;
        tmp.transform.DOScale(Vector3.one * 1.25f, 0.1f).OnComplete(() => tmp.transform.DOScale(Vector3.one, 0.1f));
    }
    private void UpdateValue(TextMeshProUGUI tmp, float value, string suffix = "")
    {
        if (value % 2 == 0) tmp.text = $"{value:f0}" + suffix;
        else tmp.text = $"{value:f2}" + suffix;
        
        tmp.transform.DOScale(Vector3.one * 1.25f, 0.1f).OnComplete(() => tmp.transform.DOScale(Vector3.one, 0.1f));
    }

    private void UpdateBar(Image image, float current, float max)
    {
        image.DOFillAmount(current/max, 0.3f);
    }
}
