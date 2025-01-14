using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WorkoutTracker.Model;
public class MuscleGroup
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string MuscleGroupName { get; set; }
}
