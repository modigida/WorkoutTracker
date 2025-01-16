using System.Collections.ObjectModel;
using WorkoutTracker.Model;
using WorkoutTracker.Repository;

namespace WorkoutTracker.ViewModel;
public class ExerciseListViewModel : BaseViewModel
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly ExerciseRepository _exerciseRepository;
    private ObservableCollection<Exercise> _exercises;
    public ObservableCollection<Exercise> Exercises 
    {
        get => _exercises;
        set => SetProperty(ref _exercises, value);
    }
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
    public ExerciseListViewModel(MainWindowViewModel mainWindowViewModel, ExerciseRepository exerciseRepository)
    {
        _mainWindowViewModel = mainWindowViewModel;
        _exerciseRepository = exerciseRepository;

        //GetExercises();
    }
    public async Task GetExerciseNames()
    {
        var exerciseNames = await _exerciseRepository.GetAllExerciseNamesAsync();
        var exercises = exerciseNames.Select(name => new Exercise { ExerciseName = name }).ToList();
        Exercises = new ObservableCollection<Exercise>(exercises);
    }
}
