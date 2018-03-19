using System;
using System.Collections;
using UnityEngine;

namespace TankGame.Messaging
{
    public interface ISubscription<TMessage>
        where TMessage : IMessage
    {
        Action<TMessage> Action { get; }
        IMessageBus MessageBus { get; }
    }
}
