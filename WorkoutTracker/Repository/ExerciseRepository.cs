using MongoDB.Driver;
using WorkoutTracker.Database;
using WorkoutTracker.Model;

namespace WorkoutTracker.Repository;
public class ExerciseRepository
{
    private readonly IMongoCollection<Exercise> _exercises;
    public ExerciseRepository(IMongoDbContext context)
    {
        _exercises = context.GetCollection<Exercise>("Exercises");
    }
    public async Task<List<Exercise>> GetAllAsync()
    {
        return await _exercises.Find(_ => true).ToListAsync();
    }
    public async Task<List<string>> GetAllExerciseNamesAsync()
    {
        return await _exercises.Find(_ => true).Project(exercise => exercise.ExerciseName).ToListAsync();
    }
    public async Task<Exercise> GetByIdAsync(string id)
    {
        return await _exercises.Find(exercise => exercise.Id == id).FirstOrDefaultAsync();
    }
    public async Task<Exercise> GetByNameAsync(string name)
    {
        return await _exercises.Find(exercise => exercise.ExerciseName == name).FirstOrDefaultAsync();
    }
    public async Task CreateAsync(Exercise exercise)
    {
        await _exercises.InsertOneAsync(exercise);
    }
    public async Task UpdateAsync(string id, Exercise updatedExercise)
    {
        await _exercises.ReplaceOneAsync(exercise => exercise.Id == id, updatedExercise);
    }
    public async Task DeleteAsync(string id)
    {
        await _exercises.DeleteOneAsync(exercise => exercise.Id == id);
    }
}
