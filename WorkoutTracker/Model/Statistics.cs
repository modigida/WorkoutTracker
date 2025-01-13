﻿namespace WorkoutTracker.Model;
public class Statistics
{
    public string UserId { get; set; }
    public double TotalWorkouts { get; set; }
    public double TotalWeightLifted { get; set; }
    public DateTime AverageWorkoutDuration { get; set; }
    public Exercise MostCommonExercise { get; set; }
}
