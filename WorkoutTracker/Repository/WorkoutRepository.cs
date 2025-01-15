using MongoDB.Driver;
using WorkoutTracker.Database;
using WorkoutTracker.Model;

namespace WorkoutTracker.Repository;
public class WorkoutRepository
{
    private readonly IMongoCollection<Workout> _workout;
    public WorkoutRepository(MongoDbContext context)
    {
        _workout = context.GetCollection<Workout>("Workouts");
    }
    public async Task<List<Workout>> GetAllAsync()
    {
        return await _workout.Find(_ => true).ToListAsync();
    }
    public async Task<List<Workout>> GetAllByUserIdAsync(string userId)
    {
        return await _workout.Find(workout => workout.UserId == userId).ToListAsync();
    }
    public async Task<Workout> GetByIdAsync(string id)
    {
        return await _workout.Find(workout => workout.Id == id).FirstOrDefaultAsync();
    }
    public async Task CreateAsync(Workout workout)
    {
        await _workout.InsertOneAsync(workout);
    }
    public async Task UpdateAsync(string id, Workout updatedWorkout)
    {
        await _workout.ReplaceOneAsync(workout => workout.Id == id, updatedWorkout);
    }
    public async Task DeleteAsync(string id)
    {
        await _workout.DeleteOneAsync(workout => workout.Id == id);
    }
}
