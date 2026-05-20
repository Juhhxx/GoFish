using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private HandManager _playerHand;
    // Temporary
    [SerializeField] private UnityEngine.UI.Button _callButton;
    [SerializeField] private UnityEngine.UI.Button _pexButton;
    [SerializeField] private UnityEngine.UI.Button _halfPexButton;

    private void Start()
    {
        _playerHand.OnPeixinhoUpdate += UpdatePeixinhoButtons;
        _playerHand.OnCallUpdate += UpdateCallButton;

        _callButton.interactable = false;
        _pexButton.interactable = false;
        _halfPexButton.interactable = false;
    }

    private void UpdatePeixinhoButtons(bool hasPeixinho, bool isFull)
    {
        Debug.Log($"Has Peixinho: {hasPeixinho}");
        Debug.Log($"Is Full: {isFull}");

        _halfPexButton.interactable = hasPeixinho && !isFull;
        _pexButton.interactable = isFull;
    }

    private void UpdateCallButton(bool canCall, Rank rank)
    {
        Debug.Log($"Can Call: {canCall}");
        Debug.Log($"Rank: {rank}");

        _callButton.interactable = canCall;
    }
}
