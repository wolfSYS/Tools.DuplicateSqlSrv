using Microsoft.SqlServer.Management.Smo;

namespace Tools.DuplicateSqlSrv.Smo;

public sealed class SmoMgr
{
	private static readonly Lazy<SmoMgr> lazy = new Lazy<SmoMgr>(() => new SmoMgr());

	public static SmoMgr Instance
	{
		get
		{
			return lazy.Value;
		}
	}


	/// <summary>
	/// CTor
	/// </summary>
	private SmoMgr()
	{
		//
	}

	//public bool Transfer()
	//{
	//	//Create an object of Transfer class and pass
	//	//reference of source database to its construtor
	//	Transfer trsfrDB = new Transfer(dbSourceAW)
	//	{
	//		CopyAllObjects = false,
	//		CopyAllSchemas = true,

	//		//Copy all user defined data types from source to destination
	//		CopyAllUserDefinedDataTypes = true,

	//		//Copy all tables & views from source to destination
	//		CopyAllTables = true,
	//		CopyAllViews = true,

	//		//Copy data of all source tables to destination tables
	//		//It actually generates INSERT statement for destination
	//		CopyData = true,

	//		//Copy all stored procedure from source to destination
	//		CopyAllStoredProcedures = true,

	//		CopyAllUserDefinedFunctions = true,

	//		//specify the destination server name
	//		//DestinationServer = myDestinationServer.Name,
			
	//		//specify the destination database name        
	//		//DestinationDatabase = dbDestinationAW.Name
	//	};

	//	//TransferData method transfers the schema objects and data
	//	//whatever you have specified to destination database
	//	trsfrDB.TransferData();
	//}

}
