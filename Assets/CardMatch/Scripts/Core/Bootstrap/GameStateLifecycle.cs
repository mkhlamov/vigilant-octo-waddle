using CardMatch.Data;
using UnityEngine;
using Zenject;

namespace CardMatch.Bootstrap
{
	public class GameStateLifecycle : IInitializable, System.IDisposable
	{
		private readonly IGameStateStorage storage;
		private readonly GameManager gameManager;

		public GameStateLifecycle(IGameStateStorage storage, GameManager gameManager)
		{
			this.storage = storage;
			this.gameManager = gameManager;
		}

		public void Initialize()
		{
			Application.quitting += OnQuitting;
		}

		public void Dispose()
		{
			Application.quitting -= OnQuitting;
		}

		private void OnQuitting()
		{
			gameManager.SaveState();
		}
	}
}


