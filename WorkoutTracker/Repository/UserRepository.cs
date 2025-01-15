using MongoDB.Driver;
using WorkoutTracker.Database;
using WorkoutTracker.Model;

namespace WorkoutTracker.Repository;
public class UserRepository
{
    private readonly IMongoCollection<User> _users;
    public UserRepository(IMongoDbContext context)
    {
        _users = context.GetCollection<User>("Users");
    }
    public async Task<List<User>> GetAllAsync()
    {
        return await _users.Find(_ => true).ToListAsync();
    }
    public async Task<User> GetByIdAsync(string id)
    {
        return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
    }
    public async Task<User> GetByUserNameAsync(string name)
    {
        return await _users.Find(user => user.UserName == name).FirstOrDefaultAsync();
    }
    public async Task CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }
    public async Task UpdateAsync(string id, User updatedUser)
    {
        await _users.ReplaceOneAsync(user => user.Id == id, updatedUser);
    }
    public async Task DeleteAsync(string id)
    {
        await _users.DeleteOneAsync(user => user.Id == id);
    }
}
