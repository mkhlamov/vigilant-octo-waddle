using System;
using CardMatch.Levels;
using Zenject;

namespace CardMatch.Core.Levels
{
	public class GameCompletionHandler : IInitializable, IDisposable
	{
		private readonly GameManager gameManager;
		private readonly LevelManager levelManager;

		public GameCompletionHandler(GameManager gameManager, LevelManager levelManager)
		{
			this.gameManager = gameManager;
			this.levelManager = levelManager;
		}

		public void Initialize()
		{
			gameManager.OnGameCompleted += OnGameCompleted;
		}

		public void Dispose()
		{
			gameManager.OnGameCompleted -= OnGameCompleted;
		}

		private void OnGameCompleted()
		{
			levelManager.MarkCurrentLevelCompleted();
		}
	}
}


