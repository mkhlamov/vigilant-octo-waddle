using System;
using System.Collections.Generic;
using System.Linq;
using CardMatch.Data;
using UnityEngine;

namespace CardMatch.Levels
{
	public class LevelManager
	{
		private const string LAST_COMPLETED_LEVEL_KEY = "LastCompletedLevel";

		private readonly List<LevelSettings> availableLevels;
		private readonly int levelIndex;

		public LevelSettings LevelSettings { get; }

		public LevelManager(IEnumerable<LevelSettings> availableLevels)
		{
			if (availableLevels == null)
			{
				throw new ArgumentException("No levels configured.");
			}

			this.availableLevels = availableLevels.ToList();

			var lastCompleted = PlayerPrefs.GetInt(LAST_COMPLETED_LEVEL_KEY, -1);
			levelIndex = (lastCompleted + 1) % this.availableLevels.Count;
			LevelSettings = this.availableLevels[levelIndex];
		}

		public void MarkCurrentLevelCompleted()
		{
			var toSave = Mathf.Clamp(levelIndex, 0, availableLevels.Count - 1);
			PlayerPrefs.SetInt(LAST_COMPLETED_LEVEL_KEY, toSave);
			PlayerPrefs.Save();
		}
	}
}


