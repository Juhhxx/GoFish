using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Battler", menuName = "Battle/Battler")]
public class Battler : ScriptableObject
{
    [field: SerializeField] public string DisplayName;
    [field: SerializeField] public int HealthPoints;
    [field: SerializeField] public int Damage;
    [field: SerializeField] public int UpgradeBonus = 5;
    [field: SerializeField] public int HPLevel;
    [field: SerializeField] public int DMGLevel;

    public BattlerInstance Instantiate() => new BattlerInstance(DisplayName, HealthPoints, Damage, UpgradeBonus, HPLevel, DMGLevel);
}

[Serializable]
public class BattlerInstance
{
    public BattlerInstance(string name, int baseHp, int baseDmg, int upgradeBonus, int levelHp = 0, int levelDmg = 0)
    {
        _name = name;

        _baseHp = baseHp;
        _currentHP = baseHp;
        
        _baseDamage = baseDmg;

        _levelUpgradeBonus = upgradeBonus;

        _hpLevel = levelHp;
        _dmgLevel = levelDmg;
    }

    [SerializeField] private string _name;
    public string DisplayName => _name;

    [SerializeField] private int _baseHp;
    public int MaxHP => _baseHp + GetStatUpgradeBonus(BattlerStats.HP);
    [SerializeField] private int _hpLevel = 0;

    private int _currentHP;
    public int CurrentHp
    {
        get => _currentHP;

        set
        {
            var tmp = 0;

            if (value > MaxHP) tmp = MaxHP;
            else if (value < 0) tmp = 0;
            else tmp = value;

            if (_currentHP != tmp) OnHPChanged?.Invoke(tmp, MaxHP);

            _currentHP = tmp;

            if (_currentHP == 0) OnDead?.Invoke();
        }
    }

    public event Action<int, int> OnHPChanged;
    public event Action OnDead;

    [SerializeField] private int _baseDamage;
    public int Damage => _baseDamage + GetStatUpgradeBonus(BattlerStats.DMG);
    [SerializeField] private int _dmgLevel = 0;
    public event Action <int> OnDamageChanged;


    [SerializeField] private float _mult = 1;
    public float Mult => _mult;
    public event Action<float> OnMultChanged;

    public void AddMult(float value)
    {
        _mult += value;

        OnMultChanged?.Invoke(_mult);
    }
    public void MultiplyMult(float value)
    {
        _mult *= value;

        OnMultChanged?.Invoke(_mult);
    }

    public int GetFinalDamage()
    {
        Debug.Log($"Final Damage: {(int)(Damage * Mult)} ({(Damage * Mult)})");
        return (int)(Damage * Mult);
    }

    public int Level => _hpLevel + _dmgLevel;

    public void RaiseStatLevel(BattlerStats stat)
    {
        switch (stat)
        {
            case BattlerStats.HP:
                _hpLevel += 1;
                OnHPChanged?.Invoke(_currentHP, MaxHP);
                break;
            
            case BattlerStats.DMG:
                _dmgLevel += 1;
                OnDamageChanged?.Invoke(Damage);
                break;
        }
    }

    private int _levelUpgradeBonus = 5;
    private int GetStatUpgradeBonus(BattlerStats stat)
    {
        int bonus = 0;

        switch (stat)
        {
            case BattlerStats.HP:
                bonus = _hpLevel * _levelUpgradeBonus;
                break;
            
            case BattlerStats.DMG:
                bonus = _dmgLevel * _levelUpgradeBonus;
                break;
        }

        Debug.Log($"Bonus for {stat} : {bonus}");
        return bonus;
    }
}

public enum BattlerStats
{
    HP,
    DMG,
}