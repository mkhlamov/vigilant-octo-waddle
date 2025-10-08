namespace CardMatch.Score
{
	public interface IScoreSaver
	{
		int GetBestScore(int levelIndex);
		void SetBestScore(int levelIndex, int score);
	}
}
