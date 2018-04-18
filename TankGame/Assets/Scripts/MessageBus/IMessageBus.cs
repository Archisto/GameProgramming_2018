using System;
using System.Collections;
using UnityEngine;

namespace TankGame.Messaging
{
    public interface IMessageBus
    {
        void Publish<TMessage>(TMessage message)
            where TMessage : IMessage;

        ISubscription<TMessage> Subscribe<TMessage>(Action<TMessage> action)
            where TMessage : IMessage;

        void Unsubscribe<TMessage>(ISubscription<TMessage> subscription)
            where TMessage : IMessage;

        void Clear();
    }
}
