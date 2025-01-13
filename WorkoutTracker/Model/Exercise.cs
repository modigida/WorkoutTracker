using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WorkoutTracker.Model;
public class Exercise
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string ExerciseName { get; set; }
    public string Description { get; set; }
    public List<string> MuscleGroups { get; set; }
    public bool IsFavorite { get; set; }
}
