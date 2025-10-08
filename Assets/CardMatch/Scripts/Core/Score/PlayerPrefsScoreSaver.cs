using UnityEngine;

namespace CardMatch.Score
{
	public class PlayerPrefsScoreSaver : IScoreSaver
	{
		private const string BEST_SCORE_KEY_PREFIX = "BestScore_Level_";
		
		public int GetBestScore(int levelIndex)
		{
			var key = GetLevelScoreKey(levelIndex);
			return PlayerPrefs.GetInt(key, 0);
		}
		
		public void SetBestScore(int levelIndex, int score)
		{
			var key = GetLevelScoreKey(levelIndex);
			PlayerPrefs.SetInt(key, score);
			PlayerPrefs.Save();
		}
		
		private static string GetLevelScoreKey(int levelIndex)
		{
			return BEST_SCORE_KEY_PREFIX + levelIndex;
		}
	}
}
