using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.DuplicateSqlSrv.Models.LocalDB
{
	public class DbConnection
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string ServerInstance { get; set; }
		public bool IsWindowsAuthentication { get; set; }
		public string LogonUserName { get; set; }


		/// <summary>
		/// Example:
		/// <code>
		///    LogonUserPwd = "abc74".Encrypt();
		///    var pwd = LogonUserPwd.Decrypt();
		/// </code>
		/// </summary>
		public string LogonUserPwd { get; set; }
	}
}
