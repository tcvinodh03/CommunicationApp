using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunicationAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommunicationAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users {get;set;}
    }
}