using System.Windows;
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
    private bool _isOnline;
    public bool IsOnline
    {
        get => _isOnline;
        set
        {
            SetProperty(ref _isOnline, value);
            if (!IsOnline)
            {
                IsAvailable = false;
            }
            else
            {
                IsAvailable = true;
            }
        }
    }
    private bool _isMenuEnabled = true;
    public bool IsMenuEnabled
    {
        get => _isMenuEnabled;
        set => SetProperty(ref _isMenuEnabled, value);
    }
    private bool _isAvailable;
    public bool IsAvailable
    {
        get => _isAvailable;
        set => SetProperty(ref _isAvailable, value);
    }
    private string _currentUserStatus;
    public string CurrentUserStatus
    {
        get => _currentUserStatus; 
        set => SetProperty(ref _currentUserStatus, value);
    }

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
    public ICommand AddNewUserCommand { get; }
    public ICommand OpenUserCommand { get; }
    public ICommand OpenWorkoutListCommand { get; }
    public ICommand OpenWorkoutCommand { get; }
    public ICommand OpenOptionsMenuCommand { get; }
    public MainWindowViewModel(MongoDbContext dbContext)
    {
        IsStartVisible = true;

        ExerciseListVM = new ExerciseListViewModel(this, new ExerciseRepository(dbContext));
        ExerciseDetailsVM = new ExerciseDetailsViewModel(ExerciseListVM, new MuscleGroupRepository(dbContext), new ExerciseRepository(dbContext), this);
        UserVM = new UserViewModel(this, new UserRepository(dbContext), new ExerciseRepository(dbContext), new PersonalRecordRepository(dbContext));
        WorkoutListVM = new WorkoutListViewModel(new WorkoutRepository(dbContext), this);
        WorkoutVM = new WorkoutViewModel(this, new WorkoutRepository(dbContext), ExerciseListVM, UserVM, new PersonalRecordRepository(dbContext), WorkoutListVM);
        WorkoutDetailsVM = new WorkoutDetailsViewModel(new WorkoutRepository(dbContext), WorkoutListVM);
        StatisticsVM = new StatisticsViewModel(UserVM, new WorkoutRepository(dbContext), new PersonalRecordRepository(dbContext), new ExerciseRepository(dbContext));

        OpenExerciseDetailsCommand = new RelayCommand(OpenExerciseDetails);
        OpenExerciseListCommand = new RelayCommand(OpenExerciseList);
        OpenStatisticsCommand = new RelayCommand(OpenStatistics);
        AddNewUserCommand = new RelayCommand(AddNewUser);
        OpenUserCommand = new RelayCommand(OpenUser);
        OpenWorkoutListCommand = new RelayCommand(OpenWorkoutList);
        OpenWorkoutCommand = new RelayCommand(OpenWorkout);
        OpenOptionsMenuCommand = new RelayCommand(OpenOptionsMenu);

        SetCurrentUserStatus();
    }
    private void OpenOptionsMenu(object obj)
    {
        IsOptionsMenuOpen = !IsOptionsMenuOpen;
    }
    public void SetCurrentUserStatus()
    {
        if (UserVM.User != null)
        {
            CurrentUserStatus = $"{UserVM.User.UserName}";
            StartText = $"{UserVM.User.UserName} is logged in";
            IsOnline = true;
        }
        else
        {
            CurrentUserStatus = "Offline";
            StartText = "Choose a user to start";
            IsOnline = false;
        }
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
    public async void OpenExerciseList(object obj)
    {
        await ExerciseListVM.GetExerciseNames();
        SetViewVisibility(() => IsExerciseListVisible = true);
    }
    private async void OpenStatistics(object obj)
    {
        if (UserVM.User.Id == null) { return; }
        await StatisticsVM.GetActiveUser();
        SetViewVisibility(() => IsStatisticsVisible = true);
    }
    private async void AddNewUser(object obj)
    {
        if (UserVM.FavoriteExercises != null)
        {
            UserVM.FavoriteExercises.Clear();
        }
        UserVM.User = new User
        {
            UserName = "New User",
            DateJoined = DateTime.Now,
            FavoriteExercises = new List<FavoriteExercise>()
        };
        await UserVM.GetExercises();
        SetViewVisibility(() => IsUserVisible = true);
        IsOptionsMenuOpen = false;
    }
    private async void OpenUser(object obj)
    {
        if (UserVM.User.Id == null) { return; }
        await UserVM.GetUser(UserVM.User.Id);
        SetViewVisibility(() => IsUserVisible = true);
        IsOptionsMenuOpen = false;
    }
    private async void OpenWorkoutList(object obj)
    {
        if (UserVM.User.Id == null) { return; }
        await WorkoutListVM.GetAllWorkouts(UserVM.User.Id);
        SetViewVisibility(() => IsWorkoutListVisible = true);
    }
    private async void OpenWorkout(object obj)
    {
        if (UserVM.User.Id == null) { return; }
        await WorkoutVM.StartNewWorkout(UserVM.User.Id);
        SetViewVisibility(() => IsWorkoutVisible = true);
    }
    public async void OpenWorkoutDetails(Workout workout)
    {
        await WorkoutDetailsVM.GetWorkout(workout);
        WorkoutListVM.SelectedWorkout = null;
        SetViewVisibility(() => IsWorkoutDetailsVisible = true);
    }
}
