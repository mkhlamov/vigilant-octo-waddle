namespace CardMatch.Score
{
	public interface IScoreSaver
	{
		int GetBestScore();
		void SetBestScore(int score);
	}
}
