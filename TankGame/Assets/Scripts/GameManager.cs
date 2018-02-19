using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private void Init()
        {
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
    }
}
