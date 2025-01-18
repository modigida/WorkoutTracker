﻿using System.Collections.ObjectModel;
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

    private Exercise _selectedExercise;
    public Exercise SelectedExercise
    {
        get => _selectedExercise;
        set => SetProperty(ref _selectedExercise, value);
    }
    private WorkoutExercise _newWorkoutExercise;
    public WorkoutExercise NewWorkoutExercise
    {
        get => _newWorkoutExercise;
        set => SetProperty(ref _newWorkoutExercise, value);
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
    private async void SaveWorkoutExercise(object obj)
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

        if (!PersonalRecords.Any(pr => pr.ExerciseName == SelectedExercise.ExerciseName) ||
            PersonalRecords.Any(pr => pr.ExerciseName == SelectedExercise.ExerciseName && pr.MaxWeight < Weight))
        {
            var personalRecord = new PersonalRecord
            {
                UserId = _userViewModel.User.Id,
                ExerciseName = SelectedExercise.ExerciseName,
                MaxWeight = Weight,
                DateAchieved = DateTime.Now
            };
            await SavePersonalRecord(personalRecord);
        }

        CountTotalWeight();
        CountTotalSets();
        CountTotalReps();

        SelectedExercise = null;
        Weight = 0;
        Reps = 0;
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
