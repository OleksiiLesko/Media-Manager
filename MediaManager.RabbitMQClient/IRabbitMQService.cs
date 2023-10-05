using MediaManager.Common;
using MediaManager.Domain.DTOs;
using RabbitMQ.Client;

namespace MediaManager.RabbitMQClient
{
    /// <summary>
    /// Represents a service interface for interacting with RabbitMQ.
    /// </summary>
    public interface IRabbitMQService
    {
        /// <summary>
        /// Starts receiving messages from RabbitMQ.
        /// </summary>
        void ReceiveMessage(IModel channel, Action<string, ulong> messageHandler);
        /// <summary>
        /// Creates and manages the RabbitMQ connection.
        /// </summary>
        /// <returns></returns>
        IConnection Connect();
        /// <summary>
        /// Sends acknowledgment for a successfully processed message.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="deliveryTag"></param>
        void AcknowledgeMessage(IModel channel, ulong deliveryTag);
    }
}
