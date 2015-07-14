using System;
using System.Data.SqlClient;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;
using Microsoft.Synchronization.Files;

namespace Sync_Framework_Basic_Example
{
	internal class Program
	{
		private static void Main(string[] args)
		{
            SyncOrchestrator sync = new SyncOrchestrator();
            string scopeName = "test";
            SqlConnection localData = new SqlConnection(@"Data Source=nipun;Initial Catalog=ClientData;Integrated Security=True;");
            SqlConnection serverData = new SqlConnection(@"Data Source=nipun;Initial Catalog=ServerData;Integrated Security=True;");

            SqlSyncProvider localProvider = new SqlSyncProvider(scopeName, localData);
            SqlSyncProvider serverProvider = new SqlSyncProvider(scopeName, serverData);

            SqlSyncScopeProvisioning scopeProvisionLocal = new SqlSyncScopeProvisioning(localData);
            if (!scopeProvisionLocal.ScopeExists(scopeName))
            {
                DbSyncScopeDescription scopeDesc = new DbSyncScopeDescription(scopeName);
                scopeDesc.Tables.Add(SqlSyncDescriptionBuilder.GetDescriptionForTable("abc", localData));
                scopeProvisionLocal.PopulateFromScopeDescription(scopeDesc);
                scopeProvisionLocal.SetCreateTableDefault(DbSyncCreationOption.Skip);
                scopeProvisionLocal.Apply();
            }

            SqlSyncScopeProvisioning scopeProvisionRemote = new SqlSyncScopeProvisioning(serverData);
            if (!scopeProvisionRemote.ScopeExists(scopeName))
            {
                DbSyncScopeDescription scopeDesc = SqlSyncDescriptionBuilder.GetDescriptionForScope(scopeName, localData);
                scopeProvisionRemote.PopulateFromScopeDescription(scopeDesc);
                scopeProvisionRemote.Apply();
            }

            SqlSyncScopeProvisioning romve = new SqlSyncScopeProvisioning(localData);
            sync.LocalProvider = localProvider;
            sync.RemoteProvider = serverProvider;
            SyncOperationStatistics stats = sync.Synchronize();
            Console.WriteLine("Update Data:\t\t {0}", stats.UploadChangesApplied);
            Console.WriteLine("Update Data ChangesFailed:\t\t {0}", stats.UploadChangesFailed);
            Console.WriteLine("Update Data Changes:\t\t {0}", stats.UploadChangesTotal);
            Console.ReadLine();
		}
	}
}