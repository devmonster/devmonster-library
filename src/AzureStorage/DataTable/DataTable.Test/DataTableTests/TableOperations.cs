namespace Devmonster.AzureStorage.DataTable.Test;

public partial class DataTablesTest
{

    [Fact]
    public async Task DeleteTable()
    {
        var config = new TestOptions();
        Startup.GetIConfigurationRoot().Bind(config);

        IDataTableRepository repo = new DataTableRepository();
        repo.SetConnectionString(config.ConnectionString);

        try
        {
            await repo.DropTable(tableName);
            Assert.True(true);
        }
        catch
        {
            Assert.True(false);
        }
    }
}
