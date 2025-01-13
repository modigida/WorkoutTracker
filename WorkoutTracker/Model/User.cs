using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WorkoutTracker.Model;
public class User
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string UserName { get; set; }
    public DateTime DateJoined { get; set; }
    public List<FavoriteExercise>? FavoriteExercises { get; set; }
}
