using System;
using UnityEngine;

namespace TankGame
{
    public class EventListener : MonoBehaviour
    {
        private void OnEnable()
        {
            EventTrigger.Event += OnEvent;
        }

        private void OnDisable()
        {
            EventTrigger.Event -= OnEvent;
        }

        private void OnEvent(object sender, TestArgs args)
        {
            Debug.Log(args.Arg);
        }
    }
}
