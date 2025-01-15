using System.Windows;
using System.Windows.Input;
using WorkoutTracker.Commands;
using WorkoutTracker.Model;

namespace WorkoutTracker.ViewModel;
public class WorkoutViewModel : BaseViewModel
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    private Workout _workout;
    public Workout Workout
    {
        get => _workout;
        set => SetProperty(ref _workout, value);
    }
    public ICommand FinishWorkoutCommand { get; }
    public WorkoutViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        FinishWorkoutCommand = new RelayCommand(FinishWorkout);
    }
    public void StartNewWorkout(string userId)
    {
        Workout = new Workout
        {
            UserId = userId,
            Date = DateTime.Now,
            Exercises = new List<WorkoutExercise>(),
            Notes = "Add note",
            IsFavorite = false
        };
    }
    private void FinishWorkout(object obj)
    {
        var finish = MessageBox.Show($"Finish workout?",
                                            "Confirm",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question);

        if (finish != MessageBoxResult.Yes) return;

        Workout.EndTime = DateTime.Now;

        _mainWindowViewModel.OpenWorkoutDetails(Workout);
    }
}
