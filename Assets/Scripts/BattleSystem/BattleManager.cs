using System;
using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private EnemyController _enemyController;

    [SerializeField] private Deck _deck;
    [SerializeField] private CardSettings _cardSettings;

    [SerializeField] private DeckManager _deckManager;

    private BattleState _currentState = BattleState.Start;

    public enum BattleState
    {
        Start,
        PlayerCall,
        PlayerFish,
        EnemyTurn,
    }

    public event Action OnBattleStateChanged;
    public event Action OnPlayerFished;
    public event Action<bool> OnPlayerCall;

    private int _currentTurn = 0;
    public bool IsPlayerTurn()
    {
        // Even turns are player turn, odd turns are enemy turns
        return _currentTurn % 2 == 0;
    }

    private void Start()
    {
        _playerController.HandManager.OnHandSelectionChanged += (_,_) => RefreshBattleState();

        StartBattle();
    }

    private void RefreshBattleState()
    {
        OnBattleStateChanged?.Invoke();
    }

    public void StartBattle()
    {
        StartCoroutine(BattleLoop());
    }

    private int _initialCards = 4;
    private IEnumerator GiveInitialCardsCR()
    {
        for (int i = 0; i < _initialCards * 2; i++)
        {
            bool toPlayer = i % 2 == 0;

            HandManager hand = toPlayer ? _playerController.HandManager : _enemyController.HandManager;

            _deckManager.GiveCard(hand, toPlayer);

            yield return new WaitForSeconds(0.2f);
        }
    }

    public bool CanPlayerCall()
    {
        return _playerController.HandManager.CanCall() && IsPlayerTurn() && _currentState != BattleState.PlayerFish;
    }
    public bool CanPlayerPlayHalfPeixinho()
    {
        return _playerController.HandManager.HasHalfPeixinho() && IsPlayerTurn() && _currentState != BattleState.PlayerFish;
    }
    public bool CanPlayerPlayPeixinho()
    {
        return _playerController.HandManager.HasPeixinho() && IsPlayerTurn() && _currentState != BattleState.PlayerFish;
    }

    private bool _playerHasCalled = false;
    private Rank _playerCalledRank;
    public void CallCard()
    {
        if (!CanPlayerCall())
            return;
        
        _playerCalledRank = _playerController.HandManager.GetCallRank();
        _playerHasCalled = true;
        _playerController.HandManager.ClearSelection();
    }
    private bool ResolveCall(HandManager caller, HandManager target, Rank rank)
    {
        var cards = target.GetCardsOfRank(rank);

        if (cards.Count > 0)
        {
            foreach (CardInstance card in cards)
            {
                target.RemoveCard(card);
                caller.AddCard(card);
            }

            Debug.Log("Successful call!");
            return true;
        }
        else
        {
            Debug.Log("Go Fish!");
            return false;
        }
    }

    public void PlayPeixinho(bool isFull)
    {
        StartCoroutine(PlayPeixinhoCR());
        DoRankHability(_playerController, _enemyController, _playerController.HandManager.GetPeixinhoRank(), isFull);
    }
    private IEnumerator PlayPeixinhoCR()
    {
        foreach (CardInstance card in _playerController.HandManager.GetSelectedCards())
        {
            _playerController.HandManager.RemoveCard(card);
            yield return new WaitForSeconds(0.5f);
        }
        _playerController.HandManager.ClearSelection();
    }

    private bool _playerHasFished;
    private Rank _playerFishedRank = Rank.None;
    public void PlayerGoFish()
    {
        if (!_deckManager.CanInteract) return;

        _playerHasFished = true;
        _playerFishedRank = _deckManager.GiveCard(_playerController.HandManager, true);
    }

    private IEnumerator BattleLoop()
    {
        _deckManager.InitializeDeck(_deck);
        _deckManager.ToggleDeck(false);


        yield return new WaitUntil(() => _deckManager.IsReady);
        yield return new WaitForSeconds(0.5f);

        yield return GiveInitialCardsCR();

        while(true)
        {
            if (IsPlayerTurn())
            {
                Debug.Log("PlayerTurn");
                yield return PlayerTurn();
                Debug.Log($"Player : {_playerController.Battler.CurrentHp}, {_playerController.Battler.Damage} x {_playerController.Battler.Mult} = {_playerController.Battler.GetFinalDamage()}");
                Debug.Log($"Enemy : {_enemyController.Battler.CurrentHp}, {_enemyController.Battler.Damage} x {_enemyController.Battler.Mult} = {_enemyController.Battler.GetFinalDamage()}");
            }
            else
            {
                Debug.Log("EnemyTurn");
                yield return EnemyTurn();
                Debug.Log($"Player : {_playerController.Battler.CurrentHp}, {_playerController.Battler.Damage} x {_playerController.Battler.Mult} = {_playerController.Battler.GetFinalDamage()}");
                Debug.Log($"Enemy : {_enemyController.Battler.CurrentHp}, {_enemyController.Battler.Damage} x {_enemyController.Battler.Mult} = {_enemyController.Battler.GetFinalDamage()}");
            }
        }
    }

    private IEnumerator PlayerTurn()
    {
        bool keepPlaying = true;

        while (keepPlaying)
        {
            _currentState = BattleState.PlayerCall;

            _playerHasCalled = false;

            yield return new WaitUntil(() => _playerHasCalled);

            Debug.Log($"PLAYER CALLED : {_playerCalledRank}");

            bool success = ResolveCall(_playerController.HandManager, _enemyController.HandManager, _playerCalledRank);

            yield return new WaitForSeconds(0.5f);

            if (!success)
            {
                _currentState = BattleState.PlayerFish;

                _playerHasFished = false;

                _deckManager.ToggleDeck(true);

                yield return new WaitUntil(() => _playerHasFished);

                Debug.Log($"{_playerCalledRank} == {_playerFishedRank} ? {_playerCalledRank == _playerFishedRank}");

                _deckManager.ToggleDeck(false);

                bool fishedRequestedRank = _playerFishedRank == _playerCalledRank;

                keepPlaying = fishedRequestedRank;
            }
        }

        _currentTurn++;
    }

    private IEnumerator EnemyTurn()
    {
        _currentState = BattleState.EnemyTurn;

        yield return new WaitForSeconds(2f);

        bool keepPlaying = true;

        while (keepPlaying)
        {
            Rank rank = ChooseEnemyRank();

            Debug.Log($"ENEMY CALLED : {rank}");

            bool success = ResolveCall(_enemyController.HandManager, _playerController.HandManager, rank);

            yield return new WaitForSeconds(1f);

            if (!success)
            {
                Rank drawnRank = _deckManager.GiveCard(_enemyController.HandManager, false);

                Debug.Log($"ENEMY : {rank} == {drawnRank} ? {rank == drawnRank}");

                yield return new WaitForSeconds(1f);

                if (drawnRank != rank)
                {
                    keepPlaying = false;
                }
            }
        }

        _currentTurn++;
    }

    // Enemy Decisions (Temporary)
    private Rank ChooseEnemyRank()
    {
        return _enemyController.HandManager.Hand[UnityEngine.Random.Range(0, _enemyController.HandManager.Hand.Count)].Rank;
    }
    
    // Peixinho Actions
    private void DoRankHability(BattlerController caller, BattlerController target, Rank rank, bool isFull)
    {
        Hability hability = _cardSettings.GetHabilityByRank(rank);

        switch (hability)
        {
            case Hability.Attack:
                target.Damage(caller.Battler, isFull);
                break;
            
            case Hability.Heal:
                caller.Heal(target.Battler, isFull);
                break;
            
            case Hability.MoneyGain:
                if (caller is PlayerController player) player.AlterMoney(player.PearGain, isFull);
                break;

            case Hability.BigMoneyGain:
                if (caller is PlayerController playerB) playerB.AlterMoney(playerB.BigPearGain, isFull);
                break;

            case Hability.MultAdd:
                caller.AddMult(1, isFull);
                break;
            
            case Hability.MultMult:
                caller.MultiplyMult(0.5f, isFull);
                break;
            
            default:
                break;
        }
    }
}
