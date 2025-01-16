using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WorkoutTracker.Commands;
using WorkoutTracker.Model;
using WorkoutTracker.Repository;
using WorkoutTracker.View.Dialogs;

namespace WorkoutTracker.ViewModel;
public class UserViewModel : BaseViewModel
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly ExerciseRepository _exerciseRepository;
    private readonly UserRepository _userRepository;

    private ChangeUserDialog changeUser;
    private AddNewUserDialog addNewUser;

    private User _user;
    public User User
    {
        get
        {
            if (_user == null)
            {
                _user = new User { UserName = "Offline" };
            }
            return _user;
        }
        set => SetProperty(ref _user, value);
    }
    private ObservableCollection<User> _users;
    public ObservableCollection<User> Users
    {
        get => _users;
        set => SetProperty(ref _users, value);
    }
    public ObservableCollection<FavoriteExercise> Exercises { get; set; }

    private ObservableCollection<FavoriteExercise> _availableExercises;
    public ObservableCollection<FavoriteExercise> AvailableExercises
    {
        get => _availableExercises;
        set => SetProperty(ref _availableExercises, value);
    }

    // ChangeUserDialog
    private User _selectedUser;
    public User SelectedUser
    {
        get => _selectedUser;
        set
        {
            if (SetProperty(ref _selectedUser, value))
            {
                if (SelectedUser != null)
                {
                    SaveChangeUser();
                }
            }
        }
    }

    // AddNewUserDialog
    private string _addedFavoriteExercise;
    public string AddedFavoriteExercise
    {
        get => _addedFavoriteExercise;
        set => SetProperty(ref _addedFavoriteExercise, value);
    }
    private FavoriteExercise _selectedFavoriteExercise;
    public FavoriteExercise SelectedFavoriteExercise
    {
        get => _selectedFavoriteExercise;
        set
        {
            if (SetProperty(ref _selectedFavoriteExercise, value))
            {
                if (SelectedFavoriteExercise != null)
                {
                    AddedFavoriteExercise = SelectedFavoriteExercise.ExerciseName;
                }
            }
        }
    }
    private double _targetWeight;
    public double TargetWeight
    {
        get => _targetWeight;
        set => SetProperty(ref _targetWeight, value);
    }

    private User _newUser;
    public User NewUser
    {
        get => _newUser;
        set => SetProperty(ref _newUser, value);
    }
    private ObservableCollection<FavoriteExercise> _favoriteExercises;
    public ObservableCollection<FavoriteExercise> FavoriteExercises
    {
        get => _favoriteExercises;
        set => SetProperty(ref _favoriteExercises, value);
    }
    public ICommand ChangeUserCommand { get; }
    public ICommand AddNewUserCommand { get; }
    public ICommand AddFavoriteExerciseCommand { get; }
    public ICommand DeleteFavoriteExerciseCommand { get; }
    public ICommand SaveAddNewUserCommand { get; }
    public ICommand LogoutCommand { get; }
    public UserViewModel(MainWindowViewModel mainWindowViewModel, UserRepository userRepository, ExerciseRepository exerciseRepository)
    {
        _mainWindowViewModel = mainWindowViewModel;
        _userRepository = userRepository;
        _exerciseRepository = exerciseRepository;

        ChangeUserCommand = new RelayCommand(ChangeUser);
        AddNewUserCommand = new RelayCommand(AddNewUser);
        AddFavoriteExerciseCommand = new RelayCommand(AddNewUserFavoriteExercise);
        DeleteFavoriteExerciseCommand = new RelayCommand<FavoriteExercise>(async favoriteExercise => await DeleteFavoriteExercise(favoriteExercise));
        SaveAddNewUserCommand = new RelayCommand(SaveAddNewUser);
        LogoutCommand = new RelayCommand(Logout);
    }
    private async Task GetExercises()
    {
        var exerciseNames = await _exerciseRepository.GetAllExerciseNamesAsync();
        var exercises = exerciseNames.Select(name => new FavoriteExercise { ExerciseName = name }).ToList();
        Exercises = new ObservableCollection<FavoriteExercise>(exercises);

        FilterAvailableExercises();
    }
    private void FilterAvailableExercises()
    {
        if (AvailableExercises == null)
        {
            AvailableExercises = new ObservableCollection<FavoriteExercise>();
        }
        if (FavoriteExercises == null)
        {
            FavoriteExercises = new ObservableCollection<FavoriteExercise>();
        }

        AvailableExercises.Clear();

        foreach (var exercise in Exercises)
        {
            if (NewUser.FavoriteExercises == null && User.FavoriteExercises != null && !User.FavoriteExercises.Any(fav => fav.ExerciseName == exercise.ExerciseName))
            {
                AvailableExercises.Add(exercise);
            }
            if (FavoriteExercises != null && !FavoriteExercises.Any(fav => fav.ExerciseName == exercise.ExerciseName))
            {
                AvailableExercises.Add(exercise);
            }
        }
    }
    private void AddNewUserFavoriteExercise(object obj)
    {
        if (FavoriteExercises == null)
        {
            FavoriteExercises = new ObservableCollection<FavoriteExercise>();
        }

        if (AddedFavoriteExercise != null)
        {
            AddedFavoriteExercise = SelectedFavoriteExercise.ExerciseName;

            FavoriteExercises.Add(new FavoriteExercise
            {
                ExerciseName = AddedFavoriteExercise,
                TargetWeight = TargetWeight
            });
        }

        TargetWeight = 0;

        FilterAvailableExercises();
    }
    private async Task DeleteFavoriteExercise(FavoriteExercise favoriteExercise)
    {
        if (favoriteExercise != null)
        {
            FavoriteExercises.Remove(favoriteExercise);
        }
        FilterAvailableExercises();
    }
    private async void GetUsers()
    {
        var users = await _userRepository.GetAllAsync();
        Users = new ObservableCollection<User>(users);
    }
    private void ChangeUser(object obj)
    {
        GetUsers();
        changeUser = new ChangeUserDialog(Application.Current.MainWindow, _mainWindowViewModel);
        changeUser.ShowDialog();
        _mainWindowViewModel.IsOptionsMenuOpen = false;
    }
    private void SaveChangeUser()
    {
        User = SelectedUser;
        _mainWindowViewModel.StartText = $"{User.UserName} is logged in";
        SelectedUser = null;
        changeUser.Close();
    }
    private async void AddNewUser(object obj)
    {
        NewUser = new User
        {
            UserName = "New User",
            FavoriteExercises = new List<FavoriteExercise>()
        };
        await GetExercises();
        addNewUser = new AddNewUserDialog(Application.Current.MainWindow, _mainWindowViewModel);
        addNewUser.ShowDialog();
        _mainWindowViewModel.IsOptionsMenuOpen = false;
    }
    private async void SaveAddNewUser(object obj)
    {
        User = NewUser;
        User.DateJoined = DateTime.Now;
        User.FavoriteExercises = new List<FavoriteExercise>(FavoriteExercises);
        await _userRepository.CreateAsync(User);
        _mainWindowViewModel.StartText = $"{User.UserName} is logged in";
        NewUser = null;
        FavoriteExercises.Clear();
        addNewUser.Close();
    }
    private void Logout(object obj)
    {
        User = new User { UserName = "Offline" };
        _mainWindowViewModel.StartText = "Choose a user to start";
        _mainWindowViewModel.OpenStartView();
        _mainWindowViewModel.IsOptionsMenuOpen = false;
    }
}
