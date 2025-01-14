using System.Collections.ObjectModel;
using System.Windows.Input;
using WorkoutTracker.Commands;
using WorkoutTracker.Model;

namespace WorkoutTracker.ViewModel;
public class ExerciseListViewModel : BaseViewModel
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    public ObservableCollection<Exercise> Exercises { get; set; }
    private Exercise? _selectedExercise;
    public Exercise? SelectedExercise 
    { 
        get => _selectedExercise;
        set
        {
            if(SetProperty(ref _selectedExercise, value))
            {
                if(_selectedExercise != null)
                {
                    _mainWindowViewModel.OpenExerciseDetails(null);
                }
            }
        }
    }
    public ExerciseListViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        Exercises = new ObservableCollection<Exercise>
        {
            new Exercise
            {
                ExerciseName = "Deadlift",
                Description = "Lift a bar with weight",
                MuscleGroups = new List<string> { "Back" },
                IsFavorite = false
            },
            new Exercise
            {
                ExerciseName = "Squat",
                Description = "Squat with weight on your shoulders",
                MuscleGroups = new List<string> { "Legs" },
                IsFavorite = true
            } 
        };
    }
}
