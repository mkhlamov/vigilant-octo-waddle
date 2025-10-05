using UnityEngine;

namespace CardMatch.Data
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "CardMatch/Grid Config")]
    public class GridConfig : ScriptableObject
    {
        [Header("Grid Layout")]
        public int rows = 2;
        public int columns = 2;
    
        [Header("Spacing")]
        public float cardSpacing = 10f;
        public Vector2 gridPadding = new Vector2(20f, 20f);
    
        [Header("Card Settings")]
        public Vector2 cardSize = new Vector2(100f, 140f);
    
        public int TotalCards => rows * columns;
    }
}