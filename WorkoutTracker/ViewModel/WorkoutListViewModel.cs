using System.Collections.ObjectModel;
using WorkoutTracker.Model;
using WorkoutTracker.Repository;

namespace WorkoutTracker.ViewModel;
public class WorkoutListViewModel : BaseViewModel
{
    private readonly WorkoutRepository _workoutRepository;
    private ObservableCollection<Workout> _workouts;
    public ObservableCollection<Workout> Workouts 
    {
        get => _workouts;
        set => SetProperty(ref _workouts, value);
    }
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
