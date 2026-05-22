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
    private int _hpLevel = 0;

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

            if (_currentHP != tmp) OnHPChanged?.Invoke(tmp);

            _currentHP = tmp;

            if (_currentHP == 0) OnDead?.Invoke();
        }
    }

    public event Action<int> OnHPChanged;
    public event Action OnDead;

    [SerializeField] private int _baseDamage;
    public int Damage => _baseDamage + GetStatUpgradeBonus(BattlerStats.DMG);
    private int _dmgLevel = 0;


    [SerializeField] private float _mult = 1;
    public float Mult => _mult;

    public void AddMult(int value)
    {
        _mult += value;
    }
    public void MultiplyMult(float value)
    {
        _mult *= value;
    }

    public int GetFinalDamage() => (int)(Damage * Mult);


    public int Level => _hpLevel + _dmgLevel;

    public void RaiseStatLevel(BattlerStats stat)
    {
        switch (stat)
        {
            case BattlerStats.HP:
                _hpLevel += 1;
                break;
            
            case BattlerStats.DMG:
                _dmgLevel += 1;
                break;
        }
    }

    private int _levelUpgradeBonus = 5;
    private int GetStatUpgradeBonus(BattlerStats stat)
    {
        switch (stat)
        {
            case BattlerStats.HP:
                return _hpLevel * _levelUpgradeBonus;
            
            case BattlerStats.DMG:
                return _dmgLevel * _levelUpgradeBonus;
            
            default:
                return 0;
        }
    }
}

public enum BattlerStats
{
    HP,
    DMG,
}