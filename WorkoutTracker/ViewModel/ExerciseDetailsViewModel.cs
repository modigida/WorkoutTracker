using System.Collections.ObjectModel;
using System.Windows.Input;
using WorkoutTracker.Commands;
using WorkoutTracker.Model;

namespace WorkoutTracker.ViewModel;
public class ExerciseDetailsViewModel : BaseViewModel
{
    private readonly ExerciseListViewModel _exerciseListViewModel;
    public Exercise _exercise;
    public Exercise Exercise 
    {
        get => _exercise;
        set => SetProperty(ref _exercise, value);
    }
    public ObservableCollection<MuscleGroup> MuscleGroups { get; set; }
    private string _insertedMuscleGroup;
    public string InsertedMuscleGroup 
    {
        get => _insertedMuscleGroup;
        set => SetProperty(ref _insertedMuscleGroup, value);
    }
    private MuscleGroup _selectedMuscleGroup;
    public MuscleGroup SelectedMuscleGroup
    {
        get => _selectedMuscleGroup;
        set
        {
            if (SetProperty(ref _selectedMuscleGroup, value))
            {
                InsertedMuscleGroup = _selectedMuscleGroup.MuscleGroupName ?? string.Empty;
            }
        }
    }
    private bool _isFavoriteExercise;
    public bool IsFavoriteExercise
    {
        get => _isFavoriteExercise;
        set
        {
            if (SetProperty(ref _isFavoriteExercise, value, new[] { nameof(IsNotFavoriteExercise) }))
            {
                Exercise.IsFavorite = value;
            }
        }
    }
    public bool IsNotFavoriteExercise => !IsFavoriteExercise;
    public ICommand IsFavoriteExerciseCommand { get; }
    public ICommand AddMuscleGroupCommand { get; }
    public ICommand DeleteMuscleGroupCommand { get; }
    public ExerciseDetailsViewModel(ExerciseListViewModel exerciseListViewModel)
    {
        _exerciseListViewModel = exerciseListViewModel;

        GetMuscleGroups();

        GetExercise();
        

        SyncFavoriteStatus();

        IsFavoriteExerciseCommand = new RelayCommand(SetFavoriteStatus);
        AddMuscleGroupCommand = new RelayCommand(AddMuscleGroup);
        DeleteMuscleGroupCommand = new RelayCommand(DeleteMuscleGroup);
    }

    public void GetExercise(Exercise selectedExercise = null)
    {
        if (selectedExercise != null)
        {
            Exercise = selectedExercise;
        }
        else
        {
            Exercise = new Exercise
            {
                ExerciseName = "New Exercise",
                Description = "Insert description",
                MuscleGroups = new List<string>(),
                IsFavorite = false
            };
        }
    }

    private void DeleteMuscleGroup(object obj)
    {
        // Delete MuscleGroup from Exercise
    }
    private void GetMuscleGroups()
    {
        // Get muscle groups from database
        MuscleGroups = new ObservableCollection<MuscleGroup>
        {
            new MuscleGroup { MuscleGroupName = "Chest" },
            new MuscleGroup { MuscleGroupName = "Back" },
            new MuscleGroup { MuscleGroupName = "Shoulders" },
            new MuscleGroup { MuscleGroupName = "Biceps" },
            new MuscleGroup { MuscleGroupName = "Triceps" },
            new MuscleGroup { MuscleGroupName = "Abdominals" },
            new MuscleGroup { MuscleGroupName = "Lower Back" },
            new MuscleGroup { MuscleGroupName = "Legs" },
            new MuscleGroup { MuscleGroupName = "Calves" }
        };
    }
    private void AddMuscleGroup(object obj)
    {
        Exercise.MuscleGroups.Add(SelectedMuscleGroup.MuscleGroupName);
    }
    private void SyncFavoriteStatus()
    {
        IsFavoriteExercise = Exercise.IsFavorite;
    }
    private void SetFavoriteStatus(object obj)
    {
        IsFavoriteExercise = !IsFavoriteExercise;
    }
}
