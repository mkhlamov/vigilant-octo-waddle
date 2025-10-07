using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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
        private bool hasCachedScale = false;
        private Tween flipTween;

        private void Awake()
        {
            EnsureCachedTransform();
        }

        private void OnDisable()
        {
            KillTweens();
            isFlipping = false;
        }

        public void Initialize(Sprite frontSprite, Sprite backSprite)
        {
            cardFrontSprite = frontSprite;
            cardBackSprite = backSprite;
        }

        public void SetSize(Vector2 cardSize)
        {
            EnsureCachedTransform();
            cardTransform.sizeDelta = cardSize;
        }

        public void SetPosition(Vector2 position)
        {
            EnsureCachedTransform();
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
            EnsureCachedTransform();

            isFlipping = true;
            cardButton.interactable = false;

            KillTweens();
            flipTween = cardTransform.DOScaleX(0f, flipDuration / 2f).SetEase(flipEase);
            await flipTween.AsyncWaitForCompletion();

            cardImage.sprite = faceUp ? cardFrontSprite : cardBackSprite;

            flipTween = cardTransform.DOScaleX(originalScale.x, flipDuration / 2f).SetEase(flipEase);
            await flipTween.AsyncWaitForCompletion();

            cardButton.interactable = true;
            isFlipping = false;
        }

        public void ShowMatched()
        {
            cardButton.interactable = false;

            var color = cardImage.color;
            color.a = 0.3f;
            cardImage.color = color;
        }

        public void ResetVisuals()
        {
            EnsureCachedTransform();
            KillTweens();
            cardTransform.localScale = originalScale;
            var color = cardImage.color;
            color.a = 1f;
            cardImage.color = color;
            cardButton.interactable = true;
        }

        private void KillTweens()
        {
            if (flipTween != null && flipTween.IsActive())
            {
                flipTween.Kill();
                flipTween = null;
            }

            EnsureCachedTransform();
            cardTransform.DOKill();
        }

        private void EnsureCachedTransform()
        {
            if (cardTransform == null)
            {
                cardTransform = GetComponent<RectTransform>();
            }

            if (!hasCachedScale && cardTransform != null)
            {
                originalScale = cardTransform.localScale;
                hasCachedScale = true;
            }
        }
    }
}