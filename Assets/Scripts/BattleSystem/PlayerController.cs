using NaughtyAttributes;
using UnityEngine;

public class PlayerController : BattlerController
{
    [SerializeField] private int _pearls;
    public int Pearls => _pearls;

    [SerializeField] private int _landBought;
    public int LandBought => _landBought;

    [Button]
    private void GetMoneyCheat()
    {
        _pearls += 999;
    }
    [Button]
    private void MoneyBeGone()
    {
        _pearls = 0;
    }

    public void AlterMoney(int amount)
    {
        _pearls += amount;
    }

    private new void Start()
    {
        base.Start();
    }
}
