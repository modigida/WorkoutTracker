using MongoDB.Driver;
using WorkoutTracker.Model;

namespace WorkoutTracker.Database;
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    private readonly string _connectionString = "mongodb://localhost:27017";
    private readonly string _databaseName = "IdaModigh";
    public MongoDbContext()
    {
        var client = new MongoClient(_connectionString);

        var databaseNames = client.ListDatabaseNames().ToList();

        _database = client.GetDatabase(_databaseName);

        EnsureDatabaseSetup();
    }
    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
    public void EnsureDatabaseSetup()
    {
        var collectionNames = _database.ListCollectionNames().ToList();

        if (!collectionNames.Contains("Users"))
        {
            _database.CreateCollection("Users");
        }

        if (!collectionNames.Contains("Exercises"))
        {
            _database.CreateCollection("Exercises");
            AddInitialExercises();
        }

        if (!collectionNames.Contains("Workouts"))
        {
            _database.CreateCollection("Workouts");
        }

        if (!collectionNames.Contains("PersonalRecords"))
        {
            _database.CreateCollection("PersonalRecords");
        }

        if (!collectionNames.Contains("MuscleGroups"))
        {
            _database.CreateCollection("MuscleGroups");
            AddMuscleGroups();
        }
    }
    private void AddInitialExercises()
    {
        var usersCollection = GetCollection<Exercise>("Exercises");
        usersCollection.InsertMany(new List<Exercise>
        {
            new Exercise
            {
                ExerciseName = "Bench Press",
                Description = "Press a barbell upward while lying on a bench.",
                MuscleGroups = new List<string> { "Chest", "Triceps", "Shoulders" },
                IsFavorite = true
            },
            new Exercise
            {
                ExerciseName = "Deadlift",
                Description = "Lift a barbell from the ground to a standing position.",
                MuscleGroups = new List<string> { "Back", "Legs", "Glutes" },
                IsFavorite = false
            },
            new Exercise
            {
                ExerciseName = "Squat",
                Description = "Perform a deep knee bend with a barbell on your shoulders.",
                MuscleGroups = new List<string> { "Legs", "Glutes", "Core" },
                IsFavorite = true
            },
            new Exercise
            {
                ExerciseName = "Pull-Up",
                Description = "Pull yourself up until your chin is above a bar.",
                MuscleGroups = new List<string> { "Back", "Biceps", "Shoulders" },
                IsFavorite = false
            },
            new Exercise
            {
                ExerciseName = "Overhead Press",
                Description = "Press a barbell upward from shoulder height.",
                MuscleGroups = new List<string> { "Shoulders", "Triceps", "Core" },
                IsFavorite = true
            }
        });
    }
    private void AddMuscleGroups()
    {
        var usersCollection = GetCollection<MuscleGroup>("MuscleGroups");
        usersCollection.InsertMany(new List<MuscleGroup>
        {
            new MuscleGroup { MuscleGroupName = "Chest" },
            new MuscleGroup { MuscleGroupName = "Back" },
            new MuscleGroup { MuscleGroupName = "Shoulders" },
            new MuscleGroup { MuscleGroupName = "Biceps" },
            new MuscleGroup { MuscleGroupName = "Triceps" },
            new MuscleGroup { MuscleGroupName = "Abdominals" },
            new MuscleGroup { MuscleGroupName = "Lower Back" },
            new MuscleGroup { MuscleGroupName = "Legs" },
            new MuscleGroup { MuscleGroupName = "Calves" },
            new MuscleGroup { MuscleGroupName = "Glutes" },
            new MuscleGroup { MuscleGroupName = "Core" }
        });
    }
}
