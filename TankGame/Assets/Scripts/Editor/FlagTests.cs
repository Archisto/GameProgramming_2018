using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using TankGame.Systems;

namespace TankGame.Testing
{
    public class FlagTests
    {
        [Test]
        public void FlagTestsCreatePlayerAndEnemyMasks()
        {
            int playerLayer = LayerMask.NameToLayer("Player");
            int enemyLayer = LayerMask.NameToLayer("Enemy");

            int mask = Flags.CreateMask(playerLayer, enemyLayer);
            int validMask = LayerMask.GetMask("Player", "Enemy");

            Assert.AreEqual(mask, validMask);
        }
    }
}
