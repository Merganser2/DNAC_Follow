
using ComputerInfo.Models;
using Microsoft.EntityFrameworkCore;

namespace ComputerInfo.Data;

public class DataContextEF : DbContext
{
    private string _connectionString = "Server=localhost;Database=DotNetCourseDatabase;Trusted_Connection=true;TrustServerCertificate=true;";

    public DbSet<Computer>? Computer {get; set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) {
            optionsBuilder.UseSqlServer(_connectionString, options => options.EnableRetryOnFailure());
        }

        // base.OnConfiguring(optionsBuilder); 
    }

    // ModelBuilder maps model to actual table in SQL Server
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Other way:
        modelBuilder.HasDefaultSchema("TutorialAppSchema"); // then don't have to use .ToTable with Entity<T>
        modelBuilder.Entity<Computer>()
                    .HasKey(comp => comp.ComputerId);
                    //.HasNoKey(); // If Table doesn't have a primary key, this will work ?? didn't for me

        // his example didn't use <> with ToTable, but it is available, not sure what that means:
        //modelBuilder.Entity<Computer>().ToTable<Computer>("Computer", "TutorialAppSchema"); 
            // modelBuilder.Entity<Computer>().ToTable<Computer>("

        // if Schema varies from DefaultSchema, and/or TableName doesn't match Model name, can use 
        // modelBuilder.Entity<ModelName>().ToTable("TableName", "SchemaName") to rectify this

//        base.OnModelCreating(modelBuilder);
    }

}