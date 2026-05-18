using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private List<CardInstance> _hand;
    public event Action<List<CardInstance>> OnHandChanged;


    public void AddCard(CardInstance card)
    {
        _hand.Add(card);
        OnHandChanged.Invoke(_hand);
    }

    public void RemoveCard(CardInstance card)
    {
        _hand.Remove(card);
        OnHandChanged?.Invoke(_hand);
    }
}
