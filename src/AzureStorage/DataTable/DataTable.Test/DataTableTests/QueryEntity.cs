

namespace Devmonster.AzureStorage.DataTable.Test;

public partial class DataTablesTest
{
    [Fact]
    public async Task GetEntity()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IDataTableRepository repo = new DataTableRepository();
        repo.SetConnectionString(config.ConnectionString);

        var data = await repo.GetAsync<CustomerEntity>(tableName, partitionKey, rowkey);

        Assert.True(data.Age == age);

    }


    [Fact]
    public async Task QueryEntity()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IDataTableRepository repo = new DataTableRepository();
        repo.SetConnectionString(config.ConnectionString);

        var data = await repo.QueryAsync<CustomerEntity>(tableName, c => c.PartitionKey == partitionKey);
        Assert.True(data.Any());
    }


    [Fact]
    public async Task QuerySpecificEntity()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IDataTableRepository repo = new DataTableRepository();
        repo.SetConnectionString(config.ConnectionString);

        var data = await repo.QueryAsync<CustomerEntity>(tableName, c => c.Age == 10 && c.Email == "example@example.com");
        Assert.True(data.Any());
    }

    [Fact]
    public async Task QueryAll()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IDataTableRepository repo = new DataTableRepository();
        repo.SetConnectionString(config.ConnectionString);

        var data = await repo.GetAllAsync<CustomerEntity>(tableName);
        Assert.True(data.Any());
    }
}
