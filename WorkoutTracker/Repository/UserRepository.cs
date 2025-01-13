using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Database;
using WorkoutTracker.Model;

namespace WorkoutTracker.Repository;
public class UserRepository
{
    private readonly MongoDbContext _dbContext;

    public UserRepository(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Skapa en ny användare
    public async Task CreateUserAsync(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    // Hämta alla användare
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    // Hämta en specifik användare via ID
    public async Task<User> GetUserByIdAsync(string id)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    // Uppdatera en användare
    public async Task UpdateUserAsync(User user)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    // Ta bort en användare
    public async Task DeleteUserAsync(string id)
    {
        var user = await GetUserByIdAsync(id);
        if (user != null)
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
