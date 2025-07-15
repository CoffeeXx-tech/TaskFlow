using System;
public class TaskItem
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime RegisteredTime { get; set; } = DateTime.Now;
    public DateTime? StartTime { get; set; } = null;
    public DateTime? CompletedTime { get; set; } = null;
    public DateTime? Deadline { get; set; } = null;
    public string Priority { get; set; } = "Medium"; 
    public bool IsStarted { get; set; } = false;
    public bool IsCompleted { get; set; } = false;
    public string Category { get; set; } = "none";
    public TimeSpan? Duration => (IsCompleted && StartTime != null && CompletedTime != null)
        ? CompletedTime - StartTime
        : null;
}
