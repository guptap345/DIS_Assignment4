using DIS_Assignment4.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DIS_Assignment4.DataAccess
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Covid_Conditions> Covid_Conditions { get; set; }
        public DbSet<grp_data> grp_data { get; set; }
        public DbSet<Covid_Condition> Covid_Conditions_data { get; set; }
    }
}
