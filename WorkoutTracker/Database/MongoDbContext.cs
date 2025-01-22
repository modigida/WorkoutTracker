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
        AddDemoUser();
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
    private void AddDemoUser()
    {
        var usersCollection = GetCollection<User>("Users");

        var existingUser = usersCollection.Find(u => u.UserName == "DemoUser").FirstOrDefault();
        if (existingUser != null)
        {
            Console.WriteLine("Demo user already exists.");
            return;
        }

        var newUser = new User
        {
            UserName = "DemoUser",
            DateJoined = DateTime.UtcNow.AddYears(-2).AddDays(-48),
            FavoriteExercises = new List<FavoriteExercise>() { new FavoriteExercise { ExerciseName = "Bench Press", TargetWeight = 40 },
            new FavoriteExercise { ExerciseName = "Squat", TargetWeight = 80 }, new FavoriteExercise { ExerciseName = "Deadlift", TargetWeight = 100 },
            new FavoriteExercise { ExerciseName = "Overhead Press", TargetWeight = 35 }}
        };
        usersCollection.InsertOne(newUser);

        var userId = newUser.Id;

        AddDemoWorkouts(userId);
    }

    private void AddDemoWorkouts(string userId)
    {
        var workoutsCollection = GetCollection<Workout>("Workouts");

        var workout1 = new Workout
        {
            UserId = userId,
            Date = DateTime.UtcNow.AddDays(-1),
            Exercises = new List<WorkoutExercise>
            {
                new WorkoutExercise { ExerciseName = "Bench Press", Sets = new List<Set>
                {
                    new Set { Reps = 10, Weight = 25 },
                    new Set { Reps = 10, Weight = 25 },
                    new Set { Reps = 8, Weight = 30 },
                    new Set { Reps = 6, Weight = 35 }
                }
                },
                new WorkoutExercise { ExerciseName = "Squat", Sets = new List<Set>
                {
                    new Set { Reps = 8, Weight = 50 },
                    new Set { Reps = 6, Weight = 60 },
                    new Set { Reps = 4, Weight = 70 }
                }
                }
            },
            Notes = "Felt strong today!"
        };

        var workout2 = new Workout
        {
            UserId = userId,
            Date = DateTime.UtcNow.AddDays(-25),
            Exercises = new List<WorkoutExercise>
            {
                new WorkoutExercise { ExerciseName = "Deadlift", Sets = new List<Set>
                {
                    new Set { Reps = 6, Weight = 70 },
                    new Set { Reps = 6, Weight = 75 },
                    new Set { Reps = 6, Weight = 80 }
                }
                },
                new WorkoutExercise { ExerciseName = "Overhead Press", Sets = new List<Set>
                {
                    new Set { Reps = 8, Weight = 20 },
                    new Set { Reps = 8, Weight = 20 },
                    new Set { Reps = 8, Weight = 25 },
                    new Set { Reps = 6, Weight = 25 }
                }
                }
            },
            Notes = "Great workout with two new personal records!"
        };        
        
        var workout3 = new Workout
        {
            UserId = userId,
            Date = DateTime.UtcNow.AddYears(-1).AddDays(34),
            Exercises = new List<WorkoutExercise>
            {
                new WorkoutExercise { ExerciseName = "Deadlift", Sets = new List<Set>
                {
                    new Set { Reps = 10, Weight = 55 },
                    new Set { Reps = 8, Weight = 60 },
                    new Set { Reps = 8, Weight = 60 },
                    new Set { Reps = 8, Weight = 65 },
                    new Set { Reps = 8, Weight = 65 }
                }
                },
                new WorkoutExercise { ExerciseName = "Bench Press", Sets = new List<Set>
                {
                    new Set { Reps = 12, Weight = 20 },
                    new Set { Reps = 10, Weight = 25 },
                    new Set { Reps = 8, Weight = 25 }
                }
                }
            },
            Notes = "Energy levels were low today, but I still completed the entire session."
        };

        var workout4 = new Workout
        {
            UserId = userId,
            Date = DateTime.UtcNow.AddDays(-72),
            Exercises = new List<WorkoutExercise>
            {
                new WorkoutExercise { ExerciseName = "Squat", Sets = new List<Set>
                {
                    new Set { Reps = 6, Weight = 55 },
                    new Set { Reps = 6, Weight = 55 },
                    new Set { Reps = 6, Weight = 60 },
                    new Set { Reps = 6, Weight = 65 }
                }
                },
                new WorkoutExercise { ExerciseName = "Bicep Curls", Sets = new List<Set>
                {
                    new Set { Reps = 12, Weight = 5 },
                    new Set { Reps = 10, Weight = 7.5 },
                    new Set { Reps = 8, Weight = 7.5 }
                }
                }
            },
            Notes = "Focused on technique, improved squat depth significantly."
        };       
        
        var workout5 = new Workout
        {
            UserId = userId,
            Date = DateTime.UtcNow.AddDays(-15),
            Exercises = new List<WorkoutExercise>
            {
                new WorkoutExercise { ExerciseName = "Leg Press", Sets = new List<Set>
                {
                    new Set { Reps = 10, Weight = 70 },
                    new Set { Reps = 10, Weight = 75 },
                    new Set { Reps = 10, Weight = 75 },
                    new Set { Reps = 8, Weight = 80 }
                }
                },
                new WorkoutExercise { ExerciseName = "Overhead Press", Sets = new List<Set>
                {
                    new Set { Reps = 8, Weight = 15 },
                    new Set { Reps = 8, Weight = 15 },
                    new Set { Reps = 6, Weight = 17.5 }
                }
                }
            },
            Notes = "Progress in overhead press, managed 3 reps with good form!"
        };

        workoutsCollection.InsertMany(new List<Workout> { workout1, workout2, workout3, workout4, workout5 });

        AddDemoPersonalRecords(userId);
    }
    private void AddDemoPersonalRecords(string userId)
    {
        var personalRecordsCollection = GetCollection<PersonalRecord>("PersonalRecords");

        var pr1 = new PersonalRecord
        {
            UserId = userId,
            ExerciseName = "Bench Press",
            MaxWeight = 40,
            DateAchieved = DateTime.UtcNow.AddDays(-10)
        };
        
        var pr2 = new PersonalRecord
        {
            UserId = userId,
            ExerciseName = "Overhead Press",
            MaxWeight = 25,
            DateAchieved = DateTime.UtcNow.AddDays(-25)
        };
        
        var pr3 = new PersonalRecord
        {
            UserId = userId,
            ExerciseName = "Squat",
            MaxWeight = 70,
            DateAchieved = DateTime.UtcNow.AddDays(-1)
        };
        
        var pr4 = new PersonalRecord
        {
            UserId = userId,
            ExerciseName = "Deadlift",
            MaxWeight = 80,
            DateAchieved = DateTime.UtcNow.AddDays(-25)
        };

        personalRecordsCollection.InsertMany(new List<PersonalRecord> { pr1, pr2, pr3, pr4 });
    }
}
