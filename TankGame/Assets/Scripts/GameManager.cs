using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.UI;
using TankGame.Persistence;
using TankGame.Messaging;
using TankGame.Localization;
using L10n = TankGame.Localization.Localization;

namespace TankGame
{
    /// <summary>
    /// A class that manages the most important aspects of the game.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Statics

        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
                if (instance == null && !IsClosing)
                {
                    GameObject gmObj = new GameObject(typeof(GameManager).Name);
                    instance = gmObj.AddComponent<GameManager>();

                    //instance = Resources.Load<GameManager>("GameManager");
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        /// <summary>
        /// Is the game about to be exited.
        /// </summary>
        public static bool IsClosing { get; private set; }

        #endregion

        // Useful field tags:
        /* SerializeField, Header, ToolTip, Range, HideInInspector,
         * Flags, ExecuteInEditMode */

        /// <summary>
        /// Preferences key for the saved language setting
        /// </summary>
        private const string LanguageKey = "Language";

        /// <summary>
        /// Are enemy weapons disabled.
        /// Debugging purposes only.
        /// </summary>
        [SerializeField]
        private bool enemyWeaponsDisabled;

        /// <summary>
        /// The required amount of score to win the game
        /// </summary>
        private int targetScore = 500;

        /// <summary>
        /// The maximum number of lives the player has
        /// </summary>
        private int maxLives = 3;

        private List<Unit> enemyUnits = new List<Unit>();
        private Unit playerUnit;
        private DestroyedTankSpawner destroyedTankSpawner;
        private UI.UI uiObj;
        private PlayerStatusUI playerStatusUI;
        private SaveSystem saveSystem;

        /// <summary>
        /// The file path where the game is saved.
        /// </summary>
        public string SavePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, "save1");
            }
        }

        /// <summary>
        /// A message bus for publishing messages.
        /// </summary>
        public MessageBus MessageBus { get; private set; }

        /// <summary>
        /// Did the player win the game.
        /// </summary>
        public bool GameWon { get; private set; }

        /// <summary>
        /// Did the player lose the game.
        /// </summary>
        public bool GameLost { get; private set; }

        /// <summary>
        /// The number of lives the player has.
        /// Losing them all means losing the game.
        /// </summary>
        public int PlayerLives { get; private set; }

        /// <summary>
        /// The maximum number of lives the player has.
        /// </summary>
        public int MaxLives { get { return maxLives; } }

        /// <summary>
        /// The number of deaths the player has suffered.
        /// </summary>
        public int PlayerDeaths { get { return MaxLives - PlayerLives; } }

        /// <summary>
        /// The player's score.
        /// </summary>
        public int Score { get; private set; }

        /// <summary>
        /// The target score to win the game.
        /// </summary>
        public int TargetScore { get { return targetScore; } }

        /// <summary>
        /// Initializes the object before Start is called.
        /// </summary>
        private void Awake()
        {
            // Keeps only one instance of this object and removes others
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Init();
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Init()
        {
            InitLocalization();

            IsClosing = false;
            PlayerLives = MaxLives;

            destroyedTankSpawner = FindObjectOfType<DestroyedTankSpawner>();
            playerStatusUI = FindObjectOfType<PlayerStatusUI>();
            saveSystem = new SaveSystem(new JSONPersistence(SavePath));
            MessageBus = new MessageBus();

            InitUI();
            InitUnits();
        }

        /// <summary>
        /// Updates the object each frame.
        /// </summary>
        protected void Update()
        {
            // Handles player input
            bool saveInput = Input.GetKeyUp(KeyCode.F2);
            bool loadInput = Input.GetKeyUp(KeyCode.F3);
            bool resetInput = Input.GetKeyUp(KeyCode.R);

            if (saveInput)
            {
                Save();
            }
            else if (loadInput)
            {
                Load();
            }
            else if (resetInput)
            {
                ResetGame();
            }
        }

        /// <summary>
        /// Called when the object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            L10n.LanguageLoaded -= OnLanguageLoaded;
        }

        /// <summary>
        /// Called when the application is closing.
        /// </summary>
        private void OnApplicationQuit()
        {
            IsClosing = true;
        }

        /// <summary>
        /// Initializes localization.
        /// </summary>
        private void InitLocalization()
        {
            LangCode currentLang =
                (LangCode) PlayerPrefs.GetInt(LanguageKey, (int) LangCode.EN);
            L10n.LoadLanguage(currentLang);
            L10n.LanguageLoaded += OnLanguageLoaded;
        }

        /// <summary>
        /// Called when a LanguageLoaded event is fired.
        /// </summary>
        private void OnLanguageLoaded()
        {
            // Saves the currently selected language
            PlayerPrefs.SetInt(LanguageKey,
                (int) L10n.CurrentLanguage.LanguageCode);
        }

        /// <summary>
        /// Initializes UI.
        /// </summary>
        private void InitUI()
        {
            uiObj = FindObjectOfType<UI.UI>();

            if (uiObj != null)
            {
                uiObj.Init();
            }
            else
            {
                Debug.LogError("UI not found.");
            }
        }

        /// <summary>
        /// Initializes player and enemy units.
        /// </summary>
        private void InitUnits()
        {
            Unit[] allUnits = FindObjectsOfType<Unit>();

            foreach (Unit unit in allUnits)
            {
                AddUnit(unit);
            }

            // Adds the player unit's health to the UI
            UI.UI.Current.HealthUI.AddUnit(playerUnit);

            // Adds the enemy units' healths to the UI
            foreach (EnemyUnit enemyUnit in enemyUnits)
            {
                UI.UI.Current.HealthUI.AddUnit(enemyUnit);
            }
        }

        /// <summary>
        /// Adds a unit to a unit list.
        /// </summary>
        /// <param name="unit">A unit</param>
        private void AddUnit(Unit unit)
        {
            unit.Init();

            if (unit is EnemyUnit)
            {
                enemyUnits.Add(unit);
            }

            // Adding a new player unit after the initialization really makes
            // no sense because we can have a reference to only one player unit.
            // Be careful with this.
            else if (unit is PlayerUnit)
            {
                playerUnit = unit;
            }
        }

        /// <summary>
        /// Are enemy weapons disabled.
        /// </summary>
        public bool EnemyWeaponsDisabled
        {
            get
            {
                return enemyWeaponsDisabled;
            }
        }

        /// <summary>
        /// Gets data from the game and units and stores it to a data object.
        /// </summary>
        public void Save()
        {
            GameData data = new GameData();

            data.GameWon = GameWon;
            data.GameLost = GameLost;
            data.PlayerLives = PlayerLives;
            data.Score = Score;

            foreach (Unit unit in enemyUnits)
            {
                data.EnemyDataList.Add(unit.GetUnitData());
            }

            data.PlayerData = playerUnit.GetUnitData();

            saveSystem.Save(data);

            Debug.Log("Game saved");
        }

        /// <summary>
        /// Loads saved data.
        /// </summary>
        /// <returns>Loaded game data</returns>
        public GameData Load()
        {
            GameData data = saveSystem.Load();

            ResetGame();

            GameWon = data.GameWon;
            GameLost = data.GameLost;

            if (GameWon)
            {
                WinGame();
            }
            else if (GameLost)
            {
                LoseGame();
            }

            PlayerLives = data.PlayerLives;
            Score = data.Score;

            foreach (UnitData enemyData in data.EnemyDataList)
            {
                Unit enemy = enemyUnits.FirstOrDefault(unit => unit.ID == enemyData.ID);

                if (enemy != null)
                {
                    enemy.SetUnitData(enemyData);
                }
            }

            playerUnit.SetUnitData(data.PlayerData);
            playerStatusUI.UpdateText();

            Debug.Log("Game loaded");

            return data;
        }

        /// <summary>
        /// Gives score to the player.
        /// </summary>
        /// <param name="score">Score amount</param>
        public void AddScore(int score)
        {
            Score += score;
            playerStatusUI.SetScoreText(Score);
            CheckWin();
        }

        /// <summary>
        /// Spawns a tank wreckage to the position of a dead unit.
        /// </summary>
        /// <param name="unit">A dead unit</param>
        /// <param name="isPlayerUnit">Is the unit a player unit</param>
        public void UnitDied(Unit unit, bool isPlayerUnit)
        {
            if (isPlayerUnit && PlayerLives > 0)
            {
                PlayerLives--;
                playerStatusUI.SetDeathsText(PlayerDeaths);
                CheckLoss();
            }

            SpawnDestroyedTank(unit);
        }

        /// <summary>
        /// Handles unit respawning.
        /// </summary>
        /// <param name="unit">A respawned unit</param>
        public void UnitRespawned(Unit unit)
        {
            destroyedTankSpawner.DespawnDestroyedTank(unit);
        }

        /// <summary>
        /// Spawns a destroyed tank to the position of a dead unit.
        /// </summary>
        /// <param name="unit">A dead unit</param>
        public void SpawnDestroyedTank(Unit unit)
        {
            destroyedTankSpawner.SpawnDestroyedTank(unit);
        }

        /// <summary>
        /// Checks if the player should win the game.
        /// </summary>
        private void CheckWin()
        {
            if (Score >= TargetScore
                && !GameWon && !GameLost)
            {
                WinGame();
            }
        }

        /// <summary>
        /// Checks if the player should lose the game.
        /// </summary>
        private void CheckLoss()
        {
            if (PlayerLives <= 0
                && !GameLost && !GameWon)
            {
                LoseGame();
            }
        }

        /// <summary>
        /// Sets the game won.
        /// </summary>
        private void WinGame()
        {
            GameWon = true;
            uiObj.DisplayWinMessage(true);
        }

        /// <summary>
        /// Sets the game lost.
        /// </summary>
        private void LoseGame()
        {
            GameLost = true;
            uiObj.DisplayLoseMessage(true);
            //MessageBus.Publish(new GameLostMessage());
        }

        /// <summary>
        /// Resets everything and starts the game from the beginning.
        /// </summary>
        public void ResetGame()
        {
            GameWon = false;
            GameLost = false;

            PlayerLives = MaxLives;
            Score = 0;

            destroyedTankSpawner.DespawnAllDestroyedTanks();
            playerUnit.Respawn();

            foreach (Unit enemyUnit in enemyUnits)
            {
                enemyUnit.Respawn();
            }

            uiObj.ResetUI();
            playerStatusUI.UpdateText();

            MessageBus.Publish(new GameResetMessage());
        }
    }
}
