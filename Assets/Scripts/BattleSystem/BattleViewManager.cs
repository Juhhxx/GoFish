using System;
using UnityEngine;

public class BattleViewManager : MonoBehaviour
{
    [SerializeField] private BattleManager _battleManager;

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

        _battleManager.OnBattleStateChanged += RefreshBattleUI;

        OnCallButtonPressed += () => _battleManager.CallCard();

        _callButton.onClick.AddListener(() => OnCallButtonPressed?.Invoke());
        callEvents.OnPointerEnterEvent.AddListener(() => OnCallButtonHovered?.Invoke(true));
        callEvents.OnPointerExitEvent.AddListener(() => OnCallButtonHovered?.Invoke(false));

        _pexButton.onClick.AddListener(() => OnPeixinhoButtonPressed?.Invoke());
        pexEvents.OnPointerEnterEvent.AddListener(() => OnPeixinhoButtonHovered?.Invoke(true));
        pexEvents.OnPointerExitEvent.AddListener(() => OnPeixinhoButtonHovered?.Invoke(false));

        _halfPexButton.onClick.AddListener(() => OnHalfPeixinhoButtonPressed?.Invoke());
        halfPexEvents.OnPointerEnterEvent.AddListener(() => OnHalfPeixinhoButtonHovered?.Invoke(true));
        halfPexEvents.OnPointerExitEvent.AddListener(() => OnHalfPeixinhoButtonHovered?.Invoke(false));
    }

    private void RefreshBattleUI()
    {
        _callButton.interactable = _battleManager.CanPlayerCall();

        _halfPexButton.interactable = _battleManager.CanPlayerPlayHalfPeixinho();

        _pexButton.interactable =  _battleManager.CanPlayerPlayPeixinho();
    }
}
