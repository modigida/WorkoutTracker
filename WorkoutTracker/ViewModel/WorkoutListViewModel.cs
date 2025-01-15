using System.Collections.ObjectModel;
using WorkoutTracker.Model;
using WorkoutTracker.Repository;

namespace WorkoutTracker.ViewModel;
public class WorkoutListViewModel
{
    private readonly WorkoutRepository _workoutRepository;
    public ObservableCollection<Workout> Workouts {  get; set; }
    public WorkoutListViewModel(WorkoutRepository workoutRepository)
    {
        _workoutRepository = workoutRepository;
    }
    public async Task GetAllWorkouts(string userId)
    {
        var workouts = await _workoutRepository.GetAllByUserIdAsync(userId);
        Workouts = new ObservableCollection<Workout>(workouts);
    }
}
