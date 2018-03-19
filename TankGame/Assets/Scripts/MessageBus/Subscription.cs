using System;
using System.Collections;
using UnityEngine;

namespace TankGame.Messaging
{
    public class Subscription<TMessage> : ISubscription<TMessage>
        where TMessage : IMessage
    {
        public Action<TMessage> Action { get; private set; }

        public IMessageBus MessageBus { get; private set; }

        public Subscription(IMessageBus messageBus, Action<TMessage> action)
        {
            if (messageBus == null)
            {
                throw new ArgumentNullException("messageBus");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            MessageBus = messageBus;
            Action = action;
        }
    }
}
