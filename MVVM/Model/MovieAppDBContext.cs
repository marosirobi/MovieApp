using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MovieApp.MVVM.Model
{
   public class MovieAppDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=Robi\\SQL2022;Database=MovieDB;Trusted_Connection=True;TrustServerCertificate=True;"); 
        }
    }
}
