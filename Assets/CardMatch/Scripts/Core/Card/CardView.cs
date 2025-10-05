using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CardMatch.Card
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] 
        private Image cardImage;
        
        [SerializeField]
        private Button cardButton;
        
        [SerializeField] 
        private float flipDuration = 0.3f;
        
        [SerializeField] 
        private Ease flipEase = Ease.InOutQuad;
        
        public Button Button => cardButton;
        
        private RectTransform cardTransform;
        private Sprite cardFrontSprite;
        private Sprite cardBackSprite;
        
        private bool isFlipping = false;
        private Vector3 originalScale;

        private void Awake()
        {
            cardTransform = GetComponent<RectTransform>();
            originalScale = cardTransform.localScale;
        }

        public void Initialize(Sprite frontSprite, Sprite backSprite)
        {
            cardFrontSprite = frontSprite;
            cardBackSprite = backSprite;
        }
        
        public void SetSize(Vector2 cardSize)
        {
            cardTransform.sizeDelta = cardSize;
        }

        public void SetPosition(Vector2 position)
        {
            cardTransform.anchoredPosition = position;
        }
        
        public void ShowFaceDown()
        {
            cardImage.sprite = cardBackSprite;
            cardButton.interactable = true;
        }
        
        public void ShowFaceUp()
        {
            cardImage.sprite = cardFrontSprite;
            cardButton.interactable = false;
        }
        
        public async UniTask FlipCard(bool faceUp)
        {
            if (isFlipping) return;
        
            isFlipping = true;
            cardButton.interactable = false;
            
            await cardTransform.DOScaleX(0f, flipDuration / 2f)
                .SetEase(flipEase)
                .AsyncWaitForCompletion();
            
            cardImage.sprite = faceUp ? cardFrontSprite : cardBackSprite;
            
            await cardTransform.DOScaleX(originalScale.x, flipDuration / 2f)
                .SetEase(flipEase)
                .AsyncWaitForCompletion();
        
            cardButton.interactable = true;
            isFlipping = false;
        }
        
        public void ShowMatched()
        {
            cardButton.interactable = false;
            
            var color = cardImage.color;
            color.a = 0.7f;
            cardImage.color = color;
        }
    }
}