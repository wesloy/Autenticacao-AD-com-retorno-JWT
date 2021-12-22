using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SI_MicroServicos.Model
{
    public class _DbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder contextBuilder)
        {
            if (!contextBuilder.IsConfigured)
            {

                var connection = DatabaseConfiguration.ConnectionString;
                contextBuilder.UseSqlServer(connection);
            }
        }

        public _DbContext(DbContextOptions<_DbContext> options) : base(options)
        {
            //LoadDefaultUsers();
        }

        public _DbContext()
        { }

        //Models
        public DbSet<UserProfile> Users { get; set; }
        public DbSet<LogAuditoria> LogAuditorias {get;set;}
      
        public static class DatabaseConfiguration
        {
            public static string ConnectionString { get; set; }
        }
            
    }
}