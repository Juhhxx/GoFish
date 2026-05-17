using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Deck", menuName = "Cards/Deck")]
public class Deck : ScriptableObject
{
    [field: SerializeField] public string DeckName;
    [field: SerializeField, ShowAssetPreview] public Sprite DeckFront;
    [field: SerializeField, ShowAssetPreview] public Sprite DeckBack;

    [SerializeField, ReorderableList] private List<Card> _cards;
    public List<Card> Cards => _cards;
}
