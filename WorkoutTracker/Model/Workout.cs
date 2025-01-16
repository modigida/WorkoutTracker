using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WorkoutTracker.Model;
public class Workout
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string UserId { get; set; }
    public DateTime Date { get; set; }
    public DateTime EndTime { get; set; }
    public List<WorkoutExercise> Exercises { get; set; }
    public string Notes { get; set; }
}
