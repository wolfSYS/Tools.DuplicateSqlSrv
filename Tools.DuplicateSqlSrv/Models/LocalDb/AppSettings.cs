using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.DuplicateSqlSrv.Models.LocalDb
{
	public class AppSettings
	{
		public int Id { get; set; }
		public bool BrowseSqlServers { get; set; }
		public bool BrowseDatabases { get; set; }
		public bool themeIsDark { get; set; }
	}
}
