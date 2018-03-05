using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Persistence;

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
                if (instance == null)
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

        #endregion

        private List<Unit> enemyUnits = new List<Unit>();
        private Unit playerUnit;

        SaveSystem saveSystem;

        public string SavePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, "save");
            }
        }

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

        protected void Update()
        {
            bool saveInput = Input.GetKeyDown(KeyCode.F2);
            bool loadInput = Input.GetKeyDown(KeyCode.F3);

            if (saveInput)
            {
                Save();
            }
            else if (loadInput)
            {
                Load();
            }
        }

        private void Init()
        {
            saveSystem = new SaveSystem(new BinaryPersistence(SavePath));

            FindUnits();
        }

        private void FindUnits()
        {
            Unit[] allUnits = FindObjectsOfType<Unit>();
            foreach (Unit unit in allUnits)
            {
                AddUnit(unit);
            }
        }

        private void AddUnit(Unit unit)
        {
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

            return data;
        }
    }
}
