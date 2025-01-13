using WorkoutTracker.Model;

namespace WorkoutTracker.ViewModel;
public class UserViewModel : BaseViewModel
{
    public User User { get; set; }
    public UserViewModel(MainWindowViewModel mainWindowViewModel)
    {
        User = new User
        {
            UserName = "modigIda",
            DateJoined = DateTime.Now
        };
        
        mainWindowViewModel.StartText = $"{User.UserName} is logged in";
    }
}
