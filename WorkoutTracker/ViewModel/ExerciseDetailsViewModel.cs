using System.Windows.Input;
using WorkoutTracker.Commands;
using WorkoutTracker.Model;

namespace WorkoutTracker.ViewModel;
public class ExerciseDetailsViewModel : BaseViewModel
{
    public Exercise Exercise { get; set; }

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

    public ICommand IsFavoriteExerciseCommand { get; set; }

    public ExerciseDetailsViewModel()
    {
        Exercise = new Exercise
        {
            ExerciseName = "Bench Press",
            Description = "Lay flat on a bench and push the barbell up and down.",
            MuscleGroups = new List<string> { "Chest", "Triceps" },
            IsFavorite = true
        };

        SyncFavoriteStatus();

        IsFavoriteExerciseCommand = new RelayCommand(SetFavoriteStatus);
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
