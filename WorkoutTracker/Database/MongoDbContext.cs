using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using WorkoutTracker.Model;

namespace WorkoutTracker.Database;
public class MongoDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<PersonalRecord> PersonalRecords { get; set; }
    public DbSet<Workout> Workouts { get; set; }

    private readonly string _connectionString = "mongodb://localhost:27017";
    private readonly string _databaseName = "IdaModigh";

    public MongoDbContext()
    {
        EnsureDatabaseExists();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMongoDB(_connectionString, _databaseName);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Exercise>().HasKey(e => e.Id);
        modelBuilder.Entity<PersonalRecord>().HasKey(pr => pr.Id);
        modelBuilder.Entity<Workout>().HasKey(w => w.Id);

        modelBuilder.Entity<User>()
            .HasMany(u => u.FavoriteExercises)
            .WithOne()
            .HasForeignKey("UserId");
    }

    private void EnsureDatabaseExists()
    {
        // Skapa MongoDB-klienten
        var client = new MongoClient(_connectionString);
        var database = client.GetDatabase(_databaseName);

        // Kontrollera om databasen finns (kontrollerar om det finns en samling)
        var collections = database.ListCollectionNames().ToList();
        if (!collections.Any())
        {
            // Skapa en standardinsamling om ingen finns (triggar MongoDB att skapa databasen)
            database.CreateCollection("InitialCollection");

            Console.WriteLine($"Databasen '{_databaseName}' skapades.");
            // TODO: Run method creating exercises in Exercises collection
        }
        else
        {
            Console.WriteLine($"Databasen '{_databaseName}' finns redan.");
        }
    }
}
