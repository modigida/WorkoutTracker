using System.Collections.ObjectModel;
using WorkoutTracker.Model;

namespace WorkoutTracker.ViewModel;
public class StatisticsViewModel : BaseViewModel
{
    private readonly UserViewModel _userViewModel;
    private User _user;
    public User User
    {
        get => _user;
        set => SetProperty(ref _user, value);
    }
    private string _currentTimeFram;
    public string CurrentTimeFrame
    {
        get => _currentTimeFram;
        set => SetProperty(ref _currentTimeFram, value);
    }
    private FavoriteExercise _selectedExercise;
    public FavoriteExercise SelectedExercise
    {
        get => _selectedExercise;
        set => SetProperty(ref _selectedExercise, value);
    }
    private ObservableCollection<FavoriteExercise> _favoriteExercises;
    public ObservableCollection<FavoriteExercise> FavoriteExercises
    {
        get => _favoriteExercises;
        set => SetProperty(ref _favoriteExercises, value);
    }
    public ObservableCollection<string> TimeFrames { get; } = new()
    {
        "Since start",
        "12 months",
        "6 months",
        "Last month",
        "Last week"
    };

    public StatisticsViewModel(UserViewModel userViewModel)
    {
        _userViewModel = userViewModel;
    }
    public void GetActiveUser()
    {
        User = _userViewModel.User;
        FavoriteExercises = new ObservableCollection<FavoriteExercise>(User.FavoriteExercises);
        SelectedExercise = FavoriteExercises.FirstOrDefault() ?? new FavoriteExercise();
        CurrentTimeFrame = "Since start";
    }
}
