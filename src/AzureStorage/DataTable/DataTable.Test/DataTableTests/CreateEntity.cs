using Microsoft.Extensions.Configuration;

namespace Devmonster.AzureStorage.DataTable.Test;

public partial class DataTablesTest
{

    string rowkey = "91a408b2-f9d8-418e-83b6-713e74f45695";

    [Fact]
    public async Task CreateEntity()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IDataTableRepository repo = new DataTableRepository();
        repo.SetConnectionString(config.ConnectionString);

        CustomerEntity item = new()
        {
            PartitionKey = partitionKey,
            RowKey = rowkey,
            Id = "1",
            Name = name,
            Age = age,
            Email = email
        };

        try
        {
            await repo.AddOrUpdateAsync(tableName, item);
            Assert.True(true);
        }
        catch
        {
            Assert.True(false);
        }

    }

    [Fact]
    public async Task CreateRandomRowkeyEntity()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IDataTableRepository repo = new DataTableRepository();
        repo.SetConnectionString(config.ConnectionString);

        CustomerEntity item = new()
        {
            PartitionKey = partitionKey,
            RowKey = Guid.NewGuid().ToString(),
            Id = "1",
            Name = name,
            Age = age,
            Email = email
        };

        try
        {
            await repo.AddOrUpdateAsync(tableName, item);
            Assert.True(true);
        }
        catch
        {
            Assert.True(false);
        }

    }





}
