using UnityEngine;

namespace CardMatch.Data
{
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "CardMatch/LevelSettings", order = 0)]
    public class LevelSettings : ScriptableObject
    {
        public Sprite[] cardSprites;
        public Sprite cardBackSprite;
    }
}