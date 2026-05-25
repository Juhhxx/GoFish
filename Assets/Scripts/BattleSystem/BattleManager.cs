using System;
using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    public BattlerInstance Player => _playerController.Battler;

    [SerializeField] private EnemyController _enemyController;
    public BattlerInstance Enemy => _enemyController.Battler;


    [SerializeField] private Deck _deck;
    [SerializeField] private CardSettings _cardSettings;

    [SerializeField] private DeckManager _deckManager;
    [SerializeField] private ShopManager _shopManager;

    [SerializeField] private KeymashManager _keymashMinigame;
    [SerializeField] private string _revoltWord = "REVOLT";
    [SerializeField] private string _capitalWord = "CAPITAL";

    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;

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
    public event Action<string> OnEnemyAction;

    private int _currentTurn = 0;
    public bool IsPlayerTurn()
    {
        // Even turns are player turn, odd turns are enemy turns
        return _currentTurn % 2 == 0;
    }

    private void Start()
    {
        _playerController.HandManager.OnHandSelectionChanged += (_,_) => RefreshBattleState();
        _playerController.Battler.OnDead += () =>
                                        {
                                            _playerDead = true;
                                            BattleEnd();
                                        };
        _enemyController.Battler.OnDead += () =>
                                        {
                                            _enemyDead = true;
                                            BattleEnd();
                                        };
        _shopManager.OnLandBought += () =>
                                        {
                                            _landBought = true;
                                            BattleEnd();
                                        };
                                        
        _keymashMinigame.OnGameEnd += (playerWon) =>
                                        {
                                            if (playerWon) _winScreen.SetActive(true);
                                            else _loseScreen.SetActive(true);
                                        };

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
    private IEnumerator RefilCardsCR(bool toPlayer)
    {
        for (int i = 0; i < _initialCards; i++)
        {
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
        StartCoroutine(PlayPeixinhoCR(true));
        DoRankHability(_playerController, _enemyController, _playerController.HandManager.GetPeixinhoRank(), isFull);
    }
    private IEnumerator PlayPeixinhoCR(bool isPlayer)
    {
        BattlerController controller = isPlayer ? _playerController : _enemyController;

        foreach (CardInstance card in controller.HandManager.GetSelectedCards())
        {
            controller.HandManager.RemoveCard(card);
            yield return new WaitForSeconds(0.5f);
        }
        controller.HandManager.ClearSelection();
        
        if (isPlayer)
        {
            OnEnemyAction?.Invoke($"Interesting play...");
            yield return new WaitForSeconds(2.5f);
            OnEnemyAction?.Invoke("");
        }
    }

    private bool _playerHasFished;
    private Rank _playerFishedRank = Rank.None;
    public void PlayerGoFish()
    {
        if (!_deckManager.CanInteract) return;

        _playerHasFished = true;
        _playerFishedRank = _deckManager.GiveCard(_playerController.HandManager, true);
        OnEnemyAction?.Invoke($"");
    }

    private bool _playerDead = false;
    private bool _enemyDead = false;
    private bool _landBought = false;

    private IEnumerator BattleLoop()
    {
        _playerDead = false;
        _enemyDead = false;
        _landBought = false;

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
                OnEnemyAction?.Invoke($"Your turn...");
                yield return new WaitForSeconds(1.5f);
                OnEnemyAction?.Invoke("");
                yield return PlayerTurn();
            }
            else
            {
                Debug.Log("EnemyTurn");
                OnEnemyAction?.Invoke($"My turn.");
                yield return new WaitForSeconds(1.5f);
                OnEnemyAction?.Invoke("");
                yield return EnemyTurn();
            }

            if (_deckManager.Deck.Cards.Count == 0) break;
        }

        BattleEnd();
    }

    private void BattleEnd()
    {
        if (_landBought) StartCoroutine(EndMessageCR("Pleasure doing business.", () => _keymashMinigame.StartMinigame(_capitalWord)));
        else if (_enemyDead) StartCoroutine(EndMessageCR("We'll see about that.", () => _keymashMinigame.StartMinigame(_revoltWord)));
        else if (_playerDead) StartCoroutine(EndMessageCR("A pitiful attempt.", () => _loseScreen.SetActive(true)));
        else if (_deckManager.Deck.Cards.Count == 0) StartCoroutine(EndMessageCR("When in the ocean,\ndo as the animals do.", () => _keymashMinigame.StartMinigame(_revoltWord)));
    }

    private IEnumerator EndMessageCR(string message, Action onEnd)
    {
        OnEnemyAction?.Invoke(message);
        yield return new WaitForSeconds(2f);
        onEnd?.Invoke();
    }

    private IEnumerator PlayerTurn()
    {
        RefreshBattleState();
        bool keepPlaying = true;

        if (_playerController.HandManager.Hand.Count == 0)
        {
            if (_deckManager.Deck.Cards.Count == 0) yield break;

            yield return RefilCardsCR(true);
        }

        while (keepPlaying)
        {
            if (_playerController.HandManager.Hand.Count > 0)
            {
                _currentState = BattleState.PlayerCall;

                _playerHasCalled = false;

                yield return new WaitUntil(() => _playerHasCalled || _playerController.HandManager.Hand.Count == 0);

                Debug.Log($"PLAYER CALLED : {_playerCalledRank}");

                _enemyController.AddRankMemory(_playerCalledRank);

                bool success = ResolveCall(_playerController.HandManager, _enemyController.HandManager, _playerCalledRank);

                yield return new WaitForSeconds(0.5f);

                if (!success || _deckManager.Deck.Cards.Count == 0)
                {
                    OnEnemyAction?.Invoke($"Go fish.");

                    _currentState = BattleState.PlayerFish;

                    _playerHasFished = false;

                    _deckManager.ToggleDeck(true);

                    yield return new WaitUntil(() => _playerHasFished);

                    Debug.Log($"{_playerCalledRank} == {_playerFishedRank} ? {_playerCalledRank == _playerFishedRank}");

                    _deckManager.ToggleDeck(false);

                    bool fishedRequestedRank = _playerFishedRank == _playerCalledRank;

                    keepPlaying = fishedRequestedRank;

                    if (keepPlaying)
                    {
                        OnEnemyAction?.Invoke($"Tsk...");
                        yield return new WaitForSeconds(2.5f);
                        OnEnemyAction?.Invoke("");
                    }
                }
            }
            else keepPlaying = false;
        }

        _currentTurn++;
    }

    private IEnumerator EnemyTurn()
    {
        _currentState = BattleState.EnemyTurn;

        yield return new WaitForSeconds(2f);

        if (_enemyController.HandManager.Hand.Count == 0)
        {
            if (_deckManager.Deck.Cards.Count == 0) yield break;
            OnEnemyAction?.Invoke($"Trust the heart of the cards.");
            yield return RefilCardsCR(false);
            yield return new WaitForSeconds(1f);
            OnEnemyAction?.Invoke("");
        }

        bool keepPlaying = true;

        while (keepPlaying)
        {
            while (_enemyController.TryChoosePeixinho(out var cards, out bool isFull))
            {
                Rank rankPex = cards[0].Rank;

                foreach (CardInstance card in cards)
                {
                    _enemyController.HandManager.ToggleCardSelection(card);
                }

                OnEnemyAction?.Invoke(isFull ? $"Peixinho of {_cardSettings.GetRankName(rankPex)}!" : $"Half-xinho  of {_cardSettings.GetRankName(rankPex)}!");
                yield return new WaitForSeconds(1.5f);
                yield return PlayPeixinhoCR(false);
                OnEnemyAction?.Invoke("");

                DoRankHability(_enemyController, _playerController, rankPex, isFull);

                yield return new WaitForSeconds(1f);
            }

            if (_enemyController.HandManager.Hand.Count > 0)
            {
                Rank rank = _enemyController.ChooseRank();

                Debug.Log($"ENEMY CALLED : {rank}");
                string rankname = _cardSettings.GetRankName(rank);
                OnEnemyAction?.Invoke($"<b><color=#{Color.maroon.ToHexString()}>{rankname}</color></b>,\nyou got any?");
                yield return new WaitForSeconds(2.5f);

                bool success = ResolveCall(_enemyController.HandManager, _playerController.HandManager, rank);

                OnEnemyAction?.Invoke("");

                if (!success || _deckManager.Deck.Cards.Count == 0)
                {
                    Rank drawnRank = _deckManager.GiveCard(_enemyController.HandManager, false);

                    Debug.Log($"ENEMY : {rank} == {drawnRank} ? {rank == drawnRank}");

                    if (drawnRank != rank)
                    {
                        OnEnemyAction?.Invoke($"What a shame...");
                        yield return new WaitForSeconds(2.5f);
                        OnEnemyAction?.Invoke("");
                        keepPlaying = false;    
                    }
                    else
                    {
                        OnEnemyAction?.Invoke($"I got the <b><color=#{Color.maroon.ToHexString()}>{rank}</color></b> I was looking for...");
                        yield return new WaitForSeconds(2.5f);
                        OnEnemyAction?.Invoke("");
                    }
                }
                else
                {
                    OnEnemyAction?.Invoke($"Thank you\nhehehe...");
                    yield return new WaitForSeconds(2.5f);
                    OnEnemyAction?.Invoke("");
                }
            }
            else
            {
                OnEnemyAction?.Invoke($"I t seems I'm out of cards...");
                yield return new WaitForSeconds(2.5f);
                OnEnemyAction?.Invoke("");

                keepPlaying = false;
            }

            yield return new WaitForSeconds(1.5f);
        }

        _currentTurn++;
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
                caller.AddMult(2, isFull);
                break;
            
            case Hability.MultMult:
                caller.MultiplyMult(4f, isFull);
                break;
            
            default:
                break;
        }
    }
}
