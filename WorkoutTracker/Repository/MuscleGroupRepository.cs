using MongoDB.Driver;
using WorkoutTracker.Database;
using WorkoutTracker.Model;

namespace WorkoutTracker.Repository;
public class MuscleGroupRepository
{
    private readonly IMongoCollection<MuscleGroup> _muscleGroup;
    public MuscleGroupRepository(IMongoDbContext context)
    {
        _muscleGroup = context.GetCollection<MuscleGroup>("MuscleGroups");
    }
    public async Task<List<MuscleGroup>> GetAllAsync()
    {
        return await _muscleGroup.Find(_ => true).ToListAsync();
    }
}
