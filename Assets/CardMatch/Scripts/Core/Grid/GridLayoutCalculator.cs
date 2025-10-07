using UnityEngine;
using CardMatch.Data;

namespace CardMatch.Grid
{
    public class GridLayoutCalculator
    {
        private readonly GridConfig gridConfig;

        public GridLayoutCalculator(GridConfig gridConfig)
        {
            this.gridConfig = gridConfig;
            ValidateGridConfig();
        }
        
        public Vector2 CalculateCardSize(Vector2 containerSize)
        {
            if (containerSize.x <= 0 || containerSize.y <= 0)
            {
                Debug.LogError($"Invalid container size: {containerSize}");
                return gridConfig.cardSize;
            }

            var availableSpace = CalculateAvailableSpace(containerSize);
            var calculatedSize = new Vector2(
                availableSpace.x / gridConfig.columns, 
                availableSpace.y / gridConfig.rows
            );

            return ApplyAspectRatio(calculatedSize);
        }
        
        public Vector2 CalculateCardPosition(int row, int col, Vector2 cardSize)
        {
            ValidateGridCoordinates(row, col);

            var totalDimensions = CalculateTotalGridDimensions(cardSize);
            var startPosition = CalculateStartPosition(totalDimensions, cardSize);
            
            return new Vector2(
                startPosition.x + col * (cardSize.x + gridConfig.cardSpacing),
                startPosition.y - row * (cardSize.y + gridConfig.cardSpacing)
            );
        }
        
        public (int row, int col) CalculateGridPosition(int index)
        {
            if (index < 0)
            {
                Debug.LogError($"Invalid card index: {index}");
                return (0, 0);
            }

            var row = index / gridConfig.columns;
            var col = index % gridConfig.columns;

            if (row >= gridConfig.rows)
            {
                Debug.LogWarning($"Card index {index} exceeds grid capacity. Row: {row}, Max rows: {gridConfig.rows}");
            }

            return (row, col);
        }

        private void ValidateGridConfig()
        {
            if (gridConfig.rows <= 0 || gridConfig.columns <= 0)
            {
                Debug.LogError($"Invalid grid dimensions: {gridConfig.rows}x{gridConfig.columns}");
            }

            if (gridConfig.cardSpacing < 0)
            {
                Debug.LogWarning($"Negative card spacing: {gridConfig.cardSpacing}");
            }

            if (gridConfig.cardSize.x <= 0 || gridConfig.cardSize.y <= 0)
            {
                Debug.LogError($"Invalid card size: {gridConfig.cardSize}");
            }
        }

        private void ValidateGridCoordinates(int row, int col)
        {
            if (!IsValidGridPosition(row, col))
            {
                Debug.LogError($"Invalid grid coordinates: ({row}, {col}). Grid size: {gridConfig.rows}x{gridConfig.columns}");
            }
        }
        
        private bool IsValidGridPosition(int row, int col)
        {
            return row >= 0 && row < gridConfig.rows && col >= 0 && col < gridConfig.columns;
        }

        private Vector2 CalculateAvailableSpace(Vector2 containerSize)
        {
            var availableWidth = containerSize.x - gridConfig.gridPadding.x * 2 - 
                                gridConfig.cardSpacing * (gridConfig.columns - 1);
            var availableHeight = containerSize.y - gridConfig.gridPadding.y * 2 - 
                                 gridConfig.cardSpacing * (gridConfig.rows - 1);
            
            availableWidth = Mathf.Max(0, availableWidth);
            availableHeight = Mathf.Max(0, availableHeight);

            return new Vector2(availableWidth, availableHeight);
        }

        private Vector2 ApplyAspectRatio(Vector2 calculatedSize)
        {
            var aspectRatio = gridConfig.cardSize.x / gridConfig.cardSize.y;
            
            return calculatedSize.x / aspectRatio <= calculatedSize.y
                ? new Vector2(calculatedSize.x, calculatedSize.x / aspectRatio)
                : new Vector2(calculatedSize.y * aspectRatio, calculatedSize.y);
        }

        private Vector2 CalculateTotalGridDimensions(Vector2 cardSize)
        {
            var totalWidth = gridConfig.columns * cardSize.x + (gridConfig.columns - 1) * gridConfig.cardSpacing;
            var totalHeight = gridConfig.rows * cardSize.y + (gridConfig.rows - 1) * gridConfig.cardSpacing;
            
            return new Vector2(totalWidth, totalHeight);
        }

        private Vector2 CalculateStartPosition(Vector2 totalDimensions, Vector2 cardSize)
        {
            var startX = -totalDimensions.x / 2 + cardSize.x / 2;
            var startY = totalDimensions.y / 2 - cardSize.y / 2;
            
            return new Vector2(startX, startY);
        }
    }
}