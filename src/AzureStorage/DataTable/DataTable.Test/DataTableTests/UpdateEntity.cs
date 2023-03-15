namespace Devmonster.AzureStorage.DataTable.Test;

public partial class DataTablesTest
{
    [Fact]
    public async Task UpdateEntity()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IDataTableRepository repo = new DataTableRepository();
        repo.SetConnectionString(config.ConnectionString);

        var data = await repo.GetAsync<CustomerEntity>(tableName, partitionKey, rowkey);
        data.Age = 55;

        await repo.AddOrUpdateAsync(tableName, data);

        var dataUpdated = await repo.GetAsync<CustomerEntity>(tableName, partitionKey, rowkey);

        Assert.True(dataUpdated.Age == 55);

    }
}
