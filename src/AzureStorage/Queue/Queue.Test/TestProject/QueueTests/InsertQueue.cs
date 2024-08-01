
namespace Devmonster.AzureStorage.Test.Queues;

public partial class QueueTests
{
    [Fact]
    public async Task InsertToQueue()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IQueueRepository queueRepository = new QueueRepository();
        queueRepository.SetConnectionString(config.ConnectionString);

        CustomerEntity customer = new()
        {
            Name = name,
            Age = age,
            Email = email
        };

        try
        {
            await queueRepository.AddMessageQueueAsync(queueName, customer);
            Assert.True(true);
        }
        catch
        {
            Assert.True(false);
        }
    }

    [Fact]
    public async Task InsertToQueueWithSchedule()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IQueueRepository queueRepository = new QueueRepository();
        queueRepository.SetConnectionString(config.ConnectionString);

        CustomerEntity customer = new()
        {
            Name = name,
            Age = age,
            Email = email
        };

        try
        {
            await queueRepository.AddMessageQueueAsync(queueName, customer, TimeSpan.FromSeconds(30));
            Assert.True(true);
        }
        catch
        {
            Assert.True(false);
        }
    }

    [Fact]
    public async Task InsertToQueueImmediatelyWithTimeToLive()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IQueueRepository queueRepository = new QueueRepository();
        queueRepository.SetConnectionString(config.ConnectionString);

        CustomerEntity customer = new()
        {
            Name = name,
            Age = age,
            Email = email
        };

        try
        {
            await queueRepository.AddMessageQueueAsync(queueName, customer, null, TimeSpan.FromSeconds(20));

            Assert.True(true);
        }
        catch
        {
            Assert.True(false);
        }
    }

    [Fact]
    public async Task InsertToQueueWithDelayAndTimeToLive()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IQueueRepository queueRepository = new QueueRepository();
        queueRepository.SetConnectionString(config.ConnectionString);

        CustomerEntity customer = new()
        {
            Name = name,
            Age = age,
            Email = email
        };

        try
        {
            await queueRepository.AddMessageQueueAsync(queueName, customer, TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(40));

            Assert.True(true);
        }
        catch
        {
            Assert.True(false);
        }
    }


}
