using System;
using System.IO;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;
using SharpDataAccess.Data;
using SharpDataAccess.Data.Impl;
using SharpDataAccess.Impl;

namespace SharpSettingsStore.Tests
{
    public class BaseTests : IDisposable
    {
        private readonly IDataService _dataService;
        private readonly string _tempPathsDir;

        protected BaseTests()
        {
            _tempPathsDir = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString().Replace("-", "")}");
            Directory.CreateDirectory(_tempPathsDir);
            
            var dbPath = Path.Combine(_tempPathsDir, "data.db");
            _dataService = new DataService(new FactoryProvider(dbPath), new ConsoleDataAccessLogger(), new DataOptions());
            using (var connection = _dataService.OpenDbConnection())
            {
                connection.CreateTable<SettingV1>();
            }
        }
        
        public virtual void Dispose()
        {
            Directory.Delete(_tempPathsDir, true);
        }

        protected IDataService GetDataService()
        {
            return _dataService;
        }

        private class FactoryProvider : IDbConnectionFactoryProvider
        {
            private readonly string _dbPath;

            public FactoryProvider(string dbPath)
            {
                _dbPath = dbPath;
            }
            
            public IDbConnectionFactory BuildConnectionFactory()
            {
                return new OrmLiteConnectionFactory(_dbPath, SqliteOrmLiteDialectProvider.Instance);
            }
        }
    }
}