using Azure;
using Azure.Data.Tables;
using System.Linq.Expressions;

namespace Devmonster.AzureStorage.DataTable;


public interface IDataTableRepository
{
    /// <summary>
    /// Replaces the specified table entity of type <seealso cref="ITableEntity"/> if it exists. Creates the entity if it does not exist.
    /// </summary>
    /// <param name="tableName">Table name to use</param>
    /// <param name="entity">Entity that will be added or replaced</param>
    /// <returns></returns>
    Task<object> AddOrUpdateAsync(string tableName, ITableEntity entity);

    /// <summary>
    /// Adds a Table Entity of type <seealso cref="ITableEntity"/> into the Table.
    /// </summary>
    /// <param name="tableName">Table name to add to</param>
    /// <param name="entity">Entity that will be added</param>
    /// <returns></returns>
    Task<object> AddAsync(string tableName, ITableEntity entity);

    /// <summary>
    /// Deletes the specified Table Entity
    /// </summary>
    /// <param name="tableName">Table name to delete from</param>
    /// <param name="partitionKey">The partition key that identifies the Entity</param>
    /// <param name="rowKey">The row key that identifies the Entity</param>
    /// <returns></returns>
    Task<object> DeleteAsync(string tableName, string partitionKey, string rowKey);

    /// <summary>
    /// Gets the specified table Entity of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tableName">Table name to use</param>
    /// <param name="partitionKey">The partition key that identifies the Entity</param>
    /// <param name="rowKey">The row key that identifies the Entity</param>
    /// <returns></returns>
    Task<T> GetAsync<T>(string tableName, string partitionKey, string rowKey) where T : class, ITableEntity, new();

    Task<IEnumerable<T>> GetAllAsync<T>(string tableName) where T : class, ITableEntity, new();

    /// <summary>
    /// Gets a Table Client
    /// </summary>
    /// <param name="tableName">Table name to use</param>
    /// <returns></returns>
    Task<TableClient> GetTableClient(string tableName);

    /// <summary>
    /// Queries entities in the Table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tableName">Table name to use</param>
    /// <param name="query">An expression to filter the results of the Entities</param>
    /// <returns>Returns a collection of entity models serialized as Type <typeparamref name="T"/></returns>
    Task<IEnumerable<T>> QueryAsync<T>(string tableName, Expression<Func<T, bool>> query) where T : class, ITableEntity, new();

    /// <summary>
    /// Deletes the Table specified by the <paramref name="tableName"/> parameter
    /// </summary>
    /// <param name="tableName">Name of Table to delete</param>
    /// <returns>If the table exists, a <seealso cref="Response"/>. If the Table already does not exist, <c>null</c></returns>
    Task<object> DropTable(string tableName);

    /// <summary>
    /// Sets the instance Connection String value specified by the <paramref name="connectionString"/> parameter
    /// </summary>
    /// <param name="connectionString">Connection String to use to authenticate and connect to Azure Data Table Storage</param>
    void SetConnectionString(string connectionString);
}

public class DataTableRepository : IDataTableRepository
{
    private string _connectionString;

    public DataTableRepository()
    {

    }

    public DataTableRepository(string connnectionString)
    {
        _connectionString = connnectionString;
    }

    /// <inheritdoc/>
    public void SetConnectionString(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public async Task<TableClient> GetTableClient(string tableName)
    {
        if (string.IsNullOrEmpty(_connectionString))
            throw new ArgumentException("ConnectionString is not initialized");

        TableServiceClient serviceClient = new(_connectionString);

        var tableClient = serviceClient.GetTableClient(tableName);
        await tableClient.CreateIfNotExistsAsync();
        return tableClient;
    }

    /// <inheritdoc/>
    public async Task<T> GetAsync<T>(string tableName, string partitionKey, string rowKey) where T
        : class, ITableEntity, new()
    {

        TableClient client = await GetTableClient(tableName);

        var entity = client.GetEntity<T>(partitionKey, rowKey);

        return entity.Value as T;
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>(string tableName) where T
        : class, ITableEntity, new()
    {
        TableClient client = await GetTableClient(tableName);
        Pageable<T> entities = client.Query<T>();

        return entities.ToList();


    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> QueryAsync<T>(string tableName, Expression<Func<T, bool>> query)
        where T : class, ITableEntity, new()
    {
        TableClient client = await GetTableClient(tableName);

        Pageable<T> linqEntities = client.Query<T>(query);

        return linqEntities.ToList();
    }


    /// <inheritdoc/>
    public async Task<object> AddOrUpdateAsync(string tableName, ITableEntity entity)
    {
        TableClient client = await GetTableClient(tableName);
        var result = await client.UpsertEntityAsync(entity);
        return result;
    }

    /// <inheritdoc/>
    public async Task<object> AddAsync(string tableName, ITableEntity entity)
    {
        TableClient client = await GetTableClient(tableName);
        var result = await client.AddEntityAsync(entity);
        return result;
    }

    /// <inheritdoc/>     
    public async Task<object> DeleteAsync(string tableName, string partitionKey, string rowKey)
    {
        TableClient client = await GetTableClient(tableName);
        var result = await client.DeleteEntityAsync(partitionKey, rowKey);
        return result;
    }

    /// <inheritdoc/>     
    public async Task<object> DropTable(string tableName)
    {
        TableClient client = await GetTableClient(tableName);
        var result = await client.DeleteAsync();
        return result;
    }

}

