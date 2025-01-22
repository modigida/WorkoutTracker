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
    private readonly UserViewModel _userViewModel;
    private readonly PersonalRecordRepository _personalRecordRepository;

    private double _newWeight;

    private Exercise _selectedExercise;
    public Exercise SelectedExercise
    {
        get => _selectedExercise;
        set
        {
            SetProperty(ref _selectedExercise, value);
            if (SelectedExercise != null && SelectedWorkoutExercise != null)
            {
                if (SelectedExercise.ExerciseName != SelectedWorkoutExercise.ExerciseName)
                {
                    Sets = new ObservableCollection<Set>();
                    Weight = 0;
                    Reps = 0;
                    SelectedSet = null;
                }
            }
        }
    }
    private WorkoutExercise _selectedWorkoutExercise;
    public WorkoutExercise SelectedWorkoutExercise
    {
        get => _selectedWorkoutExercise;
        set
        {
            SetProperty(ref _selectedWorkoutExercise, value);
            if (SelectedWorkoutExercise != null)
            {
                PrintExerciseToEdit();
            }
        }
    }
    private Set _selectedSet;
    public Set SelectedSet
    {
        get => _selectedSet;
        set
        {
            SetProperty(ref _selectedSet, value);
            if (SelectedSet != null)
            {
                PrintSetToEdit();
            }
        }
    }
    private double _weight = 0;
    public double Weight
    {
        get => _weight;
        set => SetProperty(ref _weight, value);
    }
    private int _reps = 0;
    public int Reps
    {
        get => _reps;
        set => SetProperty(ref _reps, value);
    }
    private ObservableCollection<WorkoutExercise> _workoutExercises;
    public ObservableCollection<WorkoutExercise> WorkoutExercises
    {
        get => _workoutExercises;
        set => SetProperty(ref _workoutExercises, value);
    }
    private ObservableCollection<Set> _sets;
    public ObservableCollection<Set> Sets
    {
        get => _sets;
        set => SetProperty(ref _sets, value);
    }
    private Workout _workout;
    public Workout Workout
    {
        get => _workout;
        set => SetProperty(ref _workout, value);
    }
    private double _totalWeight;
    public double TotalWeight
    {
        get => _totalWeight;
        set => SetProperty(ref _totalWeight, value);
    }
    private int _totalSets;
    public int TotalSets
    {
        get => _totalSets;
        set => SetProperty(ref _totalSets, value);
    }
    private int _totalReps;
    public int TotalReps
    {
        get => _totalReps;
        set => SetProperty(ref _totalReps, value);
    }
    private ObservableCollection<PersonalRecord> _personalRecords;
    public ObservableCollection<PersonalRecord> PersonalRecords
    {
        get => _personalRecords;
        set => SetProperty(ref _personalRecords, value);
    }
    public ICommand DeleteSetCommand { get; }
    public ICommand SaveWorkoutExerciseCommand { get; }
    public ICommand FinishWorkoutCommand { get; }
    public WorkoutViewModel(MainWindowViewModel mainWindowViewModel, WorkoutRepository workoutRepository, 
        ExerciseListViewModel exerciseListViewModel, UserViewModel userViewModel, PersonalRecordRepository personalRecordRepository)
    {
        _mainWindowViewModel = mainWindowViewModel;
        _workoutRepository = workoutRepository;
        _exerciseListViewModel = exerciseListViewModel;
        _userViewModel = userViewModel;
        _personalRecordRepository = personalRecordRepository;

        DeleteSetCommand = new RelayCommand<Set>(async set => await DeleteSet(set));
        SaveWorkoutExerciseCommand = new RelayCommand(SaveWorkoutExercise);
        FinishWorkoutCommand = new RelayCommand(FinishWorkout);
    }
    public async Task StartNewWorkout(string userId)
    {
        _mainWindowViewModel.IsAvailable = false;
        _mainWindowViewModel.IsMenuEnabled = false;

        await _exerciseListViewModel.GetExerciseNames();
        await GetPersonalRecords();

        Workout = new Workout
        {
            UserId = userId,
            Date = DateTime.Now,
            Exercises = new List<WorkoutExercise>(),
            Notes = "Add note"
        };
        WorkoutExercises = new ObservableCollection<WorkoutExercise>();
    }
    private async Task GetPersonalRecords()
    {
        var personalRecords = await _personalRecordRepository.GetBestRecordsAsync(_userViewModel.User.Id);
        PersonalRecords = new ObservableCollection<PersonalRecord>(personalRecords);
    }
    private void SaveWorkoutExercise(object obj)
    {
        _newWeight = Weight;
        if (!EditSet())
        {
            if (SelectedWorkoutExercise != null && SelectedWorkoutExercise.ExerciseName == SelectedExercise.ExerciseName)
            {
                var existingExercise = WorkoutExercises.FirstOrDefault(we => we.ExerciseName == SelectedWorkoutExercise.ExerciseName);
                if (existingExercise != null)
                {
                    EditSet();
                    existingExercise.Sets.Add(new Set { Weight = Weight, Reps = Reps });
                }
                else
                {
                    CreateWorkoutExercise();
                }
            }
            else
            {
                CreateWorkoutExercise();
            }
        }

        SelectedWorkoutExercise = WorkoutExercises.FirstOrDefault(name => name.ExerciseName == SelectedExercise.ExerciseName);
        if (SelectedWorkoutExercise == null) { return; }

        ManageSets();
        ManagePersonalRecord();
        CountTotalWeight();
        CountTotalSets();
        CountTotalReps();

        Weight = 0;
        Reps = 0;
    }
    private bool EditSet()
    {
        if (SelectedSet != null)
        {
            var weight = SelectedSet.Weight;
            CheckIfPersonalRecord(weight);
            SelectedSet.Weight = Weight;
            SelectedSet.Reps = Reps;
            SelectedSet = null;
            return true;
        }
        else
        {
            return false;
        }
    }
    private void ManageSets()
    {
        if (Sets == null)
        {
            Sets = new ObservableCollection<Set>(SelectedWorkoutExercise.Sets);
        }
        else
        {
            Sets.Clear();
            foreach (var set in SelectedWorkoutExercise.Sets)
            {
                Sets.Add(set);
            }
        }
    }
    private void CreateWorkoutExercise()
    {
        if (SelectedExercise == null) { return; }
        var existingExercise = WorkoutExercises.FirstOrDefault(we => we.ExerciseName == SelectedExercise.ExerciseName);
        if (existingExercise != null)
        {
            existingExercise.Sets.Add(new Set
            {
                Weight = Weight,
                Reps = Reps
            });
        }
        else
        {
            WorkoutExercises.Add(new WorkoutExercise
            {
                ExerciseName = SelectedExercise.ExerciseName,
                Sets = new List<Set>
            {
                new Set
                {
                    Weight = Weight,
                    Reps = Reps
                }
            }
            });
        }
    }
    private async void ManagePersonalRecord()
    {
        var existingRecord = PersonalRecords
            .FirstOrDefault(pr => pr.ExerciseName.Equals(SelectedExercise.ExerciseName, StringComparison.OrdinalIgnoreCase));

        if (existingRecord == null || existingRecord.MaxWeight < _newWeight) 
        {
            var personalRecord = new PersonalRecord
            {
                UserId = _userViewModel.User.Id,
                ExerciseName = SelectedExercise.ExerciseName,
                MaxWeight = _newWeight,
                DateAchieved = DateTime.Now
            };
            await SavePersonalRecord(personalRecord);
        }
    }
    private void PrintExerciseToEdit()
    {
        SelectedExercise = _exerciseListViewModel.Exercises.FirstOrDefault(e => e.ExerciseName == SelectedWorkoutExercise.ExerciseName);

        Sets = new ObservableCollection<Set>(SelectedWorkoutExercise.Sets);
        Weight = 0;
        Reps = 0;
    }
    private void PrintSetToEdit()
    {
        Weight = SelectedSet.Weight;
        Reps = SelectedSet.Reps;
    }
    private async Task DeleteSet(Set deleteSet)
    {
        if (deleteSet != null)
        {
            Sets.Remove(deleteSet);

            var workoutExercisesToRemove = new List<WorkoutExercise>();

            foreach (var workoutExercise in WorkoutExercises)
            {
                if (workoutExercise != null && workoutExercise.Sets.Contains(deleteSet))
                {
                    workoutExercise.Sets.Remove(deleteSet);
                    if (workoutExercise.Sets.Count == 0)
                    {
                        workoutExercisesToRemove.Add(workoutExercise);
                    }
                }
            }

            foreach (var workoutExercise in workoutExercisesToRemove)
            {
                WorkoutExercises.Remove(workoutExercise);
            }
        }

        CheckIfPersonalRecord(deleteSet.Weight);

        CountTotalWeight();
        CountTotalSets();
        CountTotalReps();
    }
    private async void CheckIfPersonalRecord(double weight)
    {
        if (SelectedWorkoutExercise == null) { return; }

        var personalRecords = await _personalRecordRepository.GetByExerciseAsync(SelectedWorkoutExercise.ExerciseName, _userViewModel.User.Id);

        foreach (var pr in personalRecords)
        {
            if (pr.DateAchieved.Date == DateTime.Now.Date &&
                weight == pr.MaxWeight)
            {
                await _personalRecordRepository.DeleteAsync(pr.Id);
            }
        }
    }
    private void CountTotalWeight()
    {
        TotalWeight = 0;

        foreach (var workoutExercise in WorkoutExercises)
        {
            foreach (var set in workoutExercise.Sets)
            {
                TotalWeight += set.Reps * set.Weight;
            }
        }
    }
    private void CountTotalSets()
    {
        TotalSets = 0;

        if (WorkoutExercises != null)
        {
            foreach (var workoutExercise in WorkoutExercises)
            {
                if (workoutExercise?.Sets != null)
                {
                    TotalSets += workoutExercise.Sets.Count;
                }
            }
        }
    }
    private void CountTotalReps()
    {
        TotalReps = 0;

        if (WorkoutExercises != null)
        {
            foreach (var workoutExercise in WorkoutExercises)
            {
                if (workoutExercise?.Sets != null)
                {
                    foreach (var set in workoutExercise.Sets)
                    {
                        TotalReps += set.Reps;
                    }
                }
            }
        }
    }
    private async void FinishWorkout(object obj)
    {
        var finish = MessageBox.Show($"Finish workout?",
                                            "Confirm",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question);

        if (finish != MessageBoxResult.Yes) return;

        Workout.Exercises = WorkoutExercises.ToList();
        Workout.EndTime = DateTime.Now;

        await _workoutRepository.CreateAsync(Workout);

        if (Sets != null)
        {
            Sets.Clear();
        }

        _mainWindowViewModel.IsAvailable = true;
        _mainWindowViewModel.IsMenuEnabled = true;

        _mainWindowViewModel.OpenWorkoutDetails(Workout);
    }
    private async Task SavePersonalRecord(PersonalRecord personalRecord)
    {
        await _personalRecordRepository.CreateAsync(personalRecord);
        await GetPersonalRecords();
    }
}
