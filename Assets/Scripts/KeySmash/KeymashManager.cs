using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class KeymashManager : MonoBehaviour
{
    private KeyCode _keyToPress;
    private float _playerScore = 5f;
    [SerializeField] private List<LetterSprite> _spriteList = new List<LetterSprite>();
    [SerializeField] private float _playerGain = 0.6f;
    [SerializeField] private float _winThreshold = 10;
    [SerializeField] private float _enemyGain = 0.2f;
    [SerializeField] private float _enemySpeed = 0.1f;
    [SerializeField] private Image _keyImage;
    private string _spamWord = "CAPITAL";
    private int _letterIndex;
    private bool _hasWon = false;
    private void Start()
    {
        UpdateKeyToPress();
        StartCoroutine(KeySmashSequence());
    }
    private IEnumerator KeySmashSequence()
    {
        while (_playerScore < _winThreshold)
        {
            _playerScore -= _enemyGain;
            yield return new WaitForSecondsRealtime(_enemySpeed);
        }
        _hasWon = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(_keyToPress) && !_hasWon)
        {
            _playerScore += 0.6f;
            if (_letterIndex < _spamWord.Length - 1) _letterIndex++;
            else _letterIndex = 0;
            UpdateKeyToPress();
        }
        
        Debug.Log(_playerScore);
    }
    private void UpdateKeyToPress()
    {
        char currentLetter = _spamWord[_letterIndex];
        _keyToPress = Enum.Parse<KeyCode>(currentLetter.ToString());
        _keyImage.sprite = SpriteForLetter(currentLetter);
    }

    private Sprite SpriteForLetter(char letter)
    {
        foreach (LetterSprite keySprite in _spriteList)
        {
            if (letter == keySprite.Letter) return keySprite.KeySprite;
        }
        return null;
    }

    [Serializable]
    public struct LetterSprite
    {
        public Sprite KeySprite;
        public char Letter;
    }
}
