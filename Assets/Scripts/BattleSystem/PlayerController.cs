using NaughtyAttributes;
using UnityEngine;

public class PlayerController : BattlerController
{
    [SerializeField] private int _pearls;
    public int Pearls => _pearls;

    [SerializeField] private int _basePearlGain;
    public int PearGain => _basePearlGain + (5 * _landBought);

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

    public void AlterMoney(int amount, bool isFull = true)
    {
        if (!isFull) amount /= 2;

        _pearls += amount;
    }
}
