using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data;

public class DataContextEF : DbContext
{
    private readonly IConfiguration _config;

    public virtual DbSet<User>? Users {get; set;}
    public virtual DbSet<UserJobInfo>? UserJobInfo {get; set;}
    public virtual DbSet<UserSalary>? UserSalary {get; set;}

    public DataContextEF(IConfiguration config)
    {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){

        string? connectionString = _config.GetConnectionString("DefaultConnection");
        if (!optionsBuilder.IsConfigured){
            optionsBuilder.UseSqlServer(connectionString, options => options.EnableRetryOnFailure());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {        
        // Tell Entity Framework to use our schema instead of the default (dbo)
        modelBuilder.HasDefaultSchema("TutorialAppSchema");

        // if TableName doesn't match Model name, or Schema varies from Default,
        //   can use ToTable to specify. In this case, TableName is "Users"
        // modelBuilder.Entity<ModelName>().ToTable("TableName", "SchemaName")
        modelBuilder.Entity<User>()
                    .ToTable("Users")
                    .HasKey(user => user.UserId);
     
        modelBuilder.Entity<UserJobInfo>()
                    .HasKey(user => user.UserId);
        modelBuilder.Entity<UserSalary>()
                    .HasKey(user => user.UserId);     
    }
}