using System;
using System.Collections.Generic;
using System.IO;
using Spectre.Console;
using System.Text.RegularExpressions;
using System.Text.Json;

public class AuthService
{
    private const string FilePath = "users.json";
    private List<User> users;
    public User? CurrentUser { get; private set; }

    public AuthService()
    {
        users = LoadUsers();
    }

    private List<User> LoadUsers()
    {
        if (!File.Exists(FilePath))
        {
            File.WriteAllText(FilePath, "[]");
        }

        string json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    private void SaveUsers()
    {
        string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }

    public bool Login()
    {
        string username;
        string password;

        do
        {
            Console.Write("Enter username: ");
            username = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(username))
            {
                AnsiConsole.MarkupLine($"[orangered1]Username cannot be empty[/]");
            }
        } while (string.IsNullOrWhiteSpace(username));

        do
        {
            Console.Write("Enter password: ");
            password = ReadPassword()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(password))
            {
                AnsiConsole.MarkupLine($"[orangered1]Password cannot be empty.[/]");
            }
        } while (string.IsNullOrWhiteSpace(password));

        foreach (var user in users)
        {
            if (user.Username == username && user.Password == password)
            {
                CurrentUser = user;
                Console.WriteLine("Login successful!");
                return true;
            }
        }

        AnsiConsole.MarkupLine($"[orangered1]Invalid username or password.[/]");
        return false;
    }


    public void Register()
    {
        string username;
        string password;
        string confirmPassword;

        do
        {
            Console.Write("Choose username (min. 5 characters, letters or digits): ");
            username = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(username) || username.Length < 5 || !Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"))
            {
                AnsiConsole.MarkupLine($"[orangered1]Invalid username. It must be at least 5 characters and contain only letters and digits.[/]");
                continue;
            }

            if (users.Exists(u => u.Username == username))
            {
                AnsiConsole.MarkupLine($"[orangered1]Username already exists.[/]");
                return;
            }

            break;

        } while (true);

        do
        {
            Console.Write("Choose password (min. 5 characters, at least 1 digit, 1 lowercase, 1 uppercase, 1 special): ");
            password = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Confirm password: ");
            confirmPassword = Console.ReadLine()?.Trim() ?? "";

            if (password != confirmPassword)
            {
                AnsiConsole.MarkupLine($"[orangered1]Passwords do not match.[/]");
                continue;
            }

            if (!IsValidPassword(password))
            {
                AnsiConsole.MarkupLine($"[orangered1]Invalid password. It must be at least 5 characters long and contain at least one lowercase letter, one uppercase letter, one digit, and one special character.[/]");
                continue;
            }

            break;

        } while (true);

        users.Add(new User { Username = username, Password = password });
        SaveUsers();
        Console.WriteLine("User registered successfully.");
    }

    private static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;

        while (true)
        {
            key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[..^1];
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password += key.KeyChar;
                Console.Write("*");
            }
        }

        return password;
    }
    private bool IsValidPassword(string password)
    {
        if (password.Length < 5)
            return false;

        bool hasLower = Regex.IsMatch(password, "[a-z]");
        bool hasUpper = Regex.IsMatch(password, "[A-Z]");
        bool hasDigit = Regex.IsMatch(password, "[0-9]");
        bool hasSpecial = Regex.IsMatch(password, @"[\W_]");

        return hasLower && hasUpper && hasDigit && hasSpecial;
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public void AddTask()
    {
        if (CurrentUser == null)
        {
            Console.WriteLine("Please log in to add a task.");
            return;
        }

        Console.Write("Task title: ");
        string title = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Task description: ");
        string description = Console.ReadLine()?.Trim() ?? "";

        var task = new TaskItem
        {
            Title = title,
            Description = description,
            RegisteredTime = DateTime.Now,
            IsStarted = false,
            IsCompleted = false
        };

        CurrentUser.Tasks.Add(task);
        SaveUsers();
        Console.WriteLine("Task added successfully.");
    }

    public void StartTask()
    {
        if (CurrentUser == null)
        {
            Console.WriteLine("Please log in to start a task.");
            return;
        }

        var pendingTasks = CurrentUser.Tasks
            .Where(t => !t.IsStarted)
            .ToList();

        if (!pendingTasks.Any())
        {
            Console.WriteLine("No tasks to start.");
            return;
        }

        Console.WriteLine("Pending tasks:");
        for (int i = 0; i < pendingTasks.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {pendingTasks[i].Title}");
        }

        Console.Write("Select task number to start: ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= pendingTasks.Count)
        {
            var task = pendingTasks[choice - 1];
            task.StartTime = DateTime.Now;
            task.IsStarted = true;
            SaveUsers();
            Console.WriteLine($"Started task: {task.Title}");
        }
        else
        {
            Console.WriteLine("Invalid selection.");
        }
    }

    public void CompleteTask()
    {
        if (CurrentUser == null)
        {
            Console.WriteLine("Please log in to complete a task.");
            return;
        }

        var startedTasks = CurrentUser.Tasks
            .Where(t => t.IsStarted && !t.IsCompleted)
            .ToList();

        if (!startedTasks.Any())
        {
            Console.WriteLine("No started tasks to complete.");
            return;
        }

        Console.WriteLine("Started tasks:");
        for (int i = 0; i < startedTasks.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {startedTasks[i].Title} (Started at {startedTasks[i].StartTime})");
        }

        Console.Write("Select task number to complete: ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= startedTasks.Count)
        {
            var task = startedTasks[choice - 1];
            task.CompletedTime = DateTime.Now;
            task.IsCompleted = true;
            SaveUsers();
            Console.WriteLine($"Task completed! Duration: {task.Duration?.ToString(@"hh\:mm\:ss")}");
        }
        else
        {
            Console.WriteLine("Invalid selection.");
        }
    }

public void CreateNewCategory()
{
    AnsiConsole.MarkupLine("[bold blue]Create a [green]new category[/]:[/]");

    string name = AnsiConsole.Ask<string>("Enter category name (max 20 characters):").Trim();
    if (name.Length > 20)
    {
        name = name.Substring(0, 20);
        AnsiConsole.MarkupLine("[grey]Category name was truncated to 20 characters.[/]");
    }

    var color = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Choose a [green]category color[/]:")
            .AddChoices(new[] {
                "red", "green", "blue", "yellow", "cyan", "magenta", "white", "grey"
            }));

    string formattedCategory = $"[{color}]{name}[/]";
    AnsiConsole.MarkupLine($"\n[bold green]Created category:[/] {formattedCategory}");

    // Możesz też zapisać ją np. do pliku albo listy, jeśli chcesz je później wybierać
}

}
