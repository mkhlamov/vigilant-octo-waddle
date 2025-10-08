using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CardMatch.UI.Score
{
    public class GameScreenView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI currentScoreText;
        
        [SerializeField]
        private TextMeshProUGUI bestScoreText;
        
        [SerializeField]
        private Button nextLevelButton;
        
        [SerializeField]
        private TextMeshProUGUI nextLevelButtonText;
        
        public event Action OnNextLevelClicked;
        
        private void Awake()
        {
            nextLevelButton.onClick.AddListener(() => OnNextLevelClicked?.Invoke());
        }
        
        private void OnDestroy()
        {
            nextLevelButton.onClick.RemoveAllListeners();
        }
        
        public void SetCurrentScore(int score)
        {
            currentScoreText.text = $"Score: {score}";
        }
        
        public void SetBestScore(int score)
        {
            bestScoreText.text = $"Best: {score}";
        }
        
        public void SetNextLevelButtonActive(bool active)
        {
            nextLevelButton.gameObject.SetActive(active);
        }
    }
}
