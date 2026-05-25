using UnityEngine;
using UnityEngine.XR;

public abstract class BattlerController : MonoBehaviour
{
    [SerializeField] private Battler _battler;
    [SerializeField] private HandManager _handManager;
    public HandManager HandManager => _handManager;

    [SerializeField] private BattlerInstance _battlerInst;
    public BattlerInstance Battler => _battlerInst;

    public void Awake()
    {
        _battlerInst = _battler.Instantiate();
    }

    public void Damage(BattlerInstance battler, bool isFull)
    {
        int damage = isFull ? battler.GetFinalDamage() : battler.GetFinalDamage() / 2;

        damage = Mathf.Min(1, damage);

        Battler.CurrentHp -= damage;
    }

    public void Heal(BattlerInstance battler, bool isFull)
    {
        int heal = isFull ? battler.GetFinalDamage() : battler.GetFinalDamage() / 2;

        heal = Mathf.Min(1, heal);

        Battler.CurrentHp += heal;
    }

    public void AddMult(float quantity, bool isFull)
    {
        if (!isFull) quantity /= 2;

        Battler.AddMult(quantity);
    }

    public void MultiplyMult(float quantity, bool isFull)
    {
        if (!isFull) quantity /= 2;

        if (quantity < 0) quantity += 1;

        Battler.MultiplyMult(quantity);
    }

}
