using System;
using CardMatch.Score;
using CardMatch.Levels;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace CardMatch.UI.Score
{
    public class GameScreenPresenter : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField]
        private GameScreenView view;
        
        private ScoreModel scoreModel;
        private GameManager gameManager;
        private LevelManager levelManager;
        
        [Inject]
        public void Construct(ScoreModel scoreModel, LevelManager levelManager, GameManager gameManager)
        {
            this.scoreModel = scoreModel;
            this.gameManager = gameManager;
            this.levelManager = levelManager;
        }
        
        public void Initialize()
        {
            if (!view)
            {
                Debug.LogError("ScoreUIView is not assigned in ScoreUIPresenter");
                return;
            }
            
            scoreModel.OnScoreChanged += OnScoreChanged;
            scoreModel.OnNewBestScore += OnNewBestScore;
            
            view.OnNextLevelClicked += OnNextLevelClicked;
            
            gameManager.OnGameCompleted += OnGameCompleted;
            
            UpdateUI();
        }
        
        public void Dispose()
        {
            if (scoreModel != null)
            {
                scoreModel.OnScoreChanged -= OnScoreChanged;
                scoreModel.OnNewBestScore -= OnNewBestScore;
            }
            
            if (view)
            {
                view.OnNextLevelClicked -= OnNextLevelClicked;
            }
            
            if (gameManager != null)
            {
                gameManager.OnGameCompleted -= OnGameCompleted;
            }
        }
        
        private void OnScoreChanged(int score)
        {
            view.SetCurrentScore(score);
        }
        
        private void OnNewBestScore(int score)
        {
            view.SetBestScore(score);
        }
        
        private void OnNextLevelClicked()
        {
            levelManager.MarkCurrentLevelCompleted();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        private void OnGameCompleted()
        {
            view.SetNextLevelButtonActive(true);
        }
        
        private void UpdateUI()
        {
            view.SetCurrentScore(scoreModel.CurrentScore);
            view.SetBestScore(scoreModel.BestScore);
            view.SetNextLevelButtonActive(false);
        }
    }
}
