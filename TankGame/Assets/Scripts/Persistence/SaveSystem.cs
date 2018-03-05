using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Persistence
{
    public class SaveSystem
    {
        IPersistence persistence;

        public SaveSystem(IPersistence persistence)
        {
            this.persistence = persistence;
        }

        public void Save(GameData data)
        {
            persistence.Save(data);
        }

        public GameData Load()
        {
            return persistence.Load<GameData>();
        }
    }
}
