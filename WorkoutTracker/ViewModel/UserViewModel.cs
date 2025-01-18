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
    private readonly PersonalRecordRepository _personalRecordRepository;

    private ChangeUserDialog changeUser;
    private AddNewUserDialog addNewUser;

    private User _user;
    public User User
    {
        get => _user;
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
    private ObservableCollection<PersonalRecord> _personalRecords;
    public ObservableCollection<PersonalRecord> PersonalRecords
    {
        get => _personalRecords;
        set => SetProperty(ref _personalRecords, value);
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
    public ICommand SaveUserCommand { get; }
    public ICommand DeleteUserCommand { get; }
    public UserViewModel(MainWindowViewModel mainWindowViewModel, UserRepository userRepository, 
        ExerciseRepository exerciseRepository, PersonalRecordRepository personalRecordRepository)
    {
        _mainWindowViewModel = mainWindowViewModel;
        _userRepository = userRepository;
        _exerciseRepository = exerciseRepository;
        _personalRecordRepository = personalRecordRepository;

        ChangeUserCommand = new RelayCommand(ChangeUser);
        AddNewUserCommand = new RelayCommand(AddNewUser);
        AddFavoriteExerciseCommand = new RelayCommand(AddNewUserFavoriteExercise);
        DeleteFavoriteExerciseCommand = new RelayCommand<FavoriteExercise>(async favoriteExercise => await DeleteFavoriteExercise(favoriteExercise));
        SaveAddNewUserCommand = new RelayCommand(SaveAddNewUser);
        LogoutCommand = new RelayCommand(Logout);
        SaveUserCommand = new RelayCommand(SaveUser);
        DeleteUserCommand = new RelayCommand(DeleteUser);
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

        if (FavoriteExercises != null)
        {
            var favoriteExerciseNames = new HashSet<string>(FavoriteExercises.Select(fav => fav.ExerciseName));

            var exercisesToAdd = Exercises.Where(ex => !favoriteExerciseNames.Contains(ex.ExerciseName));

            AvailableExercises.Clear();
            foreach (var exercise in exercisesToAdd)
            {
                AvailableExercises.Add(exercise);
            }
        }
        else
        {
            AvailableExercises.Clear();
            foreach (var exercise in Exercises)
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
    public async Task GetUser(string userId)
    {
        User = await _userRepository.GetByIdAsync(userId);
        FavoriteExercises = new ObservableCollection<FavoriteExercise>(User.FavoriteExercises);
        await GetExercises();
        CalculatePersonalRecords();
    }
    private async void CalculatePersonalRecords()
    {
        var personalRecords = await _personalRecordRepository.GetBestRecordsAsync(User.Id);
        PersonalRecords = new ObservableCollection<PersonalRecord>(personalRecords);
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
        _mainWindowViewModel.SetCurrentUserStatus();
        SelectedUser = null;
        _mainWindowViewModel.OpenStartView();
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
        _mainWindowViewModel.SetCurrentUserStatus();
        addNewUser.Close();
        NewUser = null;
        FavoriteExercises.Clear();
    }
    private void Logout(object obj)
    {
        User = null;
        _mainWindowViewModel.SetCurrentUserStatus();
        _mainWindowViewModel.OpenStartView();
        _mainWindowViewModel.IsOptionsMenuOpen = false;
    }
    private async void SaveUser(object obj)
    {
        User.FavoriteExercises = FavoriteExercises.ToList();
        await _userRepository.UpdateAsync(User.Id, User);
    }
    private async void DeleteUser(object obj)
    {
        var finish = MessageBox.Show($"Delete user {User.UserName}?",
                                            "Confirm",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question);

        if (finish != MessageBoxResult.Yes) return;

        await _userRepository.DeleteAsync(User.Id);
        Logout(null);
    }
}
