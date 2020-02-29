using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.API.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace LogWire.API.Data.Context
{
    public class DataContext : DbContext
    {

        public DbSet<RefreshTokenEntry> RefreshTokens { get; set; }

        public DataContext(DbContextOptions options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        [DbFunction]
        public static DateTime? AddDays(DateTime? dateValue, int? addValue)
        {
            // you don't need any implementation 
            throw new Exception();
        }

    }
}
