using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Newtonsoft.Json;
using System.Text;

namespace Devmonster.AzureStorage.Queue;
public interface IQueueRepository
{

    /// <summary>
    /// Sets the instance Connection String value specified by the <paramref name="connectionString"/> parameter
    /// </summary>
    /// <param name="connectionString">Connection String to use to authenticate and connect to Azure Data Table Storage</param>
    void SetConnectionString(string connectionString);

    /// <summary>
    /// Adds a new serialized Type of <typeparamref name="T"/> message to the back of the Queue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queueBody">Message that will be serialized</param>
    /// <param name="queueName">Name of Queue to use</param>
    /// <returns></returns>
    Task AddMessageQueueAsync<T>(string queueName, T queueBody);

    Task AddMessageQueueAsync<T>(string queueName, T queueBody, TimeSpan? visibilitySpan);

    Task AddMessageQueueAsync<T>(string queueName, T queueBody, TimeSpan? visibilitySpan, TimeSpan? timeToLive);

    /// <summary>
    /// Receives one message in the front of the queue as Type <typeparamref name="T"/> and permanently removes the message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queueName">Name of Queue to use</param>
    /// <returns>Queue entity serialized as Type <typeparamref name="T"/></returns>
    Task<T> DequeueMessageAsync<T>(string queueName);

    /// <summary>
    /// Retrieves one message in the front of the queue as string and permanently removes the message
    /// </summary>
    /// <param name="queueName">Name of queue to use</param>
    /// <returns>Queue entity as string</returns>
    Task<string> DequeueMessageAsStringAsync(string queueName);

    /// <summary>
    /// Retrieves one message in the front of the queue as Type <typeparamref name="T"/> but does not alter the visibility of the message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queueName">Name of queue to use</param>
    /// <returns>Queue entity serialized as Type <typeparamref name="T"/></returns>
    Task<T> PeekMessageAsync<T>(string queueName);

    /// <summary>
    /// Retrieves one message in the front of the queue as string but does not alter the visibility of the message
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queueName">Name of queue to use</param>
    /// <returns>Queue entity as string</returns>
    Task<string> PeekMessageAsStringAsync<T>(string queueName);

    /// <summary>
    /// Initiate a QueueClient which will be used to manipulate the queue
    /// </summary>
    /// <param name="queueName">Queue name to use</param>
    /// <returns><seealso cref="QueueClient"/> instance</returns>
    QueueClient GetQueueClient(string queueName);
}

/// <inheritdoc/>     
public class QueueRepository : IQueueRepository
{
    private string _connectionString;

    public QueueRepository()
    {

    }

    /// <inheritdoc/>
    public void SetConnectionString(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public async Task AddMessageQueueAsync<T>(string queueName, T queueBody)
    {

        QueueClient queueClient = GetQueueClient(queueName);

        queueClient.CreateIfNotExists();

        if (queueClient.Exists())
        {
            string queuePayload = JsonConvert.SerializeObject(queueBody);
            await queueClient.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(queuePayload)));
        }
    }

    public async Task AddMessageQueueAsync<T>(string queueName, T queueBody, TimeSpan? visibilitySpan)
    {
        QueueClient queueClient = GetQueueClient(queueName);

        queueClient.CreateIfNotExists();

        if (queueClient.Exists())
        {
            string queuePayload = JsonConvert.SerializeObject(queueBody);
            await queueClient.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(queuePayload)), visibilitySpan);
        }
    }


    public async Task AddMessageQueueAsync<T>(string queueName, T queueBody, TimeSpan? visibilitySpan, TimeSpan? timeToLive)
    {
        QueueClient queueClient = GetQueueClient(queueName);

        queueClient.CreateIfNotExists();

        if (queueClient.Exists())
        {
            string queuePayload = JsonConvert.SerializeObject(queueBody);
            await queueClient.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(queuePayload)), visibilitySpan, timeToLive);
        }
    }

    /// <inheritdoc/>
    public async Task<T> DequeueMessageAsync<T>(string queueName)
    {

        QueueClient queueClient = GetQueueClient(queueName);

        if (queueClient.Exists() == false) return default;

        QueueMessage retrievedMessage = await queueClient.ReceiveMessageAsync();

        if (retrievedMessage is null) return default;

        await queueClient.DeleteMessageAsync(retrievedMessage.MessageId, retrievedMessage.PopReceipt);

        return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(Convert.FromBase64String(retrievedMessage.Body.ToString())));

    }

    /// <inheritdoc/>
    public async Task<string> DequeueMessageAsStringAsync(string queueName)
    {

        QueueClient queueClient = GetQueueClient(queueName);

        if (queueClient.Exists() == false) return default;

        QueueMessage retrievedMessage = await queueClient.ReceiveMessageAsync();

        if (retrievedMessage is null) return default;

        await queueClient.DeleteMessageAsync(retrievedMessage.MessageId, retrievedMessage.PopReceipt);

        return retrievedMessage.Body.ToString();

    }

    /// <inheritdoc/>
    public async Task<T> PeekMessageAsync<T>(string queueName)
    {
        QueueClient queueClient = GetQueueClient(queueName);

        if (queueClient.Exists() == false) return default;

        PeekedMessage retrievedMessage = await queueClient.PeekMessageAsync();

        if (retrievedMessage is null) return default;

        return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(Convert.FromBase64String(retrievedMessage.Body.ToString())));

    }

    /// <inheritdoc/>
    public async Task<string> PeekMessageAsStringAsync<T>(string queueName)
    {
        QueueClient queueClient = GetQueueClient(queueName);

        if (queueClient.Exists() == false) return default;

        PeekedMessage retrievedMessage = await queueClient.PeekMessageAsync();

        if (retrievedMessage is null) return default;

        return retrievedMessage.Body.ToString();

    }

    /// <inheritdoc/>
    public QueueClient GetQueueClient(string queueName)
    {
        if (string.IsNullOrEmpty(_connectionString))
            throw new ArgumentException("ConnectionString is not initialized");

        QueueClient queueClient = new(_connectionString, queueName);
        queueClient.CreateIfNotExists();

        return queueClient;
    }


}
