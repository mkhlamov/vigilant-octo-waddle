using UnityEngine;

namespace CardMatch.Score
{
	public class PlayerPrefsScoreSaver : IScoreSaver
	{
		private const string BEST_SCORE_KEY = "BestScore";
		
		public int GetBestScore()
		{
			return PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
		}
		
		public void SetBestScore(int score)
		{
			PlayerPrefs.SetInt(BEST_SCORE_KEY, score);
			PlayerPrefs.Save();
		}
	}
}
