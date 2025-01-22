using MongoDB.Driver;
using WorkoutTracker.DataService;
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
        DemoData.AddDemoUser(this);
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
                Description = "Press a barbell upward while lying on a bench to target the chest and triceps.",
                MuscleGroups = new List<string> { "Chest", "Triceps", "Shoulders" },
                IsFavorite = true
            },
            new Exercise
            {
                ExerciseName = "Deadlift",
                Description = "Lift a barbell from the ground to a standing position, engaging multiple muscle groups.",
                MuscleGroups = new List<string> { "Back", "Legs", "Glutes", "Hamstrings" },
                IsFavorite = false
            },
            new Exercise
            {
                ExerciseName = "Squat",
                Description = "Perform a deep knee bend with a barbell on your shoulders to strengthen the lower body.",
                MuscleGroups = new List<string> { "Legs", "Glutes", "Core" },
                IsFavorite = true
            },
            new Exercise
            {
                ExerciseName = "Bicep Curls",
                Description = "Curl a barbell or dumbbell to work the biceps.",
                MuscleGroups = new List<string> { "Biceps"},
                IsFavorite = false
            },
            new Exercise
            {
                ExerciseName = "Overhead Press",
                Description = "Press a barbell upward from shoulder height to strengthen the upper body.",
                MuscleGroups = new List<string> { "Shoulders", "Triceps", "Core" },
                IsFavorite = true
            },
            new Exercise
            {
                ExerciseName = "Lat Pulldown",
                Description = "Pull a weighted bar down towards your chest while seated to target the back and biceps.",
                MuscleGroups = new List<string> { "Back", "Biceps" },
                IsFavorite = false
            },
            new Exercise
            {
                ExerciseName = "Leg Press",
                Description = "Push a weighted platform away from your body using your legs to strengthen the lower body.",
                MuscleGroups = new List<string> { "Legs", "Glutes", "Calves", "Hamstrings" },
                IsFavorite = true
            },
            new Exercise
            {
                ExerciseName = "Triceps Pushdown",
                Description = "Push a cable attachment downward to target the triceps and improve arm strength.",
                MuscleGroups = new List<string> { "Triceps", "Shoulders", "Core" },
                IsFavorite = false
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
            new MuscleGroup { MuscleGroupName = "Core" },
            new MuscleGroup { MuscleGroupName = "Hamstrings" }
        });
    }
    
}
