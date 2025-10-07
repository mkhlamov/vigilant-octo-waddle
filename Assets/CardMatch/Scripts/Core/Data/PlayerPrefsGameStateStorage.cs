using System;
using UnityEngine;

namespace CardMatch.Data
{
    public class PlayerPrefsGameStateStorage : IGameStateStorage
    {
        private const string KEY_PREFIX = "CardMatch_GameState_Level_";

        public bool HasSavedState(int levelIndex)
        {
            return PlayerPrefs.HasKey(GetKey(levelIndex));
        }

        public void Save(int levelIndex, GameStateData state)
        {
            var json = JsonUtility.ToJson(state);
            PlayerPrefs.SetString(GetKey(levelIndex), json);
            PlayerPrefs.Save();
        }

        public GameStateData Load(int levelIndex)
        {
            if (!HasSavedState(levelIndex))
            {
                return null;
            }

            var json = PlayerPrefs.GetString(GetKey(levelIndex));
            try
            {
                return JsonUtility.FromJson<GameStateData>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Clear(int levelIndex)
        {
            PlayerPrefs.DeleteKey(GetKey(levelIndex));
            PlayerPrefs.Save();
        }

        private string GetKey(int levelIndex)
        {
            return KEY_PREFIX + levelIndex;
        }
    }
}