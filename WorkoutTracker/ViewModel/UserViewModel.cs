using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.Xml;
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
    private ChangeUserDialog changeUser;
    private AddNewUserDialog addNewUser;
    private readonly UserRepository _userRepository;



    private User _user;
    public User User 
    {
        get => _user;
        set => SetProperty(ref _user, value);
    }
    public ObservableCollection<User> Users { get; set; }
    public ObservableCollection<Exercise> Exercises { get; set; }

    private ObservableCollection<Exercise> _availableExercises;
    public ObservableCollection<Exercise> AvailableExercises
    {
        get => _availableExercises;
        set => SetProperty(ref _availableExercises, value);
    }
    private User _selectedUser;
    public User SelectedUser
    {
        get => _selectedUser;
        set
        {
            if(SetProperty(ref _selectedUser, value))
            {
                SaveChangeUser();
            }
        }
    }
    private Exercise _selectedExercise;
    public Exercise SelectedExercise
    {
        get => _selectedExercise;
        set
        {
            if (SetProperty(ref _selectedExercise, value))
            {
                AddFavoriteExercise();
            }
        }
    }

    private User _newUser;
    public User NewUser
    {
        get => _newUser;
        set => SetProperty(ref _newUser, value);
    }
    public ICommand ChangeUserCommand { get; }
    public ICommand AddNewUserCommand { get; }
    public ICommand SaveAddNewUserCommand { get; }
    public ICommand LogoutCommand { get; }
    public UserViewModel(MainWindowViewModel mainWindowViewModel, UserRepository userRepository, ExerciseRepository exerciseRepository)
    {
        _mainWindowViewModel = mainWindowViewModel;
        _userRepository = userRepository;
        _exerciseRepository = exerciseRepository;

        User = new User
        {
            UserName = "modigIda",
            DateJoined = DateTime.Now
        };
        
        mainWindowViewModel.StartText = $"{User.UserName} is logged in";

        ChangeUserCommand = new RelayCommand(ChangeUser);
        AddNewUserCommand = new RelayCommand(AddNewUser);
        SaveAddNewUserCommand = new RelayCommand(SaveAddNewUser);
        LogoutCommand = new RelayCommand(Logout);
    }
    private async Task GetExercises()
    {
        var exerciseNames = await _exerciseRepository.GetAllExerciseNamesAsync();
        var exercises = exerciseNames.Select(name => new Exercise { ExerciseName = name }).ToList();
        Exercises = new ObservableCollection<Exercise>(exercises);
        
        FilterAvailableExercises();
    }
    private void FilterAvailableExercises()
    {
        if (AvailableExercises == null)
        {
            AvailableExercises = new ObservableCollection<Exercise>();
        }

        AvailableExercises.Clear();

        foreach (var exercise in Exercises)
        {
            if (User.FavoriteExercises != null && !User.FavoriteExercises.Any(fav => fav.ExerciseName == exercise.ExerciseName))
            {
                AvailableExercises.Add(exercise);
            }
            if (NewUser.FavoriteExercises != null && !NewUser.FavoriteExercises.Any(fav => fav.ExerciseName == exercise.ExerciseName))
            {
                AvailableExercises.Add(exercise);
            }
        }
    }
    private void AddFavoriteExercise()
    {
        if (User.FavoriteExercises == null)
        {
            User.FavoriteExercises = new List<FavoriteExercise>();
        }
        User.FavoriteExercises.Add(new FavoriteExercise
        {
            ExerciseName = SelectedExercise.ExerciseName,
            TargetWeight = 0 
        });
        FilterAvailableExercises();
    }
    private async void GetUsers()
    {
        // Get Users from database
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
        await _userRepository.CreateAsync(User);
        _mainWindowViewModel.StartText = $"{User.UserName} is logged in";
        addNewUser.Close();
    }
    private void Logout(object obj)
    {
        User = null;
        _mainWindowViewModel.StartText = "Choose a user to start";
        _mainWindowViewModel.IsOptionsMenuOpen = false;
    }
}
