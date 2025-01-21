using LiveCharts.Wpf;
using LiveCharts;
using System.Collections.ObjectModel;
using WorkoutTracker.Model;
using System.Windows.Media;
using WorkoutTracker.Repository;

namespace WorkoutTracker.ViewModel;
public class StatisticsViewModel : BaseViewModel
{
    private readonly UserViewModel _userViewModel;
    private readonly WorkoutRepository _workoutRepository;
    private readonly PersonalRecordRepository _personalRecordRepository;
    private SolidColorBrush diagramBorder = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#80414141"));
    private SeriesCollection _percentPerExerciseDiagram;
    public SeriesCollection PercentPerExerciseDiagram 
    {
        get => _percentPerExerciseDiagram;
        set => SetProperty(ref _percentPerExerciseDiagram, value);
    }
    private SeriesCollection _percentOfGoalDiagram;
    public SeriesCollection PercentOfGoalDiagram 
    {
        get => _percentOfGoalDiagram;
        set => SetProperty(ref _percentOfGoalDiagram, value);
    }
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
        set
        {
            SetProperty(ref _selectedExercise, value);
            if (UserPersonalRecords != null && SelectedExercise != null)
            {
                LoadPercentOfGoal();
            }
        }
    }
    private ObservableCollection<FavoriteExercise> _favoriteExercises;
    public ObservableCollection<FavoriteExercise> FavoriteExercises 
    {
        get => _favoriteExercises;
        set => SetProperty(ref _favoriteExercises, value); 
    }
    public List<Workout> UserWorkouts { get; set; }
    public List<WorkoutExercise> UserWorkoutExercises { get; set; }
    public List<PersonalRecord> UserPersonalRecords { get; set; }
    public ObservableCollection<string> TimeFrames { get; } = new()
    {
        "Since start",
        "12 months",
        "6 months",
        "Last month",
        "Last week"
    };
    public StatisticsViewModel(UserViewModel userViewModel, WorkoutRepository workoutRepository, PersonalRecordRepository personalRecordRepository)
    {
        _userViewModel = userViewModel;
        _workoutRepository = workoutRepository;
        _personalRecordRepository = personalRecordRepository;
    }
    public async Task GetActiveUser()
    {
        User = _userViewModel.User;
        FavoriteExercises = new ObservableCollection<FavoriteExercise>(User.FavoriteExercises);
        SelectedExercise = FavoriteExercises.FirstOrDefault() ?? new FavoriteExercise();
        CurrentTimeFrame = "Since start";
        await GetPersonalRecords();
        await GetWorkoutData();
    }
    private async Task GetWorkoutData()
    {
        UserWorkouts = new List<Workout>(await _workoutRepository.GetAllByUserIdAsync(User.Id));
        LoadPercentPerExercise();
    }
    private async Task GetPersonalRecords()
    {
        UserPersonalRecords = new List<PersonalRecord>(await _personalRecordRepository.GetBestRecordsAsync(User.Id));
        LoadPercentOfGoal();
    }
    private void LoadPercentPerExercise()
    {
        if (PercentPerExerciseDiagram == null)
        {
            PercentPerExerciseDiagram = new SeriesCollection();
        }

        var colors = new List<SolidColorBrush>
        {
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E8E41")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1976D2")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD32F2F")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFA000")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7B1FA2")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFC107")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF009688")),
        };

        int colorIndex = 0;
        Dictionary<string, int> exerciseSetCounts = new();

        foreach (var exercise in FavoriteExercises)
        {
            int totalSets = 0;

            foreach (var workout in UserWorkouts)
            {
                foreach (var workoutExercise in workout.Exercises)
                {
                    if (workoutExercise.ExerciseName == exercise.ExerciseName)
                    {
                        totalSets += workoutExercise.Sets.Count;
                    }
                }
            }

            exerciseSetCounts[exercise.ExerciseName] = totalSets;
        }

        double grandTotalSets = exerciseSetCounts.Values.Sum();

        if (grandTotalSets == 0)
        {
            return;
        }

        foreach (var kvp in exerciseSetCounts)
        {
            double percentage = (kvp.Value / grandTotalSets) * 100;

            PercentPerExerciseDiagram.Add(new PieSeries
            {
                Title = $"{kvp.Key} ({percentage:F1}%)",
                Values = new ChartValues<double> { kvp.Value },
                Fill = colors[colorIndex % colors.Count],
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                PushOut = 0
            });

            colorIndex++;
        }
    }
    private void LoadPercentOfGoal()
    {
        double goal = SelectedExercise.TargetWeight;
        double record = UserPersonalRecords.FirstOrDefault(pr => pr.ExerciseName == SelectedExercise.ExerciseName)?.MaxWeight ?? 0;

        if (record >= goal)
        {
            PercentOfGoalDiagram = new SeriesCollection
            {
                new PieSeries
                {
                    Title = $"Goal Reached ({SelectedExercise.TargetWeight} kg)",
                    Values = new ChartValues<double> { 100 },
                    Fill = Brushes.Green,
                    Stroke = diagramBorder,
                    StrokeThickness = 2,
                    PushOut = 0
                }
            };
        }
        else
        {
            double weightLeft = goal - record;

            string formattedWeight = weightLeft == Math.Floor(weightLeft)
                ? $"{weightLeft:F0}"
                : weightLeft == Math.Round(weightLeft, 1)
                    ? $"{weightLeft:F1}"
                    : $"{weightLeft:F2}";

            double achievedPercentage = (record / goal) * 100;
            double remainingPercentage = 100 - achievedPercentage;

            PercentOfGoalDiagram = new SeriesCollection
            {
                new PieSeries
                {
                    Title = $"Personal Record ({SelectedExercise.TargetWeight} kg)",
                    Values = new ChartValues<double> { achievedPercentage },
                    Fill = Brushes.Green,
                    Stroke = diagramBorder,
                    StrokeThickness = 2,
                    PushOut = 0
                },
                new PieSeries
                {
                    Title = $"{formattedWeight} kg left to reach goal",
                    Values = new ChartValues<double> { remainingPercentage },
                    Fill = Brushes.Red,
                    Stroke = diagramBorder,
                    StrokeThickness = 2,
                    PushOut = 0
                }
            };
        }
    }
}
