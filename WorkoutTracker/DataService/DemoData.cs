using MongoDB.Driver;
using WorkoutTracker.Database;
using WorkoutTracker.Model;

namespace WorkoutTracker.DataService;

public class DemoData
{
    public static void AddDemoUser(MongoDbContext dbContext)
    {
        var usersCollection = dbContext.GetCollection<User>("Users");

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
            FavoriteExercises = new List<FavoriteExercise>() { new FavoriteExercise { ExerciseName = "Bench Press", TargetWeight = 45 },
            new FavoriteExercise { ExerciseName = "Squat", TargetWeight = 70 }, new FavoriteExercise { ExerciseName = "Deadlift", TargetWeight = 100 },
            new FavoriteExercise { ExerciseName = "Overhead Press", TargetWeight = 35 }}
        };
        usersCollection.InsertOne(newUser);

        var userId = newUser.Id;

        AddDemoWorkouts(userId, dbContext);
    }

    private static void AddDemoWorkouts(string userId, MongoDbContext dbContext)
    {
        var workoutsCollection = dbContext.GetCollection<Workout>("Workouts");

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

        var workout6 = new Workout
        {
            UserId = userId,
            Date = DateTime.UtcNow.AddYears(-1).AddDays(-12),
            Exercises = new List<WorkoutExercise>
            {
                new WorkoutExercise { ExerciseName = "Leg Press", Sets = new List<Set>
                {
                    new Set { Reps = 10, Weight = 60 },
                    new Set { Reps = 10, Weight = 65 }
                }
                },
                new WorkoutExercise { ExerciseName = "Overhead Press", Sets = new List<Set>
                {
                    new Set { Reps = 8, Weight = 7.5 },
                    new Set { Reps = 8, Weight = 7.5 },
                    new Set { Reps = 6, Weight = 10 }
                }
                }
                ,new WorkoutExercise { ExerciseName = "Triceps Pushdown", Sets = new List<Set>
                {
                    new Set { Reps = 8, Weight = 5 },
                    new Set { Reps = 8, Weight = 5 },
                    new Set { Reps = 6, Weight = 7.5 }
                }
                }
                ,new WorkoutExercise { ExerciseName = "Lat Pulldown", Sets = new List<Set>
                {
                    new Set { Reps = 8, Weight = 10 },
                    new Set { Reps = 8, Weight = 10 },
                    new Set { Reps = 6, Weight = 12.5 }
                }
                },
            },
            Notes = "Add note"
        };

        var workout7 = new Workout
        {
            UserId = userId,
            Date = DateTime.UtcNow.AddYears(-1).AddDays(-23),
            Exercises = new List<WorkoutExercise>
            {
                new WorkoutExercise { ExerciseName = "Leg Press", Sets = new List<Set>
                {
                    new Set { Reps = 10, Weight = 60 },
                    new Set { Reps = 10, Weight = 60 },
                    new Set { Reps = 8, Weight = 65 }
                }
                },
                new WorkoutExercise { ExerciseName = "Overhead Press", Sets = new List<Set>
                {
                    new Set { Reps = 8, Weight = 7.5 },
                    new Set { Reps = 8, Weight = 7.5 },
                    new Set { Reps = 6, Weight = 10 }
                }
                }
                ,new WorkoutExercise { ExerciseName = "Triceps Pushdown", Sets = new List<Set>
                {
                    new Set { Reps = 8, Weight = 5 },
                    new Set { Reps = 8, Weight = 5 },
                    new Set { Reps = 6, Weight = 7.5 },
                    new Set { Reps = 4, Weight = 10 }
                }
                }
                ,new WorkoutExercise { ExerciseName = "Lat Pulldown", Sets = new List<Set>
                {
                    new Set { Reps = 8, Weight = 10 },
                    new Set { Reps = 8, Weight = 10 },
                    new Set { Reps = 6, Weight = 12.5 },
                    new Set { Reps = 6, Weight = 12.5 },
                    new Set { Reps = 4, Weight = 15 },
                }
                },
            },
            Notes = "Add note"
        };

        workoutsCollection.InsertMany(new List<Workout> { workout1, workout2, workout3, workout4, workout5, workout6, workout7 });

        AddDemoPersonalRecords(userId, dbContext);
    }
    private static void AddDemoPersonalRecords(string userId, MongoDbContext dbContext)
    {
        var personalRecordsCollection = dbContext.GetCollection<PersonalRecord>("PersonalRecords");

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
