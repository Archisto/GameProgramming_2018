using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TankGame.Messaging
{
    /// <summary>
    /// A system for delivering messages and firing events based on them.
    /// </summary>
    public class MessageBus : IMessageBus
    {
        private readonly Dictionary<Type, IList> subscriptions =
            new Dictionary<Type, IList>();

        /// <summary>
        /// Publishes a message of a certain type.
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <param name="message">A message</param>
        public void Publish<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            Type messageType = typeof(TMessage);
            if (subscriptions.ContainsKey(messageType))
            {
                // Creates a copy of the subscriptions list to prevent errors
                // if the subscription action causes an unsubscription
                var subscriptionsList = new List<ISubscription<TMessage>>(
                    subscriptions[messageType].Cast<ISubscription<TMessage>>());

                foreach (var subscription in subscriptionsList)
                {
                    subscription.Action(message);
                }

                // Normal implementation:
                /*
                var subscriptionsList = subscriptions[messageType].
                    Cast<ISubscription<TMessage>>();

                foreach (var subscription in subscriptionsList)
                {
                    subscription.Action(message);
                }
                */
            }
        }

        /// <summary>
        /// Creates a new subscription to the message bus.
        /// </summary>
        /// <typeparam name="TMessage">A mesage type</typeparam>
        /// <param name="action">A method that is called when a
        /// subscribed message is delivered</param>
        /// <returns>A message bus subscription</returns>
        public ISubscription<TMessage> Subscribe<TMessage>
            (Action<TMessage> action)
            where TMessage : IMessage
        {
            Type messageType = typeof(TMessage);

            Subscription<TMessage> subscription =
                new Subscription<TMessage>(this, action);

            if (subscriptions.ContainsKey(messageType))
            {
                // The subscription is added the a list of
                // subsriptions with the same message type
                subscriptions[messageType].Add(subscription);
            }
            else
            {
                // A new subscription list is created for the new
                // message type and the subscription is added to it
                subscriptions.Add(messageType,
                    new List<ISubscription<TMessage>>() { subscription });
            }

            return subscription;
        }

        /// <summary>
        /// Unsubscribes from the message bus.
        /// </summary>
        /// <typeparam name="TMessage">A message type</typeparam>
        /// <param name="subscription">A subscription that should
        /// be unsubscribed from</param>
        public void Unsubscribe<TMessage>(ISubscription<TMessage> subscription)
            where TMessage : IMessage
        {
            Type messageType = typeof(TMessage);

            // Unsubsribes only if the subscription's message
            // type is contained in the subscriptions
            if (subscriptions.ContainsKey(messageType))
            {
                subscriptions[messageType].Remove(subscription);
            }
        }

        /// <summary>
        /// Unsubscribes all subscriptions.
        /// </summary>
        public void Clear()
        {
            subscriptions.Clear();
        }
    }
}
