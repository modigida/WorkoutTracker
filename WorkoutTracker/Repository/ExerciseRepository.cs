using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Database;
using WorkoutTracker.Model;

namespace WorkoutTracker.Repository;
public class ExerciseRepository
{
    private readonly MongoDbContext _dbContext;

    public ExerciseRepository(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Skapa en ny övning
    public async Task CreateExerciseAsync(Exercise exercise)
    {
        _dbContext.Exercises.Add(exercise);
        await _dbContext.SaveChangesAsync();
    }

    // Hämta alla övningar
    public async Task<List<Exercise>> GetAllExercisesAsync()
    {
        return await _dbContext.Exercises.ToListAsync();
    }

    // Hämta en specifik övning via ID
    public async Task<Exercise> GetExerciseByIdAsync(string id)
    {
        return await _dbContext.Exercises.FirstOrDefaultAsync(e => e.Id == id);
    }

    // Hämta en specifik övning via Name
    public async Task<Exercise> GetExerciseByNameAsync(string name)
    {
        return await _dbContext.Exercises.FirstOrDefaultAsync(e => e.ExerciseName == name);
    }

    // Uppdatera en övning
    public async Task UpdateExerciseAsync(Exercise exercise)
    {
        _dbContext.Exercises.Update(exercise);
        await _dbContext.SaveChangesAsync();
    }

    // Ta bort en övning
    public async Task DeleteExerciseAsync(string id)
    {
        var exercise = await GetExerciseByIdAsync(id);
        if (exercise != null)
        {
            _dbContext.Exercises.Remove(exercise);
            await _dbContext.SaveChangesAsync();
        }
    }
}
