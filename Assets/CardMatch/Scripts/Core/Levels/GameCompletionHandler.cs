using System;
using CardMatch.Levels;
using CardMatch.Data;
using Zenject;

namespace CardMatch.Core.Levels
{
	public class GameCompletionHandler : IInitializable, IDisposable
	{
		private readonly GameManager gameManager;
		private readonly LevelManager levelManager;
        private readonly IGameStateStorage gameStateStorage;

		public GameCompletionHandler(GameManager gameManager, LevelManager levelManager, IGameStateStorage gameStateStorage)
		{
			this.gameManager = gameManager;
			this.levelManager = levelManager;
            this.gameStateStorage = gameStateStorage;
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
			gameStateStorage.Clear(levelManager.LevelIndex);
		}
	}
}


