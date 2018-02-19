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
    }
}
