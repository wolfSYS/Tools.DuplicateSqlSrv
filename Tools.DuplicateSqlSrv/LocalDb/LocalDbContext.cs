namespace Tools.DuplicateSqlSrv.LocalDb
{
	using Microsoft.EntityFrameworkCore;

	public class LocalDbContext : DbContext
	{
		public DbSet<Models.LocalDB.DbConnection> LocalDb { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite("Data Source=wolfSYS.Tools.DuplicateSqlSrv.db");
			optionsBuilder.UseLazyLoadingProxies();
		}
	}
}
