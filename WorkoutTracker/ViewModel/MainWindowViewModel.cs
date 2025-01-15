using System.Windows.Input;
using WorkoutTracker.Commands;
using WorkoutTracker.Database;
using WorkoutTracker.Model;
using WorkoutTracker.Repository;

namespace WorkoutTracker.ViewModel;
public class MainWindowViewModel : BaseViewModel
{
    public ExerciseDetailsViewModel ExerciseDetailsVM { get; set; }
    public ExerciseListViewModel ExerciseListVM { get; set; }
    public StatisticsViewModel StatisticsVM { get; set; }
    public UserViewModel UserVM { get; set; }
    public WorkoutListViewModel WorkoutListVM { get; set; }
    public WorkoutViewModel WorkoutVM { get; set; }
    public WorkoutDetailsViewModel WorkoutDetailsVM { get; set; }

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
    
    private bool _isWorkoutDetailsVisible;
    public bool IsWorkoutDetailsVisible
    {
        get => _isWorkoutDetailsVisible;
        set => SetProperty(ref _isWorkoutDetailsVisible, value);
    }
    private bool _isOptionsMenuOpen;
    public bool IsOptionsMenuOpen
    {
        get => _isOptionsMenuOpen;
        set => SetProperty(ref _isOptionsMenuOpen, value);
    }
    private string _startText;
    public string StartText 
    {
        get => _startText;
        set => SetProperty(ref _startText, value);
    }

    public ICommand OpenExerciseDetailsCommand { get; }
    public ICommand OpenExerciseListCommand { get; }
    public ICommand OpenStatisticsCommand { get; }
    public ICommand OpenUserCommand { get; }
    public ICommand OpenWorkoutListCommand { get; }
    public ICommand OpenWorkoutCommand { get; }
    public ICommand OpenOptionsMenuCommand { get; }
    public MainWindowViewModel(MongoDbContext dbContext)
    {
        IsStartVisible = true;
        StartText = "Choose a user to start";

        ExerciseListVM = new ExerciseListViewModel(this, new ExerciseRepository(dbContext));
        ExerciseDetailsVM = new ExerciseDetailsViewModel(ExerciseListVM, new MuscleGroupRepository(dbContext), new ExerciseRepository(dbContext));
        StatisticsVM = new StatisticsViewModel();
        UserVM = new UserViewModel(this, new UserRepository(dbContext), new ExerciseRepository(dbContext));
        WorkoutListVM = new WorkoutListViewModel(new WorkoutRepository(dbContext));
        WorkoutVM = new WorkoutViewModel(this);
        WorkoutDetailsVM = new WorkoutDetailsViewModel(new WorkoutRepository(dbContext));

        OpenExerciseDetailsCommand = new RelayCommand(OpenExerciseDetails);
        OpenExerciseListCommand = new RelayCommand(OpenExerciseList);
        OpenStatisticsCommand = new RelayCommand(OpenStatistics);
        OpenUserCommand = new RelayCommand(OpenUser);
        OpenWorkoutListCommand = new RelayCommand(OpenWorkoutList);
        OpenWorkoutCommand = new RelayCommand(OpenWorkout);
        OpenOptionsMenuCommand = new RelayCommand(OpenOptionsMenu);
    }

    private void OpenOptionsMenu(object obj)
    {
        IsOptionsMenuOpen = !IsOptionsMenuOpen;
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
        IsWorkoutDetailsVisible = false;

        setVisible();
    }
    public void OpenStartView()
    {
        SetViewVisibility(() => IsStartVisible = true);
    }
    public async void OpenExerciseDetails(object obj)
    {
        await ExerciseDetailsVM.GetMuscleGroups();
        ExerciseDetailsVM.GetExercise(ExerciseListVM.SelectedExercise);
        SetViewVisibility(() => IsExerciseDetailsVisible = true);
        ExerciseListVM.SelectedExercise = null;
    }
    private async void OpenExerciseList(object obj)
    {
        await ExerciseListVM.GetExercises();
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
    private async void OpenWorkoutList(object obj)
    {
        await WorkoutListVM.GetAllWorkouts(UserVM.User.Id);
        SetViewVisibility(() => IsWorkoutListVisible = true);
    }
    private void OpenWorkout(object obj)
    {
        WorkoutVM.StartNewWorkout(UserVM.User.Id);
        SetViewVisibility(() => IsWorkoutVisible = true);
    }
    public async void OpenWorkoutDetails(Workout workout)
    {
        await WorkoutDetailsVM.GetWorkout(workout);
        SetViewVisibility(() => IsWorkoutDetailsVisible = true);
    }
}
