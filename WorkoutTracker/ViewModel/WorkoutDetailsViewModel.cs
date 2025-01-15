using WorkoutTracker.Model;
using WorkoutTracker.Repository;

namespace WorkoutTracker.ViewModel;
public class WorkoutDetailsViewModel : BaseViewModel
{
    private readonly WorkoutRepository _workoutRepository;
    private Workout _workout;
    public Workout Workout
    {
        get => _workout;
        set => SetProperty(ref _workout, value);
    }
    private Workout _selectedWorkout;
    public Workout SelectedWorkout
    {
        get => _selectedWorkout;
        set => SetProperty(ref _selectedWorkout, value);
    }

    private TimeSpan _workoutLength;
    public TimeSpan WorkoutLength
    {
        get => _workoutLength;
        set
        {
            _workoutLength = Workout.EndTime - Workout.Date;
            if (_workoutLength.TotalMinutes > 60)
            {
                _workoutLength = TimeSpan.FromMinutes(_workoutLength.TotalMinutes);
            }
            OnPropertyChanged(nameof(WorkoutLength));
        }
    }
    public WorkoutDetailsViewModel(WorkoutRepository workoutRepository)
    {
        _workoutRepository = workoutRepository;
    }
    public async Task GetWorkout(Workout workout)
    {
        Workout = workout;
        //Workout = await _workoutRepository.GetByIdAsync(workout.Id);
    }
}
