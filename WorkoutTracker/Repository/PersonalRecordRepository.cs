using MongoDB.Bson;
using MongoDB.Driver;
using System.Xml.Linq;
using WorkoutTracker.Database;
using WorkoutTracker.Model;

namespace WorkoutTracker.Repository;
public class PersonalRecordRepository
{
    private readonly IMongoCollection<PersonalRecord> _personalRecords;
    public PersonalRecordRepository(MongoDbContext context)
    {
        _personalRecords = context.GetCollection<PersonalRecord>("PersonalRecords");
    }
    public async Task<List<PersonalRecord>> GetAllAsync(string userId)
    {
        return await _personalRecords.Find(pr => pr.UserId == userId).ToListAsync();
    }
    public async Task<PersonalRecord> GetByExerciseAsync(string exercise, string userId)
    {
        return await _personalRecords.Find(pr => pr.ExerciseName == exercise && pr.UserId == userId).FirstOrDefaultAsync();
    }
    public async Task CreateAsync(PersonalRecord personalRecord)
    {
        await _personalRecords.InsertOneAsync(personalRecord);
    }
    public async Task DeleteAsync(string id)
    {
        await _personalRecords.DeleteOneAsync(pr => pr.Id == id);
    }
    public async Task<List<PersonalRecord>> GetBestRecordsAsync(string userId)
    {
        var pipeline = new[]
        {
        new BsonDocument("$match", new BsonDocument("UserId", userId)), // Filtrera på användarens ID
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", "$ExerciseName" }, // Gruppera efter övningens namn
            { "MaxWeight", new BsonDocument("$max", "$MaxWeight") }, // Hämta maxvikten
            { "DateAchieved", new BsonDocument("$first", "$DateAchieved") }, // Behåll ett representativt datum
            { "UserId", new BsonDocument("$first", "$UserId") }, // Behåll UserId för spårbarhet
        }),
        new BsonDocument("$project", new BsonDocument
        {
            { "_id", 0 }, // Dölj det automatiska "_id"-fältet
            { "ExerciseName", "$_id" }, // Mappa tillbaka gruppens ID till "ExerciseName"
            { "MaxWeight", 1 },
            { "DateAchieved", 1 },
            { "UserId", 1 },
        })
    };

        var result = await _personalRecords.Aggregate<PersonalRecord>(pipeline).ToListAsync();
        return result;
    }

}
