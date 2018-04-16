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

        private const string LanguageKey = "Language";

        [SerializeField]
        private bool enemyWeaponsDisabled;

        /// <summary>
        /// The required amount of score to win the game
        /// </summary>
        private int targetScore = 300;

        /// <summary>
        /// The number of lives the player has at the start
        /// </summary>
        private int startingLives = 3;

        private List<Unit> enemyUnits = new List<Unit>();
        private Unit playerUnit;
        private UI.UI uiObj;
        private PlayerStatusUI playerStatusUI;

        SaveSystem saveSystem;

        public string SavePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, "save1");
            }
        }

        public MessageBus MessageBus { get; private set; }

        public bool GameWon { get; private set; }

        public bool GameLost { get; private set; }

        /// <summary>
        /// The number of lives the player has.
        /// Losing them all means losing the game.
        /// </summary>
        public int PlayerLives { get; private set; }

        /// <summary>
        /// The number of lives the player has at the start.
        /// </summary>
        public int StartingLives { get { return startingLives; } }

        /// <summary>
        /// The number of deaths the player has suffered.
        /// </summary>
        public int PlayerDeaths { get { return StartingLives - PlayerLives; } }

        /// <summary>
        /// The player's score.
        /// </summary>
        public int Score { get; private set; }

        /// <summary>
        /// The target score to win the game.
        /// </summary>
        public int TargetScore { get { return targetScore; } }

        private void Awake()
        {
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

        //private void Start()
        //{
        //    InitUI();
        //    FindUnits();
        //}

        private void Init()
        {
            InitLocalization();

            PlayerLives = StartingLives;

            playerStatusUI = FindObjectOfType<PlayerStatusUI>();
            saveSystem = new SaveSystem(new JSONPersistence(SavePath));
            MessageBus = new MessageBus();

            IsClosing = false;

            InitUI();
            FindUnits();
        }

        protected void Update()
        {
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

        private void OnDestroy()
        {
            L10n.LanguageLoaded -= OnLanguageLoaded;
        }

        private void OnApplicationQuit()
        {
            IsClosing = true;
        }

        private void InitLocalization()
        {
            LangCode currentLang =
                (LangCode) PlayerPrefs.GetInt(LanguageKey, (int) LangCode.EN);
            L10n.LoadLanguage(currentLang);
            L10n.LanguageLoaded += OnLanguageLoaded;
        }

        private void OnLanguageLoaded()
        {
            PlayerPrefs.SetInt(LanguageKey,
                (int) L10n.CurrentLanguage.LanguageCode);
        }

        private void InitUI()
        {
            uiObj = FindObjectOfType<UI.UI>();
            uiObj.Init();
        }

        private void FindUnits()
        {
            Unit[] allUnits = FindObjectsOfType<Unit>();

            foreach (Unit unit in allUnits)
            {
                AddUnit(unit);
            }

            // Adds the player unit's health to the UI
            UI.UI.Current.HealthUI.AddUnit(playerUnit);

            // Registers to listen to the player dying
            playerUnit.Health.UnitDied += PlayerDied;

            // Adds the enemy units' healths to the UI
            foreach (EnemyUnit enemyUnit in enemyUnits)
            {
                UI.UI.Current.HealthUI.AddUnit(enemyUnit);
            }
        }

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
        /// Gets data from units and stores it to a data object.
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

        public GameData Load()
        {
            GameData data = saveSystem.Load();

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

            Debug.Log("Game loaded");

            playerStatusUI.UpdateText();

            return data;
        }

        public bool EnemyWeaponsDisabled
        {
            get
            {
                return enemyWeaponsDisabled;
            }
        }

        public void AddScore(int score)
        {
            Score += score;
            playerStatusUI.SetScoreText(Score);
            CheckWin();
        }

        private void PlayerDied(Unit unit)
        {
            if (PlayerLives > 0)
            {
                PlayerLives--;
                playerStatusUI.SetDeathsText(PlayerDeaths);
                CheckLoss();
            }
        }

        private void CheckWin()
        {
            if (Score >= TargetScore && !GameWon && !GameLost)
            {
                WinGame();
            }
        }

        private void CheckLoss()
        {
            if (PlayerLives <= 0 && !GameLost && !GameWon)
            {
                LoseGame();
            }
        }

        private void WinGame()
        {
            GameWon = true;
            uiObj.DisplayWinMessage(true);
        }

        private void LoseGame()
        {
            GameLost = true;
            uiObj.DisplayLoseMessage(true);
        }

        public void ResetGame()
        {
            GameWon = false;
            GameLost = false;

            PlayerLives = StartingLives;
            Score = 0;

            playerUnit.ResetUnit();

            foreach (Unit enemyUnit in enemyUnits)
            {
                enemyUnit.ResetUnit();
            }

            uiObj.ResetUI();
            playerStatusUI.UpdateText();
        }
    }
}
