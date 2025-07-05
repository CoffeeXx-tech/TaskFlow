using System;
public class TaskItem
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";

    public DateTime RegisteredTime { get; set; } = DateTime.Now;
    public DateTime? StartTime { get; set; } = null;
    public DateTime? CompletedTime { get; set; } = null;
    public DateTime? Deadline { get; set; } = null;

    public bool IsStarted { get; set; } = false;
    public bool IsCompleted { get; set; } = false;

    public string Category { get; set; } = "none";
    public string Priority { get; set; } = "Normal"; 
    public TimeSpan? Duration => (IsCompleted && StartTime != null && CompletedTime != null)
        ? CompletedTime - StartTime
        : null;
}
