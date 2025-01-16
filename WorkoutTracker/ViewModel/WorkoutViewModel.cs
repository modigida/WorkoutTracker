using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WorkoutTracker.Commands;
using WorkoutTracker.Model;
using WorkoutTracker.Repository;

namespace WorkoutTracker.ViewModel;
public class WorkoutViewModel : BaseViewModel
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly ExerciseListViewModel _exerciseListViewModel;
    private readonly WorkoutRepository _workoutRepository;

    private Exercise _selectedExercise;
    public Exercise SelectedExercise
    {
        get => _selectedExercise;
        set => SetProperty(ref _selectedExercise, value);
    }
    private Workout _workout;
    public Workout Workout
    {
        get => _workout;
        set => SetProperty(ref _workout, value);
    }
    public ICommand FinishWorkoutCommand { get; }
    public WorkoutViewModel(MainWindowViewModel mainWindowViewModel, WorkoutRepository workoutRepository, ExerciseListViewModel exerciseListViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        _workoutRepository = workoutRepository;
        _exerciseListViewModel = exerciseListViewModel;

        FinishWorkoutCommand = new RelayCommand(FinishWorkout);
    }
    public async Task StartNewWorkout(string userId)
    {
        await _exerciseListViewModel.GetExerciseNames();

        Workout = new Workout
        {
            UserId = userId,
            Date = DateTime.Now,
            Exercises = new List<WorkoutExercise>(),
            Notes = "Add note",
            IsFavorite = false
        };
    }

    private async void FinishWorkout(object obj)
    {
        var finish = MessageBox.Show($"Finish workout?",
                                            "Confirm",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question);

        if (finish != MessageBoxResult.Yes) return;

        Workout.EndTime = DateTime.Now;

        await _workoutRepository.CreateAsync(Workout);

        _mainWindowViewModel.OpenWorkoutDetails(Workout);
    }
}
