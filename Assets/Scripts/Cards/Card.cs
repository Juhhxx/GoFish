using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Cards/Card")]
public class Card : ScriptableObject
{
    [field: OnValueChanged("UpdateName")]
    [field: SerializeField] public Rank Rank { get; private set; }
    [field: OnValueChanged("UpdateName")]
    [field: SerializeField] public Suit Suit { get; private set; }

    [field: SerializeField, ReadOnly] public string DisplayName { get; private set; }
    [field: SerializeField, ShowAssetPreview] public Sprite DisplayImage { get; private set; }

    private void UpdateName()
    {
        DisplayName = Rank.ToString() + " of " + Suit.ToString() + "s";
    }
}
