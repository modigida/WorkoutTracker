using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Database;
using WorkoutTracker.Model;

namespace WorkoutTracker.Repository;
public class PersonalRecordRepository
{
    private readonly MongoDbContext _dbContext;

    public PersonalRecordRepository(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Skapa ett nytt personligt rekord
    public async Task CreatePersonalRecordAsync(PersonalRecord personalRecord)
    {
        _dbContext.PersonalRecords.Add(personalRecord);
        await _dbContext.SaveChangesAsync();
    }

    // Hämta alla personliga rekord
    public async Task<List<PersonalRecord>> GetAllPersonalRecordsAsync()
    {
        return await _dbContext.PersonalRecords.ToListAsync();
    }

    // Hämta alla personliga rekord via UserId
    public async Task<List<PersonalRecord>> GetAllPersonalRecordsByUserIdAsync(string userId)
    {
        return await _dbContext.PersonalRecords.Where(pr => pr.UserId == userId).ToListAsync();
    }

    // Hämta ett specifikt personligt rekord via ID
    public async Task<PersonalRecord> GetPersonalRecordByIdAsync(string id)
    {
        return await _dbContext.PersonalRecords.FirstOrDefaultAsync(pr => pr.Id == id);
    }

    // Uppdatera ett personligt rekord
    public async Task UpdatePersonalRecordAsync(PersonalRecord personalRecord)
    {
        _dbContext.PersonalRecords.Update(personalRecord);
        await _dbContext.SaveChangesAsync();
    }

    // Ta bort ett personligt rekord
    public async Task DeletePersonalRecordAsync(string id)
    {
        var personalRecord = await GetPersonalRecordByIdAsync(id);
        if (personalRecord != null)
        {
            _dbContext.PersonalRecords.Remove(personalRecord);
            await _dbContext.SaveChangesAsync();
        }
    }
}
