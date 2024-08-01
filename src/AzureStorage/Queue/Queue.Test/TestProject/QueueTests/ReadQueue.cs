namespace Devmonster.AzureStorage.Test.Queues;

public partial class QueueTests
{
    [Fact]
    public async Task ReadQueue()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IQueueRepository queueRepository = new QueueRepository();
        queueRepository.SetConnectionString(config.ConnectionString);

        try
        {
            var data = await queueRepository.DequeueMessageAsync<CustomerEntity>(queueName);

            Assert.True(data is not null);
        }
        catch (Exception)
        {
            Assert.True(false);
        }

    }

    [Fact]
    public async Task PeekQueue()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IQueueRepository queueRepository = new QueueRepository();
        queueRepository.SetConnectionString(config.ConnectionString);

        try
        {
            var data = await queueRepository.PeekMessageAsync<CustomerEntity>(queueName);

            Assert.True(data is not null);
        }
        catch (Exception ex)
        {
            Assert.True(false);
        }

    }

    [Fact]
    public async Task ClearQueueItems()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IQueueRepository queueRepository = new QueueRepository();
        queueRepository.SetConnectionString(config.ConnectionString);

        await queueRepository.ClearQueueAsync(queueName);

        var itemCount = await queueRepository.GetApproximateMessageCount(queueName);
        Assert.True(itemCount == 0);
    }

    [Fact]
    public async Task GetQueueItemCount()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IQueueRepository queueRepository = new QueueRepository();
        queueRepository.SetConnectionString(config.ConnectionString);

        await queueRepository.ClearQueueAsync(queueName);

        CustomerEntity customer = new()
        {
            Name = name,
            Age = age,
            Email = email
        };
        
        await queueRepository.AddMessageQueueAsync(queueName, customer);
        var items = await queueRepository.GetApproximateMessageCount(queueName);

        Assert.True(items > 0);
    }
}