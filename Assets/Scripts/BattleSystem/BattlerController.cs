using UnityEngine;

public class BattlerController : MonoBehaviour
{
    [SerializeField] private Battler _battler;
    [SerializeField] private int _startingHpLevel;
    [SerializeField] private int _startingDmgLevel;

    private BattlerInstance _battlerInst;
    public BattlerInstance Battler => _battlerInst;

    public void Start()
    {
        _battlerInst = _battler.Instantiate(_startingHpLevel, _startingDmgLevel);
    }
}
