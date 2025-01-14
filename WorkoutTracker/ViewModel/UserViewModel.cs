using System.Windows;
using System.Windows.Input;
using WorkoutTracker.Commands;
using WorkoutTracker.Model;
using WorkoutTracker.View.Dialogs;

namespace WorkoutTracker.ViewModel;
public class UserViewModel : BaseViewModel
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    public User User { get; set; }

    public ICommand ChangeUserCommand { get; set; }
    public ICommand AddNewUserCommand { get; set; }
    public ICommand LogoutCommand { get; set; }
    public UserViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        User = new User
        {
            UserName = "modigIda",
            DateJoined = DateTime.Now
        };
        
        mainWindowViewModel.StartText = $"{User.UserName} is logged in";

        ChangeUserCommand = new RelayCommand(ChangeUser);
        AddNewUserCommand = new RelayCommand(AddNewUser);
        LogoutCommand = new RelayCommand(Logout);
    }

    private void ChangeUser(object obj)
    {
        var changeUser = new ChangeUserDialog(Application.Current.MainWindow);
        changeUser.ShowDialog();
        _mainWindowViewModel.IsOptionsMenuOpen = false;
    }
    private void AddNewUser(object obj)
    {
        var addNewUser = new AddNewUserDialog(Application.Current.MainWindow);
        addNewUser.ShowDialog();
        _mainWindowViewModel.IsOptionsMenuOpen = false;
    }
    private void Logout(object obj)
    {
        User = null;
        _mainWindowViewModel.StartText = "Choose a user to start";
        _mainWindowViewModel.IsOptionsMenuOpen = false;
    }
}
