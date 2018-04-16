using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Persistence
{
    [Serializable]
    public class GameData
    {
        public bool GameWon;
        public bool GameLost;
        public int PlayerLives;
        public int Score;
        public UnitData PlayerData;
        public List<UnitData> EnemyDataList = new List<UnitData>();
    }
}
