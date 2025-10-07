using UnityEngine;

namespace CardMatch.Core
{
    [CreateAssetMenu(fileName = "ScoreSettings", menuName = "CardMatch/Score Settings")]
    public class ScoreSettings : ScriptableObject
    {
        public int pointsPerMatch = 100;
        public int consecutiveMatchBonus = 50;
        public int mismatchPenalty = 25;
    }
}