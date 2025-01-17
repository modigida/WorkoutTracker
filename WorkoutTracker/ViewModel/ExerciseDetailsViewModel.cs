﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WorkoutTracker.Commands;
using WorkoutTracker.Model;
using WorkoutTracker.Repository;

namespace WorkoutTracker.ViewModel;
public class ExerciseDetailsViewModel : BaseViewModel
{
    private readonly ExerciseListViewModel _exerciseListViewModel;
    private readonly MainWindowViewModel _mainWindowViewModel;
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
    public ICommand SaveExerciseCommand { get; }
    public ICommand DeleteExerciseCommand { get; }
    public ExerciseDetailsViewModel(ExerciseListViewModel exerciseListViewModel, MuscleGroupRepository muscleGroupRepository, 
        ExerciseRepository exerciseRepository, MainWindowViewModel mainWindowViewModel)
    {
        _exerciseListViewModel = exerciseListViewModel;
        _muscleGroupRepository = muscleGroupRepository;
        _exerciseRepository = exerciseRepository;
        _mainWindowViewModel = mainWindowViewModel;

        AvailableMuscleGroups = new ObservableCollection<MuscleGroup>();

        IsFavoriteExerciseCommand = new RelayCommand(SetFavoriteStatus);
        AddMuscleGroupCommand = new RelayCommand(AddMuscleGroup);
        DeleteMuscleGroupCommand = new RelayCommand(DeleteMuscleGroup);
        SaveExerciseCommand = new RelayCommand(SaveExercise);
        DeleteExerciseCommand = new RelayCommand(DeleteExercise);
    }
    public async void GetExercise(Exercise selectedExercise = null)
    {
        if (selectedExercise != null)
        {
            Exercise = await _exerciseRepository.GetByNameAsync(selectedExercise.ExerciseName);
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
        SyncFavoriteStatus();
        FilterAvailableMuscleGroups();
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
    private async void SaveExercise(object obj)
    {
        if (_exerciseListViewModel.Exercises.Any(e => e.ExerciseName == Exercise.ExerciseName))
        {
            await _exerciseRepository.UpdateAsync(Exercise.Id, Exercise);
        }
        else
        {
            await _exerciseRepository.CreateAsync(Exercise);
        }
        _mainWindowViewModel.OpenExerciseList(null);
    }
    private async void DeleteExercise(object obj)
    {
        var finish = MessageBox.Show($"Delete exercise?",
                                            "Confirm",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question);

        if (finish != MessageBoxResult.Yes) return;

        await _exerciseRepository.DeleteAsync(Exercise.Id);
        _mainWindowViewModel.OpenExerciseList(null);
    }
}
