using UnityEngine;

namespace CardMatch.Data
{
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "CardMatch/LevelSettings", order = 0)]
    public class LevelSettings : ScriptableObject
    {
        [Header("Grid Configuration")]
        public GridConfig gridConfig;

        [Header("Card Sprites")]
        public Sprite[] cardSprites;
        public Sprite cardBackSprite;
    }
}