using System;
using UnityEngine;

namespace CardMatch.UI
{
    public class ContainerResizeDetector : MonoBehaviour
    {
        public event Action<Vector2> OnContainerSizeChanged;
        
        private RectTransform rectTransform;
        private Vector2 lastSize;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            lastSize = rectTransform.rect.size;
        }
        
        private void Update()
        {
            CheckForSizeChange();
        }
        
        private void OnRectTransformDimensionsChange()
        {
            CheckForSizeChange();
        }
        
        private void CheckForSizeChange()
        {
            if (!rectTransform)
            {
                return;
            }
            var currentSize = rectTransform.rect.size;
            
            if (Mathf.Abs(currentSize.x - lastSize.x) > 0.1f || Mathf.Abs(currentSize.y - lastSize.y) > 0.1f)
            {
                lastSize = currentSize;
                OnContainerSizeChanged?.Invoke(currentSize);
            }
        }
    }
}
