using NaughtyAttributes;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int _pearls;
    public int Pearls => _pearls;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AlterMoney(int amount)
    {
        _pearls += amount;
    }
}
