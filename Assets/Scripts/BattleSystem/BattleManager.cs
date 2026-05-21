using System;
using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private Deck _deck;
    [SerializeField] private CardSettings _cardSettings;

    [SerializeField] private DeckManager _deckManager;
    [SerializeField] private HandManager _playerHand;
    [SerializeField] private HandManager _enemyHand;

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
        _playerHand.OnHandSelectionChanged += (_,_) => RefreshBattleState();

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

            HandManager hand = toPlayer ? _playerHand : _enemyHand;

            _deckManager.GiveCard(hand, toPlayer);

            yield return new WaitForSeconds(0.2f);
        }
    }

    public bool CanPlayerCall()
    {
        return _playerHand.CanCall() && IsPlayerTurn() && _currentState != BattleState.PlayerFish;
    }
    public bool CanPlayerPlayHalfPeixinho()
    {
        return _playerHand.HasHalfPeixinho() && IsPlayerTurn() && _currentState != BattleState.PlayerFish;
    }
    public bool CanPlayerPlayPeixinho()
    {
        return _playerHand.HasPeixinho() && IsPlayerTurn() && _currentState != BattleState.PlayerFish;
    }

    private bool _playerHasCalled = false;
    private Rank _playerCalledRank;
    public void CallCard()
    {
        if (!CanPlayerCall())
            return;
        
        _playerCalledRank = _playerHand.GetCallRank();
        _playerHasCalled = true;
        _playerHand.ClearSelection();
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
        foreach (CardInstance card in _playerHand.GetSelectedCards())
        {
            _playerHand.RemoveCard(card);
        }
    }

    private bool _playerHasFished;
    private Rank _playerFishedRank = Rank.None;
    public void PlayerGoFish()
    {
        if (!_deckManager.CanInteract) return;

        _playerHasFished = true;
        _playerFishedRank = _deckManager.GiveCard(_playerHand, true);
    }

    private IEnumerator BattleLoop()
    {
        _deckManager.InitializeDeck(_deck);

        yield return new WaitUntil(() => _deckManager.IsReady);
        yield return new WaitForSeconds(0.5f);

        yield return GiveInitialCardsCR();

        while(true)
        {
            if (IsPlayerTurn())
            {
                Debug.Log("PlayerTurn");
                yield return PlayerTurn();
            }
            else
            {
                Debug.Log("EnemyTurn");
                yield return EnemyTurn();
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

            bool success = ResolveCall(_playerHand, _enemyHand, _playerCalledRank);

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

        yield return new WaitForSeconds(1f);

        bool keepPlaying = true;

        while (keepPlaying)
        {
            Rank rank = ChooseEnemyRank();

            Debug.Log($"ENEMY CALLED : {rank}");

            bool success = ResolveCall(_enemyHand, _playerHand, rank);

            yield return new WaitForSeconds(0.5f);

            if (!success)
            {
                Rank drawnRank = _deckManager.GiveCard(_enemyHand, false);

                Debug.Log($"ENEMY : {rank} == {drawnRank} ? {rank == drawnRank}");

                yield return new WaitForSeconds(0.5f);

                if (drawnRank != rank)
                {
                    keepPlaying = false;
                }
            }
        }

        _currentTurn++;
    }
    private Rank ChooseEnemyRank()
    {
        return _enemyHand.Hand[UnityEngine.Random.Range(0, _enemyHand.Hand.Count)].Rank;
    }
    
}
