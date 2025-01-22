using System.Collections.ObjectModel;
using WorkoutTracker.Model;
using WorkoutTracker.Repository;

namespace WorkoutTracker.ViewModel;
public class WorkoutListViewModel : BaseViewModel
{
    private readonly WorkoutRepository _workoutRepository;
    private readonly MainWindowViewModel _mainWindowViewModel;

    private ObservableCollection<Workout> _workouts;
    public ObservableCollection<Workout> Workouts 
    {
        get => _workouts;
        set => SetProperty(ref _workouts, value);
    }
    private Workout? _seletedWorkout;
    public Workout? SelectedWorkout
    {
        get => _seletedWorkout;
        set
        {
            if(SetProperty(ref _seletedWorkout, value) && SelectedWorkout != null)
            {
                _mainWindowViewModel.OpenWorkoutDetails(SelectedWorkout);
            }
        }
    }
    public WorkoutListViewModel(WorkoutRepository workoutRepository, MainWindowViewModel mainWindowViewModel)
    {
        _workoutRepository = workoutRepository;
        _mainWindowViewModel = mainWindowViewModel;
    }
    public async Task GetAllWorkouts(string userId)
    {
        var workouts = await _workoutRepository.GetAllByUserIdAsync(userId);
        var sortedWorkouts = workouts.OrderByDescending(w => w.Date).ToList();
        Workouts = new ObservableCollection<Workout>(sortedWorkouts);
    }
}

