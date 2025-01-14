using System.Collections.ObjectModel;
using System.Windows.Input;
using WorkoutTracker.Commands;
using WorkoutTracker.Model;
using WorkoutTracker.Repository;

namespace WorkoutTracker.ViewModel;
public class ExerciseDetailsViewModel : BaseViewModel
{
    private readonly ExerciseListViewModel _exerciseListViewModel;
    private readonly MuscleGroupRepository _muscleGroupRepository;
    private readonly ExerciseRepository _exerciseRepository;

    public Exercise _exercise;
    public Exercise Exercise 
    {
        get => _exercise;
        set => SetProperty(ref _exercise, value);
    }
    public ObservableCollection<MuscleGroup> MuscleGroups { get; set; }
    private ObservableCollection<MuscleGroup> _availableMuscleGroups;
    public ObservableCollection<MuscleGroup> AvailableMuscleGroups
    {
        get => _availableMuscleGroups;
        set => SetProperty(ref _availableMuscleGroups, value);
    }
    private MuscleGroup _selectedMuscleGroup;
    public MuscleGroup SelectedMuscleGroup
    {
        get => _selectedMuscleGroup;
        set => SetProperty(ref _selectedMuscleGroup, value);
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
    public ExerciseDetailsViewModel(ExerciseListViewModel exerciseListViewModel, MuscleGroupRepository muscleGroupRepository, ExerciseRepository exerciseRepository)
    {
        _exerciseListViewModel = exerciseListViewModel;
        _muscleGroupRepository = muscleGroupRepository;
        _exerciseRepository = exerciseRepository;

        AvailableMuscleGroups = new ObservableCollection<MuscleGroup>();

        GetExercise();

        SyncFavoriteStatus();

        IsFavoriteExerciseCommand = new RelayCommand(SetFavoriteStatus);
        AddMuscleGroupCommand = new RelayCommand(AddMuscleGroup);
        DeleteMuscleGroupCommand = new RelayCommand(DeleteMuscleGroup);
    }

    public async void GetExercise(Exercise selectedExercise = null)
    {
        if (selectedExercise != null)
        {
            Exercise = await _exerciseRepository.GetByNameAsync(selectedExercise.ExerciseName);

            FilterAvailableMuscleGroups();
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
        Exercise.MuscleGroups.Remove(SelectedMuscleGroup.MuscleGroupName);
        FilterAvailableMuscleGroups();
    }
    public async Task GetMuscleGroups()
    {
        var muscleGroups = await _muscleGroupRepository.GetAllAsync();
        MuscleGroups = new ObservableCollection<MuscleGroup>(muscleGroups);
    }
    private void AddMuscleGroup(object obj)
    {
        Exercise.MuscleGroups.Add(SelectedMuscleGroup.MuscleGroupName);
        FilterAvailableMuscleGroups();
    }
    private void FilterAvailableMuscleGroups()
    {
        AvailableMuscleGroups.Clear();

        foreach (var muscleGroup in MuscleGroups)
        {
            if (Exercise.MuscleGroups != null &&  !Exercise.MuscleGroups.Any(m => m.Equals(muscleGroup.MuscleGroupName)))
            {
                AvailableMuscleGroups.Add(muscleGroup);
            }
        }
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
