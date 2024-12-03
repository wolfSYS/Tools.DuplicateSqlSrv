

using System;
using System.Data;
using System.Drawing.Text;
using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;

Console.ResetColor();
Console.WriteLine("Tools.DuplicateSqlSrv.Smo.Playground STARTED.");

try
{
	Console.ResetColor();

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// explore all SQL server instances on the network
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	List<string> servers = new List<string>();
	DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources();
	foreach (DataRow dr in dt.Rows)
	{
		string? instance = dr["InstanceName"].ToString();
		instance = (string.IsNullOrEmpty(instance)) ? "" : "\\" + instance;
		servers.Add(string.Concat(dr["ServerName"] ?? "localhost", instance));
	}
	foreach(var s in servers)
	{
		Console.WriteLine(s);
	}

	// and now all databases for a given server
	try
	{
		Server sqlSrvInstance = new Server(servers[0]);
		DatabaseCollection lstDb = sqlSrvInstance.Databases;
		foreach (var db in lstDb)
		{
			Console.WriteLine(db.ToString());
		}
	}
	catch
	{
		string connString = string.Format("server={0};Integrated Security = sspi", servers[0]);
		using (var con = new SqlConnection(connString))
		{
			using (var da = new SqlDataAdapter("SELECT Name FROM master.sys.databases", con))
			{
				var ds = new DataSet();
				da.Fill(ds);
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					dr["Name"].ToString();
				}
			}
		}
	}


	string stop = "";


	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// connect to sql servers
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//Server sqlSrvSource = new Server(@"ARSHADALI-PC\ARSHADALI");
	//Server sqlSrvDestination = new Server(@"ARSHADALI-LAP\ARSHADALI");
	Server sqlSrvSource = new Server(@"localhost");
	Server sqlSrvDestination = new Server(@"localhost");

	//Using windows authentication
	sqlSrvSource.ConnectionContext.LoginSecure = true;

	//Using SQL Server authentication
	//sqlSrvSource.ConnectionContext.LoginSecure = false; 
	//sqlSrvSource.ConnectionContext.Login = "SQLLogin";
	//sqlSrvSource.ConnectionContext.Password = "entry@2008";
	sqlSrvSource.ConnectionContext.Connect();

	//Using windows authentication	
	sqlSrvDestination.ConnectionContext.LoginSecure = true;

	//Using SQL Server authentication
	//sqlSrvDestination.ConnectionContext.LoginSecure = false;
	//sqlSrvDestination.ConnectionContext.Login = "SQLLogin";
	//sqlSrvDestination.ConnectionContext.Password = "entry@2008";	
	sqlSrvDestination.ConnectionContext.Connect();
	
	Database dbSource = sqlSrvSource.Databases["AdventureWorks"];
	Database dbDestination = new Database(sqlSrvDestination, "AdventureWorksNEW");

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// CREATE method will create the database on the specified server
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	dbDestination.Collation = dbSource.Collation;
	dbDestination.IsFullTextEnabled = dbSource.IsFullTextEnabled;
	dbDestination.EncryptionEnabled = dbSource.EncryptionEnabled;

	ScriptDatabase(sqlSrvSource, dbSource);	
	string x = "";



	dbDestination.Create();
	Console.WriteLine("Database [{0}] created at [{1}] server.", dbDestination.Name, sqlSrvDestination.Name);

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// TRANSFER content of source DB to destination DB
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	Console.WriteLine("Starting transfer of DB content.");
	// reference of source database to its construtor
	Transfer trsfrDB = new Transfer(dbSource);

	trsfrDB.CopyAllObjects = false;
	trsfrDB.CopyAllSchemas = true;
	trsfrDB.CopyAllDefaults = true;

	// Copy all tables from source to destination
	trsfrDB.CopyAllTables = true;
	trsfrDB.CopyAllColumnEncryptionKey = true;
	trsfrDB.CopyAllColumnMasterkey = true;
	trsfrDB.CopyAllUserDefinedDataTypes = true;
	trsfrDB.CopyAllXmlSchemaCollections = true;
	trsfrDB.CopyAllUserDefinedAggregates = true;
	trsfrDB.CopyAllUserDefinedFunctions = true;
	trsfrDB.CopyAllUserDefinedTableTypes = true;
	trsfrDB.CopyAllUserDefinedTypes = true;

	trsfrDB.CopyAllViews = true;
	trsfrDB.CopyAllSequences = true;
	trsfrDB.CopyAllDatabaseTriggers = true;

	if (dbDestination.IsFullTextEnabled)
	{
		trsfrDB.CopyAllFullTextCatalogs = true;
		trsfrDB.CopyAllFullTextStopLists = true;
		trsfrDB.Options.FullTextCatalogs = true;
		trsfrDB.Options.FullTextIndexes = true;
	}
	else
	{
		trsfrDB.CopyAllFullTextCatalogs = false;
		trsfrDB.CopyAllFullTextStopLists = false;
		trsfrDB.Options.FullTextCatalogs = false;
		trsfrDB.Options.FullTextIndexes = false;
	}

	trsfrDB.CopyAllSearchPropertyLists = true;

	trsfrDB.CopyAllLogins = false;

	// Copy data of all source tables to destination tables
	// (it actually generates INSERT statement for destination)
	trsfrDB.CopyData = true;

	trsfrDB.CopyAllStoredProcedures = true;
	trsfrDB.CopyAllSqlAssemblies = true;


	// specify the destination server name
	trsfrDB.DestinationServer = sqlSrvDestination.Name;
	// specify the destination database name
	trsfrDB.DestinationDatabase = dbDestination.Name;

	trsfrDB.Options.ContinueScriptingOnError = true;


	trsfrDB.Options.DriAll = true;
	trsfrDB.Options.Triggers = true;
	trsfrDB.Options.Indexes = true;
	trsfrDB.Options.Statistics = true;
	trsfrDB.Options.DriForeignKeys = true;
	trsfrDB.Options.DriPrimaryKey = true;
	trsfrDB.Options.DriUniqueKeys = true;
	trsfrDB.Options.IncludeIfNotExists = true;

	trsfrDB.Options.ClusteredIndexes = true;
	trsfrDB.Options.ColumnStoreIndexes = true;
	trsfrDB.Options.NonClusteredIndexes = true;
	trsfrDB.Options.SpatialIndexes = true;
	trsfrDB.Options.XmlIndexes = true;

	// wire up event handler to monitor progress
	trsfrDB.DataTransferEvent += new DataTransferEventHandler(DataTransferReport);
	trsfrDB.DiscoveryProgress += new ProgressReportEventHandler(DiscoveryProgressReport);
	trsfrDB.ScriptingProgress += new ProgressReportEventHandler(ScriptingProgressReport);
	trsfrDB.ScriptingError += new ScriptingErrorEventHandler(ScriptingErrorReport);




	/* With ScriptingOptions you can specify different scripting
	 * options, for example to include IF NOT EXISTS, DROP
	 * statements, output location etc*/
	//ScriptingOptions scriptOptions = new ScriptingOptions()
	//{
	//	ScriptForCreateDrop = true,
	//	IncludeDatabaseContext = true,
	//	IncludeHeaders = true,
	//	ScriptDrops = true,
	//	ScriptOwner = true,
	//	ScriptSchema = true,
	//	IncludeIfNotExists = true,
	//	ScriptData = true,
	//	WithDependencies = true,
	//	Indexes = true,
	//	DriAll = true,
	//	SchemaQualify = true,
	//	SchemaQualifyForeignKeysReferences = true,
	//	ScriptDataCompression = true,
	//	BatchSize = 10485760,
	//	FileName = @"W:\TransferDatabaseSchemaAndData.sql"
	//};
	//trsfrDB.Options = scriptOptions;
	//trsfrDB.ScriptTransfer();
	////IEnumerable<string> scripts = trsfrDB.EnumScriptTransfer();


	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine("STARTING TRANSFER...");
	Console.ResetColor();

	// TransferData method transfers the schema objects and data
	// whatever you have specified to destination database
	trsfrDB.TransferData();
	Console.WriteLine("Transfer of DB content suceeded.");



}
catch(SmoException smoEx)
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine("EXCEPTION: " + smoEx);
}
catch(SqlException sqlEx)
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine("EXCEPTION: " + sqlEx);
}
catch(Exception ex)
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine("EXCEPTION: " + ex);
}



Console.WriteLine("Tools.DuplicateSqlSrv.Smo.Playground ENDED.");




static void DataTransferReport(object sender, DataTransferEventArgs args)
{
	Console.ForegroundColor = ConsoleColor.Green;
	Console.Write("[" + args.DataTransferEventType + "] ");
	Console.ResetColor();
	Console.WriteLine(" : " + args.Message);
}
static void DiscoveryProgressReport(object sender, ProgressReportEventArgs args)
{
	Console.ForegroundColor = ConsoleColor.Gray;
	Console.WriteLine("[" + args.Current.Value + "]");
}
static void ScriptingProgressReport(object sender, ProgressReportEventArgs args)
{
	Console.ForegroundColor = ConsoleColor.Yellow;
	Console.WriteLine("[" + args.Current.Value + "]");
}
static void ScriptingErrorReport(object sender, ScriptingErrorEventArgs args)
{
	Console.ForegroundColor = ConsoleColor.DarkYellow;
	Console.WriteLine("[" + args.Current.Value + "]");
}




// this MAY work instead (needs some love)
static void ScriptDatabase(Server sourceSqlServer, Database sourceDatabase)
{
	//SqlConnection conn = new SqlConnection(dbConnectionString);
	//ServerConnection serverConn = new ServerConnection(conn);
	//var server = new Server(serverConn);
	//var database = server.Databases[databaseName];

	var scripterCreate = GetScripterCreate(sourceSqlServer);
	var scripterDrop = GetScripterDrop(sourceSqlServer);

	ScriptTables(sourceDatabase, scripterCreate, scripterDrop);
	ScriptViews(sourceDatabase, scripterCreate, scripterDrop);
	ScriptUserDefinedFunctions(sourceDatabase, scripterCreate, scripterDrop);
	ScriptStoredProcedures(sourceDatabase, scripterCreate, scripterDrop);
}

static void ScriptTables(Database sourceDatabase, Scripter scripterCreate, Scripter scripterDrop)
{
	List<string> scrip = new List<string>();
	foreach (Table myTable in sourceDatabase.Tables)
	{
		foreach (string s in scripterDrop.EnumScript(new Urn[] { myTable.Urn }))
			scrip.Add(s);
	}
	scrip.Add("");
	foreach (Table myTable in sourceDatabase.Tables)
	{
		foreach (string s in scripterCreate.EnumScript(new Urn[] { myTable.Urn }))
			scrip.Add(s);
	}
	System.IO.File.WriteAllLines(@$"W:\{sourceDatabase.Name}_Tables.sql", scrip);
}

static void ScriptViews(Database sourceDatabase, Scripter scripterCreate, Scripter scripterDrop)
{
	List<string> scrip = new List<string>();
	foreach (View myView in sourceDatabase.Views)
	{
		//Skip system views
		//There is a scripter.Options.AllowSystemObjects = false; setting that does the same but it is glacially slow
		if (myView.IsSystemObject)
			continue;
		foreach (string s in scripterDrop.EnumScript(new Urn[] { myView.Urn }))
			scrip.Add(s);
	}
	scrip.Add("");
	foreach (View myView in sourceDatabase.Views)
	{
		//Skip system views
		//There is a scripter.Options.AllowSystemObjects = false; setting that does the same but it is glacially slow
		if (myView.IsSystemObject)
			continue;
		foreach (string s in scripterCreate.EnumScript(new Urn[] { myView.Urn }))
			scrip.Add(s);
	}
	System.IO.File.WriteAllLines(@$"W:\{sourceDatabase.Name}_Views.sql", scrip);
}

static void ScriptStoredProcedures(Database sourceDatabase, Scripter scripterCreate, Scripter scripterDrop)
{
	List<string> scrip = new List<string>();
	foreach (StoredProcedure myStoredProc in sourceDatabase.StoredProcedures)
	{
		if (myStoredProc.IsSystemObject)
			continue;
		foreach (string s in scripterDrop.EnumScript(new Urn[] { myStoredProc.Urn }))
			scrip.Add(s);
	}
	scrip.Add("");
	foreach (StoredProcedure myStoredProc in sourceDatabase.StoredProcedures)
	{
		if (myStoredProc.IsSystemObject)
			continue;
		foreach (string s in scripterCreate.EnumScript(new Urn[] { myStoredProc.Urn }))
			scrip.Add(s);
	}
	System.IO.File.WriteAllLines(@$"W:\{sourceDatabase.Name}_StoredProcedures.sql", scrip);
}

static void ScriptUserDefinedFunctions(Database sourceDatabase, Scripter scripterCreate, Scripter scripterDrop)
{
	List<string> scrip = new List<string>();
	foreach (UserDefinedFunction myUdf in sourceDatabase.UserDefinedFunctions)
	{
		if (myUdf.IsSystemObject)
			continue;
		foreach (string s in scripterDrop.EnumScript(new Urn[] { myUdf.Urn }))
			scrip.Add(s);
	}
	scrip.Add("");
	foreach (UserDefinedFunction myUdf in sourceDatabase.UserDefinedFunctions)
	{
		if (myUdf.IsSystemObject)
			continue;
		foreach (string s in scripterCreate.EnumScript(new Urn[] { myUdf.Urn }))
			scrip.Add(s);
	}
	System.IO.File.WriteAllLines(@$"W:\{sourceDatabase.Name}_UserDefinedFunctions.sql", scrip);
}

static Scripter GetScripterCreate(Server sqlSrv)
{
	var scripter = new Scripter(sqlSrv);
	//scripter.Options.IncludeIfNotExists = true;
	scripter.Options.ScriptSchema = true;
	scripter.Options.ScriptData = true;
	//scripter.Options.WithDependencies = true;
	scripter.Options.ContinueScriptingOnError = true;
	scripter.Options.Indexes = true;
	//scripter.Options.DriAll = true;
	scripter.Options.ScriptForCreateDrop = true;  // there is NO single DROP statement?
	//scripter.Options.ScriptDrops = true;	// NO does ONLY create DROP statements
	
	scripter.ScriptingProgress += new ProgressReportEventHandler(ScriptingProgressReport);
	scripter.ScriptingError += new ScriptingErrorEventHandler(ScriptingErrorReport);

	return scripter;
}


static Scripter GetScripterDrop(Server sqlSrv)
{
	var scripter = new Scripter(sqlSrv);
	scripter.Options.ScriptDrops = true;

	return scripter;
}


