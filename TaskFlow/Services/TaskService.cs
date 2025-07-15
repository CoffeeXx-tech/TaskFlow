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

    public void AddTask(User currentUser, CategoryService categoryService)

    {
        if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.Username))
        {
            AnsiConsole.MarkupLine("[orangered1]You must be logged in to add tasks.[/]");
            return;
        }
        AnsiConsole.Markup("Enter task [skyblue1]title: [/]");
        string title = Console.ReadLine()?.Trim() ?? "";

        AnsiConsole.Markup("Enter task [skyblue1]description: [/]");
        string description = Console.ReadLine()?.Trim() ?? "";

        var categories = categoryService.GetCategories(currentUser);
        string category = "None";

        if (categories.Count > 0)
        {
            var selectedCategory = AnsiConsole.Prompt(
                new SelectionPrompt<Category>()
                    .Title("Choose a [skyblue1]category[/] for this task:")
                    .HighlightStyle("skyblue1 on black")
                    .UseConverter(c => $"[{c.Color}]{c.Name}[/]")
                    .AddChoices(categories)
            );

            category = selectedCategory.Name;
        }
        else
        {
            AnsiConsole.MarkupLine("[orangered1]No categories found. Using default: None[/]");
        }


        AnsiConsole.MarkupLine("Enter [skyblue1]deadline[/] (dd:MM:yyyy) or leave empty: ");
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
                AnsiConsole.MarkupLine($"[orangered1]Invalid date format. Skipping deadline.[/]");
            }
        }
        var priorities = new List<string> { "High", "Medium", "Low" };
        var selectedPriority = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose task priority:")
                .HighlightStyle("white on black")
                .UseConverter(p => p switch           
                {
                    "High"   => "[red]High[/]",
                    "Medium" => "[orange1]Medium[/]",
                    "Low"    => "[yellow1]Low[/]",
                    _        => p
                })
                .AddChoices(priorities)
        );

        var task = new TaskItem
        {
            Title = title,
            Description = description,
            RegisteredTime = DateTime.Now,
            Deadline = deadline,
            Category = category,
            Priority = selectedPriority
        };

        if (!userTasks.ContainsKey(currentUser.Username))
            userTasks[currentUser.Username] = new List<TaskItem>();

        userTasks[currentUser.Username].Add(task);
        SaveTasks();

        AnsiConsole.MarkupLine($"Task added [skyblue1]successfully.[/]");
    }

    public void ShowTasks(User currentUser, CategoryService categoryService)
    {
        if (currentUser == null)
        {
            AnsiConsole.MarkupLine("[orangered1]You must be logged in to view tasks.[/]");
            return;
        }

        if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.Username))
        {
            AnsiConsole.MarkupLine("[orangered1]You must be logged in to view tasks.[/]");
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

        int[] columnWidths = { 3, 15, 24, 12, 16, 8, 10, 7, 9, 8 };
        int index = 1;
        foreach (var task in tasks)
        {
            table.AddRow(
                $"[aqua]{index}[/]",
                $"[skyblue1]{task.Title}[/]",
                task.Description,
                $"[deepskyblue1]{task.Category}[/]",
                task.RegisteredTime.ToString("yyyy-MM-dd HH:mm"),
                GetPriorityColor(task.Priority),
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
        AnsiConsole.MarkupLine("\nSelect a task by its [skyblue1]ID[/] to perform an action.");
        AnsiConsole.MarkupLine("[grey](Or press Enter to return to menu)[/]");

        while (true)
        {
            string? input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input)) return;

            if (int.TryParse(input, out int taskIndex) && taskIndex > 0 && taskIndex <= tasks.Count)
            {
                var selectedTask = tasks[taskIndex - 1];

                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"[bold skyblue1]Choose action for task [aqua]{selectedTask.Title}[/]:[/]")
                        .HighlightStyle("skyblue1 on black")
                        .AddChoices([
                            "Start",
                        "Stop",
                        "Mark as Completed",
                        "Unmark Completed",
                        "Edit",
                        "Change Category",
                        "Delete",
                        "Cancel"
                        ])
                );

                switch (action)
                {
                    case "Start":
                        if (!selectedTask.IsStarted)
                        {
                            selectedTask.IsStarted = true;
                            selectedTask.StartTime = DateTime.Now;
                            AnsiConsole.MarkupLine("[skyblue1]Task started.[/]");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[orangered1]Task already started.[/]");
                        }
                        break;

                    case "Stop":
                        if (selectedTask.IsStarted && !selectedTask.IsCompleted)
                        {
                            selectedTask.IsStarted = false;
                            if (selectedTask.StartTime == null)
                                selectedTask.StartTime = DateTime.Now;
                            selectedTask.CompletedTime = DateTime.Now;
                            selectedTask.IsCompleted = true;


                            AnsiConsole.MarkupLine("[skyblue1]Task stopped. Duration updated.[/]");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[orangered1]Task not started or already completed.[/]");
                        }
                        break;

                    case "Mark as Completed":
                        selectedTask.IsCompleted = true;
                        selectedTask.CompletedTime = DateTime.Now;
                        AnsiConsole.MarkupLine("[skyblue1]Task marked as completed.[/]");
                        break;

                    case "Unmark Completed":
                        selectedTask.IsCompleted = false;
                        selectedTask.CompletedTime = null;
                        AnsiConsole.MarkupLine("[orangered1]Task unmarked as completed.[/]");
                        break;

                    case "Edit":
                        AnsiConsole.Markup("New [skyblue1]title[/] (leave blank to keep current): ");
                        string? newTitle = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newTitle))
                            selectedTask.Title = newTitle;

                        AnsiConsole.Markup("New [skyblue1]description[/] (leave blank to keep current): ");
                        string? newDesc = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newDesc))
                            selectedTask.Description = newDesc;

                        AnsiConsole.Markup("New [skyblue1]deadline[/] (dd:MM:yyyy) or leave empty: ");
                        string? deadlineInput = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(deadlineInput) &&
                            DateTime.TryParseExact(deadlineInput, "dd:MM:yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var newDeadline))
                        {
                            selectedTask.Deadline = newDeadline;
                        }

                        var priorities = new List<string> { "High", "Medium", "Low" };

                        var newPriority = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Choose new [skyblue1]priority[/] (or Cancel to keep current):")
                            .HighlightStyle("white on black")
                            .UseConverter(p => p switch   
                            {
                                "High"   => "[red]High[/]",
                                "Medium" => "[orange1]Medium[/]",
                                "Low"    => "[yellow1]Low[/]",
                                "Cancel" => "[grey]Cancel[/]",
                                _        => p
                            })
                            .AddChoices(priorities.Concat(new[] { "Cancel" }))
                    );


                        if (newPriority != "Cancel")
                        selectedTask.Priority = newPriority;

                        var categories = categoryService.GetCategories(currentUser);
                        if (categories.Count > 0)
                        {
                            var selectedCategory = AnsiConsole.Prompt(
                                new SelectionPrompt<Category>()
                                    .Title("Choose new [skyblue1]category[/] (or skip):")
                                    .HighlightStyle("skyblue1 on black")
                                    .UseConverter(c => $"[{c.Color}]{c.Name}[/]")
                                    .AddChoices(categories.Concat(new[] { new Category { Name = "Keep current", Color = "grey" } }))
                        );

                        if (selectedCategory.Name != "Keep current")
                            selectedTask.Category = selectedCategory.Name;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[orangered1]No categories found. Keeping current.[/]");
                        }
                        AnsiConsole.MarkupLine("[skyblue1]Task updated.[/]");
                        break;

                    case "Delete":
                        tasks.RemoveAt(taskIndex - 1);
                        AnsiConsole.MarkupLine("[orangered1]Task deleted.[/]");
                        SaveTasks();
                        return;

                    case "Cancel":
                        return;
                }

                SaveTasks();
                return;
            }
            else
            {
                AnsiConsole.MarkupLine("[orangered1]Invalid ID. Please enter a valid number.[/]");
                AnsiConsole.MarkupLine("[grey](Or press Enter to return to menu)[/]");
            }
        }

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
    private string GetPriorityColor(string priority)
{
    return priority switch
    {
        "High" => "[red]High[/]",
        "Medium" => "[orange1]Medium[/]",
        "Low" => "[yellow1]Low[/]",
        _ => "[grey]Unknown[/]"
    };
}

}
