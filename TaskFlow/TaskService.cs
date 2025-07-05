using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Spectre.Console;
using System.Text.Json;

public class TaskService
{
    private const string TaskFilePath = "tasks.json";
    private Dictionary<string, List<TaskItem>> userTasks = new();

    public TaskService()
    {
        LoadTasks();
    }

    public void AddTask(User currentUser)
    {
        if (currentUser == null)
        {
            Console.WriteLine("You must be logged in to add tasks.");
            return;
        }

        Console.Write("Enter task title: ");
        string title = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Enter task description: ");
        string description = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Enter deadline (dd:MM:yyyy) or leave empty: ");
        string deadlineInput = Console.ReadLine()?.Trim() ?? "";

        DateTime? deadline = null;
        if (!string.IsNullOrWhiteSpace(deadlineInput))
        {
            if (DateTime.TryParseExact(deadlineInput, "dd:MM:yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                deadline = parsed;
            }
            else
            {
                Console.WriteLine("Invalid date format. Skipping deadline.");
            }
        }

        var task = new TaskItem
        {
            Title = title,
            Description = description,
            RegisteredTime = DateTime.Now,
            Deadline = deadline
        };

        if (!userTasks.ContainsKey(currentUser.Username))
            userTasks[currentUser.Username] = new List<TaskItem>();

        userTasks[currentUser.Username].Add(task);
        SaveTasks();

        Console.WriteLine("Task added successfully.");
    }

    public void ShowTasks(User currentUser)
{
    if (currentUser == null)
    {
        AnsiConsole.MarkupLine("[red]You must be logged in to view tasks.[/]");
        return;
    }

    if (!userTasks.ContainsKey(currentUser.Username) || userTasks[currentUser.Username].Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]No tasks found.[/]");
        return;
    }

    var tasks = userTasks[currentUser.Username];

    var table = new Table();
table.Border = TableBorder.Heavy;
table.BorderColor(Color.White); 

table.AddColumn("[bold aqua]ID[/]");
table.AddColumn("[bold skyblue1]Title[/]");
table.AddColumn("[bold skyblue1]Description[/]");
table.AddColumn("[bold skyblue1]Category[/]");
table.AddColumn("[bold skyblue1]Registered[/]");
table.AddColumn("[bold skyblue1]Priority[/]");
table.AddColumn("[bold skyblue1]Deadline[/]");
table.AddColumn("[bold skyblue1]Started[/]");
table.AddColumn("[bold skyblue1]Completed[/]");
table.AddColumn("[bold skyblue1]Duration[/]");

int[] columnWidths = { 3, 20, 30, 16, 16, 10, 12, 8, 10, 10 };
        int index = 1;
foreach (var task in tasks)
{
    table.AddRow(
        $"[aqua]{index}[/]",
        $"[skyblue1]{task.Title}[/]",
        task.Description,
        $"[deepskyblue1]{task.Category}[/]",
        task.RegisteredTime.ToString("yyyy-MM-dd HH:mm"),
        $"[red]{task.Priority}[/]",
        task.Deadline?.ToString("yyyy-MM-dd") ?? "[grey]None[/]",
        task.IsStarted ? "[green]Yes[/]" : "[grey]No[/]",
        task.IsCompleted ? "[green]Yes[/]" : "[grey]No[/]",
        task.Duration?.ToString(@"hh\:mm\:ss") ?? "[grey]--[/]"
    );
    if (index < tasks.Count)
    {
        table.AddRow(columnWidths.Select(w => new string('─', w)).ToArray());
    }
    index++;
}

        AnsiConsole.Write(table);
    AnsiConsole.MarkupLine("\n[bold yellow]Select a task by its [aqua]ID[/] to perform an action.[/]");
    AnsiConsole.MarkupLine("[grey](Or press Enter to return to menu)[/]");

        string? input = Console.ReadLine();
        
}
    private void LoadTasks()
    {
        if (!File.Exists(TaskFilePath)) return;

        string json = File.ReadAllText(TaskFilePath);
        userTasks = JsonSerializer.Deserialize<Dictionary<string, List<TaskItem>>>(json) ?? new();
    }

    private void SaveTasks()
    {
        string json = JsonSerializer.Serialize(userTasks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(TaskFilePath, json);
    }
}
