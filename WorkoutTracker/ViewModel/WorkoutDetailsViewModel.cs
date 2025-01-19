using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Markup;
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
    private ObservableCollection<WorkoutExercise> _exercises;
    public ObservableCollection<WorkoutExercise> Exercises
    {
        get => _exercises;
        set => SetProperty(ref _exercises, value);
    }
    private WorkoutExercise _selectedExercise;
    public WorkoutExercise SelectedExercise
    {
        get => _selectedExercise;
        set => SetProperty(ref _selectedExercise, value);
    }
    private string _note;
    public string Note
    {
        get => _note;
        set
        {
            SetProperty(ref _note, value);
            if (Note != Workout.Notes || Note != "Add note")
            {
                Workout.Notes = Note;
                UpdateWorkout();
            }
        }
    }

    private async void UpdateWorkout()
    {
        await _workoutRepository.UpdateAsync(Workout.Id, Workout);
    }

    private TimeSpan _workoutLength;
    public TimeSpan WorkoutLength
    {
        get => _workoutLength;
        set => SetProperty(ref _workoutLength, value);
    }
    public WorkoutDetailsViewModel(WorkoutRepository workoutRepository)
    {
        _workoutRepository = workoutRepository;
    }
    public async Task GetWorkout(Workout workout)
    {
        Workout = await _workoutRepository.GetByIdAsync(workout.Id);
        WorkoutLength = Workout.EndTime - Workout.Date;
        Note = Workout.Notes;

        if (_workoutLength.TotalMinutes > 60)
        {
            _workoutLength = TimeSpan.FromMinutes(_workoutLength.TotalMinutes);
        }
        GetExercises();
    }
    private void GetExercises()
    {
        if (Workout?.Exercises == null)
        {
            Exercises = new ObservableCollection<WorkoutExercise>();
            return;
        }

        var groupedExercises = Workout.Exercises
            .GroupBy(ex => ex.ExerciseName)
            .Select(group => new WorkoutExercise
            {
                ExerciseName = group.Key,
                Sets = group.SelectMany(ex => ex.Sets).ToList()
            });

        Exercises = new ObservableCollection<WorkoutExercise>(groupedExercises);
    }
}
