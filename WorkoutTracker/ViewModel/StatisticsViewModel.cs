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
    private string _totalWeightLifted;
    public string TotalWeightLifted
    {
        get => _totalWeightLifted;
        set => SetProperty(ref _totalWeightLifted, value);
    }
    private string _averageWeightPerRep;
    public string AverageWeightPerRep
    {
        get => _averageWeightPerRep;
        set => SetProperty(ref _averageWeightPerRep, value);
    }
    private string _percentOfGoalString;
    public string PercentOfGoalString
    {
        get => _percentOfGoalString;
        set => SetProperty(ref _percentOfGoalString, value);
    }
    private string _selectedTimeFram;
    public string SelectedTimeFrame
    {
        get => _selectedTimeFram;
        set
        {
            SetProperty(ref _selectedTimeFram, value);
            if (SelectedExercise != null && UserWorkouts != null)
            {
                FilterWorkouts();
            }
        }
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
                GetTotalWeightLifted();
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
    public List<Workout> FilteredWorkouts { get; set; }
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
        SelectedTimeFrame = "Since start";
        await GetPersonalRecords();
        await GetWorkoutData();
        GetTotalWeightLifted();
    }
    private async Task GetWorkoutData()
    {
        UserWorkouts = await _workoutRepository.GetAllByUserIdAsync(User.Id);
        FilterWorkouts();
    }
    private void FilterWorkouts()
    {
        DateTime now = DateTime.Now;
        FilteredWorkouts = SelectedTimeFrame switch
        {
            "Since start" => new List<Workout>(UserWorkouts),
            "12 months" => new List<Workout>(UserWorkouts.Where(w => w.Date >= now.AddMonths(-12))),
            "6 months" => new List<Workout>(UserWorkouts.Where(w => w.Date >= now.AddMonths(-6))),
            "Last month" => new List<Workout>(UserWorkouts.Where(w => w.Date >= now.AddMonths(-1))),
            "Last week" => new List<Workout>(UserWorkouts.Where(w => w.Date >= now.AddDays(-7))),
            _ => new List<Workout>(UserWorkouts)
        };

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
        else
        {
            PercentPerExerciseDiagram.Clear();
        }

        var colors = new List<SolidColorBrush>
        {
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EC7063")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#76448A")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8C471")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC7633")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495E")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5499C7")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECF0F1"))
        };

        int maxCategories = 6;
        Dictionary<string, int> exerciseSetCounts = new();

        foreach (var exercise in FavoriteExercises)
        {
            int totalSets = 0;

            foreach (var workout in FilteredWorkouts)
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

        var sortedExercises = exerciseSetCounts.OrderByDescending(kvp => kvp.Value).ToList();

        double grandTotalSets = sortedExercises.Sum(kvp => kvp.Value);
        if (grandTotalSets == 0) return;

        int colorIndex = 0;
        double otherSets = 0;

        for (int i = 0; i < sortedExercises.Count; i++)
        {
            if (i < maxCategories)
            {
                var kvp = sortedExercises[i];
                double percentage = (kvp.Value / grandTotalSets) * 100;

                PercentPerExerciseDiagram.Add(new PieSeries
                {
                    Title = $"{kvp.Key}, {kvp.Value} sets = {percentage:F1}%",
                    Values = new ChartValues<double> { kvp.Value },
                    Fill = colors[colorIndex % colors.Count],
                    Stroke = diagramBorder,
                    StrokeThickness = 1,
                    PushOut = 0
                });

                colorIndex++;
            }
            else
            {
                otherSets += sortedExercises[i].Value;
            }
        }

        if (otherSets > 0)
        {
            double otherPercentage = (otherSets / grandTotalSets) * 100;

            PercentPerExerciseDiagram.Add(new PieSeries
            {
                Title = $"Övrigt, {otherSets} sets = {otherPercentage:F1}%",
                Values = new ChartValues<double> { otherSets },
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDC3C7")),
                Stroke = diagramBorder,
                StrokeThickness = 1,
                PushOut = 0
            });
        }
    }
    private void GetTotalWeightLifted()
    {
        TotalWeightLifted = string.Empty;
        var totalWeight = 0.0;
        var amountOfReps = 0.0;

        foreach (var workout in UserWorkouts)
        {
            foreach (var exercise in workout.Exercises)
            {
                if (exercise.ExerciseName == SelectedExercise.ExerciseName)
                {
                    foreach (var set in exercise.Sets)
                    {
                        totalWeight += set.Weight * set.Reps;
                        amountOfReps += set.Reps;
                    }
                }
            }
        }

        var average = totalWeight / amountOfReps;
        string formattedAverage = FormatDouble(average);

        AverageWeightPerRep = amountOfReps > 0
            ? $"in average you {SelectedExercise.ExerciseName.ToLower()} {formattedAverage} kg per rep"
            : "no reps available to calculate average";

        string formattedWeight = FormatDouble(totalWeight);

        TotalWeightLifted = amountOfReps > 0
            ? $"your total weight lifted for {SelectedExercise.ExerciseName.ToLower()} is {formattedWeight} kg"
            : $"no data available for {SelectedExercise.ExerciseName.ToLower()}";
    }
    private void LoadPercentOfGoal()
    {
        PercentOfGoalString = string.Empty;
        double goal = SelectedExercise.TargetWeight;
        double record = UserPersonalRecords.FirstOrDefault(pr => pr.ExerciseName == SelectedExercise.ExerciseName)?.MaxWeight ?? 0;

        if (record >= goal)
        {
            PercentOfGoalDiagram = new SeriesCollection
            {
                new PieSeries
                {
                    Title = $"Goal ({SelectedExercise.TargetWeight} kg) Reached",
                    Values = new ChartValues<double> { 100 },
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC7633")),
                    Stroke = diagramBorder,
                    StrokeThickness = 2,
                    PushOut = 0
                }
            };
            PercentOfGoalString = $"you have reached your goal for {SelectedExercise.ExerciseName.ToLower()}";
        }
        else
        {
            double weightLeft = goal - record;

            string formattedWeight = FormatDouble(weightLeft);
            string formattedeRecord = FormatDouble(record);

            double achievedPercentage = (record / goal) * 100;
            double remainingPercentage = 100 - achievedPercentage;

            PercentOfGoalDiagram = new SeriesCollection
            {
                new PieSeries
                {
                    Title = $"Personal Record ({formattedeRecord} kg)",
                    Values = new ChartValues<double> { achievedPercentage },
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC7633")),
                    Stroke = diagramBorder,
                    StrokeThickness = 2,
                    PushOut = 0
                },
                new PieSeries
                {
                    Title = $"{formattedWeight} kg left to reach goal ({SelectedExercise.TargetWeight} kg)",
                    Values = new ChartValues<double> { remainingPercentage },
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8C471")),
                    Stroke = diagramBorder,
                    StrokeThickness = 2,
                    PushOut = 0
                }
            };

            var formattedPercentage = FormatDouble(achievedPercentage);
            PercentOfGoalString = $"you have reached {formattedPercentage} % of your {SelectedExercise.ExerciseName.ToLower()} goal";
        }
    }
    public string FormatDouble(double number)
    {
        return number == Math.Floor(number)
            ? $"{number:F0}"
            : number == Math.Round(number, 1)
                ? $"{number:F1}"
                : $"{number:F2}";
    }
}
