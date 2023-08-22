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
        void ReceiveMessage(IModel channel,Action<string> messageHandler);
        /// <summary>
        /// Creates and manages the RabbitMQ connection.
        /// </summary>
        /// <returns></returns>
        IConnection Connect();
    }
}
