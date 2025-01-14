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
    public async Task GetExercises()
    {
        //Exercises = new ObservableCollection<Exercise>
        //{
        //    new Exercise
        //    {
        //        ExerciseName = "Deadlift",
        //        Description = "Lift a bar with weight",
        //        MuscleGroups = new List<string> { "Back" },
        //        IsFavorite = false
        //    },
        //    new Exercise
        //    {
        //        ExerciseName = "Squat",
        //        Description = "Squat with weight on your shoulders",
        //        MuscleGroups = new List<string> { "Legs" },
        //        IsFavorite = true
        //    } 
        //};

       // var exercises = _exerciseRepository.GetAllAsync().Result;
       // Exercises = new ObservableCollection<Exercise>(exercises);


        var exerciseNames = await _exerciseRepository.GetAllExerciseNamesAsync();
        var exercises = exerciseNames.Select(name => new Exercise { ExerciseName = name }).ToList();
        Exercises = new ObservableCollection<Exercise>(exercises);
    }
}
