using UnityEngine;

namespace TankGame
{
    public interface IDamageReceiver
    {
        void TakeDamage(int amount);
    }
}
