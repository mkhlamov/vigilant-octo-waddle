using System;
using CardMatch.Core;
using UnityEngine;

namespace CardMatch.Score
{
	public class ScoreModel
	{
		public int CurrentScore { get; private set; }
		public int MatchesCount { get; private set; }
		public int AttemptsCount { get; private set; }
		public int BestScore { get; private set; }
		
		public event Action<int> OnScoreChanged;
		public event Action<int> OnMatchCountChanged;
		public event Action<int> OnAttemptCountChanged;
		public event Action<int> OnNewBestScore;
		
		private readonly ScoreSettings scoreSettings;
		private readonly IScoreSaver scoreSaver;
		
		private int currentStreak;

		public ScoreModel(ScoreSettings scoreSettings, IScoreSaver scoreSaver)
		{
			this.scoreSettings = scoreSettings;
			this.scoreSaver = scoreSaver;
		}

		public void Initialize(int levelIndex)
		{
			CurrentScore = 0;
			MatchesCount = 0;
			AttemptsCount = 0;
			currentStreak = 0;
			BestScore = scoreSaver.GetBestScore(levelIndex);
		}

		public void AddMatch()
		{
			MatchesCount++;
			currentStreak++;
			var scoreToAdd = scoreSettings.pointsPerMatch;
			
			if (currentStreak > 1)
			{
				scoreToAdd += scoreSettings.consecutiveMatchBonus;
			}
			
			CurrentScore += scoreToAdd;
			OnMatchCountChanged?.Invoke(MatchesCount);
			OnScoreChanged?.Invoke(CurrentScore);
		}

		public void AddAttempt()
		{
			AttemptsCount++;
			OnAttemptCountChanged?.Invoke(AttemptsCount);
		}

		public void SubtractPenalty()
		{
			CurrentScore = Math.Max(0, CurrentScore - scoreSettings.mismatchPenalty);
			currentStreak = 0;
			OnScoreChanged?.Invoke(CurrentScore);
		}

		public void CompleteGame(int levelIndex)
		{
			if (CurrentScore > BestScore)
			{
				BestScore = CurrentScore;
				scoreSaver.SetBestScore(levelIndex, BestScore);
				OnNewBestScore?.Invoke(BestScore);
			}
		}

		public void Load(int currentScore, int matchesCount, int attemptsCount)
		{
			CurrentScore = Math.Max(0, currentScore);
			MatchesCount = Math.Max(0, matchesCount);
			AttemptsCount = Math.Max(0, attemptsCount);
			currentStreak = 0;
			OnScoreChanged?.Invoke(CurrentScore);
			OnMatchCountChanged?.Invoke(MatchesCount);
			OnAttemptCountChanged?.Invoke(AttemptsCount);
		}
	}
}