using System.Collections.ObjectModel;
using System.Linq;
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
    public UserViewModel(MainWindowViewModel mainWindowViewModel, UserRepository userRepository)
    {
        _mainWindowViewModel = mainWindowViewModel;
        _userRepository = userRepository;

        GetExercises();

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

    private void GetExercises()
    {
        // Get Exercises from database

        Exercises = new ObservableCollection<Exercise>();
        FilterAvailableExercises();
    }

    private void FilterAvailableExercises()
    {
        foreach (var exercise in Exercises)
        {
            if (User.FavoriteExercises != null && User.FavoriteExercises.Any(fav => fav.ExerciseName == exercise.ExerciseName))
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
    private void GetUsers()
    {
        // Get Users from database

        Users = new ObservableCollection<User>() { new User() { UserName = "ida", DateJoined = DateTime.Now }, new User() { UserName = "johanna", DateJoined = DateTime.Now } };
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
    private void AddNewUser(object obj)
    {
        addNewUser = new AddNewUserDialog(Application.Current.MainWindow, _mainWindowViewModel);
        addNewUser.ShowDialog();
        _mainWindowViewModel.IsOptionsMenuOpen = false;
    }
    private async void SaveAddNewUser(object obj)
    {
        // Save new user to database
        User = NewUser;
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
