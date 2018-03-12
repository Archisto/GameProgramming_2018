using UnityEngine;
using TankGame.Persistence;

namespace TankGame.Testing
{
    public class OperatorTesting : MonoBehaviour
    {
        private void Start()
        {
            var first = new SerializableVector3(1, 2, 3);
            var second = new Vector3(3, 2, 1);

            var result = first + second;
            Debug.Log(result);

            var result2 = -first;
            Debug.Log(result2);
        }
    }
}
