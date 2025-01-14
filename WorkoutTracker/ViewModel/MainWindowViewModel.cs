using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WorkoutTracker.Commands;
using WorkoutTracker.Model;

namespace WorkoutTracker.ViewModel;
public class MainWindowViewModel : BaseViewModel
{
    public ExerciseDetailsViewModel ExerciseDetailsVM { get; set; }
    public ExerciseListViewModel ExerciseListVM { get; set; }
    public StatisticsViewModel StatisticsVM { get; set; }
    public UserViewModel UserVM { get; set; }
    public WorkoutListViewModel WorkoutListVM { get; set; }
    public WorkoutViewModel WorkoutVM { get; set; }

    private bool _isStartVisible;
    public bool IsStartVisible
    {
        get => _isStartVisible;
        set => SetProperty(ref _isStartVisible, value);
    }
    private bool _IsExerciseDetailsVisible;
    public bool IsExerciseDetailsVisible
    {
        get => _IsExerciseDetailsVisible;
        set => SetProperty(ref _IsExerciseDetailsVisible, value);
    }
    private bool _isExerciseListVisible;
    public bool IsExerciseListVisible 
    {
        get => _isExerciseListVisible;
        set => SetProperty(ref _isExerciseListVisible, value);
    }
    private bool _isStatisticsVisible;
    public bool IsStatisticsVisible 
    { 
        get => _isStatisticsVisible;
        set => SetProperty(ref _isStatisticsVisible, value);
    }
    private bool _isUserVisible;
    public bool IsUserVisible
    {
        get => _isUserVisible;
        set => SetProperty(ref _isUserVisible, value);
    }
    private bool _isWorkoutListVisible;
    public bool IsWorkoutListVisible
    {
        get => _isWorkoutListVisible;
        set => SetProperty(ref _isWorkoutListVisible, value);
    }
    private bool _isWorkoutVisible;
    public bool IsWorkoutVisible
    {
        get => _isWorkoutVisible;
        set => SetProperty(ref _isWorkoutVisible, value);
    }
    public string StartText { get; set; }

    public ICommand OpenExerciseDetailsCommand { get; }
    public ICommand OpenExerciseListCommand { get; }
    public ICommand OpenStatisticsCommand { get; }
    public ICommand OpenUserCommand { get; }
    public ICommand OpenWorkoutListCommand { get; }
    public ICommand OpenWorkoutCommand { get; }
    public MainWindowViewModel()
    {
        IsStartVisible = true;
        StartText = "Choose a user to start";

        ExerciseDetailsVM = new ExerciseDetailsViewModel(new ExerciseListViewModel(this));
        ExerciseListVM = new ExerciseListViewModel(this);
        StatisticsVM = new StatisticsViewModel();
        UserVM = new UserViewModel(this);
        WorkoutListVM = new WorkoutListViewModel();
        WorkoutVM = new WorkoutViewModel();

        OpenExerciseDetailsCommand = new RelayCommand(OpenExerciseDetails);
        OpenExerciseListCommand = new RelayCommand(OpenExerciseList);
        OpenStatisticsCommand = new RelayCommand(OpenStatistics);
        OpenUserCommand = new RelayCommand(OpenUser);
        OpenWorkoutListCommand = new RelayCommand(OpenWorkoutList);
        OpenWorkoutCommand = new RelayCommand(OpenWorkout);
    }

    private void SetViewVisibility(Action setVisible)
    {
        IsStartVisible = false;
        IsExerciseDetailsVisible = false;
        IsExerciseListVisible = false;
        IsStatisticsVisible = false;
        IsUserVisible = false;
        IsWorkoutListVisible = false;
        IsWorkoutVisible = false;

        setVisible();
    }
    public void OpenExerciseDetails(object obj)
    {
        // If added button is pressed = open details page to create new exercise
        // If existing exercise is pressed = open details page to edit exercise
        ExerciseDetailsVM.GetExercise(ExerciseListVM.SelectedExercise);
        
        SetViewVisibility(() => IsExerciseDetailsVisible = true);

        ExerciseListVM.SelectedExercise = null;
    }
    private void OpenExerciseList(object obj)
    {
        SetViewVisibility(() => IsExerciseListVisible = true);
    }
    private void OpenStatistics(object obj)
    {
        SetViewVisibility(() => IsStatisticsVisible = true);
    }
    private void OpenUser(object obj)
    {
        SetViewVisibility(() => IsUserVisible = true);
    }
    private void OpenWorkoutList(object obj)
    {
        SetViewVisibility(() => IsWorkoutListVisible = true);
    }
    private void OpenWorkout(object obj)
    {
        SetViewVisibility(() => IsWorkoutVisible = true);
    }
}
