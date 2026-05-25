using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class KeymashManager : MonoBehaviour
{
    private KeyCode _keyToPress;
    private float _playerScore = 5f;
    private List<GameObject> _spawnedKeys = new List<GameObject>(); 
    private List<Image> _spawnedKeyImages = new List<Image>();
    [SerializeField] private List<LetterSprite> _spriteList = new List<LetterSprite>();
    [SerializeField] private float _playerGain = 0.45f;
    [SerializeField] private float _winThreshold = 10;
    [SerializeField] private float _enemyGain = 0.2f;
    [SerializeField] private float _enemySpeed = 0.1f;
    [SerializeField] private float _shakeIntensity = 50f;
    [SerializeField] private RectTransform _shakeTarget;
    [SerializeField] private Image _progressSlider;
    [SerializeField] private RectTransform _sliderTransform;
    [SerializeField] private Transform _layoutGroup;
    [SerializeField] private GameObject _keycapPrefab;
    private string _spamWord;
    private int _letterIndex;
    private char _currentLetter;
    private bool _hasEnded = false;
    [Button(enabledMode:EButtonEnableMode.Playmode)]
    private void StartMinigameTest()
    {
        StartMinigame("PENISBALLS");
    }
    public void StartMinigame(string givenWord)
    {
        for (int i = 0; i < _spawnedKeyImages.Count; i++) 
        {
            _spawnedKeyImages[i].DOKill();
            Destroy(_spawnedKeys[i]);
        }
        _spawnedKeys.Clear();
        _spawnedKeyImages.Clear();
        _playerScore = _winThreshold/2;
        _letterIndex = 0;
        _hasEnded = false;

        _spamWord = givenWord;
        
        UpdateKeyToPress();
        UpdateKeyUI();
        StartCoroutine(KeySmashSequence());
    }
    
    private IEnumerator KeySmashSequence()
    {
        while (_playerScore < _winThreshold)
        {
            _playerScore -= _enemyGain;
            yield return new WaitForSecondsRealtime(_enemySpeed);
        }
        _hasEnded = true;
        StartCoroutine(AutoComplete());
    }

    private void Update()
    {
        if (Input.GetKeyDown(_keyToPress) && !_hasEnded)
        {
            ProceedLetter();
            _shakeTarget.DOAnchorPos(Vector3.zero, 0.1f);
            _shakeTarget.DOShakePosition(0.2f, new Vector3(_shakeIntensity, _shakeIntensity, 0));
        }
        _progressSlider.fillAmount = Mathf.Clamp01(_playerScore / _winThreshold);
    }

    private void UpdateKeyToPress()
    {
        _currentLetter = _spamWord[_letterIndex];
        _keyToPress = Enum.Parse<KeyCode>(_currentLetter.ToString());
    }

    private void UpdateKeyUI()
    {
        GameObject instantiatedKey = Instantiate(_keycapPrefab, _layoutGroup);
        instantiatedKey.GetComponent<Image>().sprite = SpriteForLetter(_currentLetter);
        instantiatedKey.transform.localScale = Vector3.zero;

        instantiatedKey.transform.DOScale(1f, 0.15f).SetEase(Ease.OutBack);
        _spawnedKeys.Add(instantiatedKey);
        _spawnedKeyImages.Add(instantiatedKey.GetComponent<Image>());
    }
    
    private void ProceedLetter()
    {
        if (!_hasEnded) 
        {
            _playerScore += _playerGain;
            _sliderTransform.DOKill(false);
            _sliderTransform.DOShakeRotation(0.075f, 5f);
        }
        if (_spawnedKeyImages.Count >= 1) 
        {
            RectTransform rectTrans = _spawnedKeyImages.Last().rectTransform;
            rectTrans.DOKill(false);
            rectTrans.localScale = Vector3.one;
            _spawnedKeyImages.Last().sprite = PressedSpriteForLetter(_currentLetter);
            _spawnedKeyImages.Last().rectTransform.DOPunchScale(Vector3.one * 0.4f, 0.2f);
        }
        if (_letterIndex < _spamWord.Length - 1) _letterIndex++;
        else if (!_hasEnded)
        {
            _letterIndex = 0;
            for (int i = 0; i < _spawnedKeyImages.Count; i++) 
            {
                _spawnedKeyImages[i].DOKill();
                Destroy(_spawnedKeys[i]);
            }
            _spawnedKeys.Clear();
            _spawnedKeyImages.Clear();
        }
        UpdateKeyToPress();
        UpdateKeyUI();
    }
    private IEnumerator AutoComplete()
    {
        while (_letterIndex < _spamWord.Length - 1)
        {
            ProceedLetter();
            yield return new WaitForSecondsRealtime(0.15f);
        }
        if (_spawnedKeyImages.Count > 0)
            {
                _spawnedKeyImages.Last().sprite = PressedSpriteForLetter(_currentLetter);
                _spawnedKeyImages.Last().rectTransform.DOPunchScale(Vector3.one * 0.4f, 0.2f);
            }
    }

    private Sprite SpriteForLetter(char letter)
    {
        foreach (LetterSprite keySprite in _spriteList)
        {
            if (letter == keySprite.Letter) return keySprite.KeySprite;
        }
        return null;
    }

    private Sprite PressedSpriteForLetter(char letter)
    {
        foreach (LetterSprite keySprite in _spriteList)
        {
            if (letter == keySprite.Letter) return keySprite.PressedSprite;
        }
        return null;
    }

    [Serializable]
    public struct LetterSprite
    {
        public Sprite KeySprite;
        public Sprite PressedSprite;
        public char Letter;
    }
}
