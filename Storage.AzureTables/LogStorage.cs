using System.Collections;

using Azure.Data.Tables;

using DioRed.Common.AzureStorage;
using DioRed.Murka.Storage.Contracts;

namespace DioRed.Murka.Storage.AzureTables
{
    public class LogStorage : ILogStorage
    {
        private readonly TableClient _tableClient;

        public LogStorage(TableClient tableClient)
        {
            _tableClient = tableClient;
        }

        public void Log(string level, string message, object? argument = null, Exception? exception = null)
        {
            TableEntity entity = new()
            {
                PartitionKey = level,
                RowKey = StorageHelper.GenerateId(),
                Message = message,
                Argument = GetArgumentString(argument),
                Exception = GetExceptionString(exception)
            };

            _tableClient.AddEntity(entity);
        }

        private static string? GetArgumentString(object? argumentObject)
        {
            return argumentObject is IEnumerable enumerable
                ? string.Join(", ", enumerable)
                : argumentObject?.ToString();
        }

        private static string? GetExceptionString(Exception? exception)
        {
            if (exception is null)
            {
                return null;
            }

            return exception.GetType().Name + " — " + exception.Message;
        }

        private class TableEntity : BaseTableEntity
        {
            public string Message { get; set; } = default!;
            public string? Argument { get; set; }
            public string? Exception { get; set; }
        }
    }
}
