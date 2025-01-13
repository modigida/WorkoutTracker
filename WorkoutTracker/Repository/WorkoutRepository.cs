using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Database;
using WorkoutTracker.Model;

namespace WorkoutTracker.Repository;
public class WorkoutRepository
{
    private readonly MongoDbContext _dbContext;

    public WorkoutRepository(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Skapa ett nytt träningspass
    public async Task CreateWorkoutAsync(Workout workout)
    {
        _dbContext.Workouts.Add(workout);
        await _dbContext.SaveChangesAsync();
    }

    // Hämta alla träningspass
    public async Task<List<Workout>> GetAllWorkoutsAsync()
    {
        return await _dbContext.Workouts.ToListAsync();
    }

    // Hämta ett specifikt träningspass via ID
    public async Task<Workout> GetWorkoutByIdAsync(string id)
    {
        return await _dbContext.Workouts.FirstOrDefaultAsync(w => w.Id == id);
    }

    // Hämta ett specifikt träningspass via Date
    public async Task<Workout> GetWorkoutByDateAsync(DateTime date)
    {
        return await _dbContext.Workouts.FirstOrDefaultAsync(w => w.Date == date);
    }

    // Uppdatera ett träningspass
    public async Task UpdateWorkoutAsync(Workout workout)
    {
        _dbContext.Workouts.Update(workout);
        await _dbContext.SaveChangesAsync();
    }

    // Ta bort ett träningspass
    public async Task DeleteWorkoutAsync(string id)
    {
        var workout = await GetWorkoutByIdAsync(id);
        if (workout != null)
        {
            _dbContext.Workouts.Remove(workout);
            await _dbContext.SaveChangesAsync();
        }
    }
}
