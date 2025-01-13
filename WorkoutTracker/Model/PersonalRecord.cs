using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WorkoutTracker.Model;
public class PersonalRecord
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string UserId { get; set; }
    public string ExerciseId { get; set; }
    public double MaxWeight { get; set; }
    public DateTime DateAchieved { get; set; }
}

