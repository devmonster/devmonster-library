
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

}
