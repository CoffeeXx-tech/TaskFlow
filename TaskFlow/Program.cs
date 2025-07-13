using System;
using Spectre.Console;
class Program
{
    static void Main()
    {        
        var authService = new AuthService(); 
        var taskService = new TaskService();
        CategoryService categoryService = new CategoryService();
        User? currentUser = null;

        bool running = true;

        while (running)
        {

            Console.Clear();


            AnsiConsole.Write(
                    new FigletText("FlowTask")
                        .Color(Color.SkyBlue1)
                        .Centered());

            if (authService.CurrentUser != null)
{
    AnsiConsole.MarkupLine($"[skyblue1]Welcome, {authService.CurrentUser.Username}![/]");
}
else
{
    AnsiConsole.MarkupLine("[skyblue1]Welcome to FlowTask![/]");
    AnsiConsole.MarkupLine("[bold orange1]You are not logged in. Please log in or register below.[/]");
}

if (authService.CurrentUser == null)
{
    var loginTable = new Table();
    loginTable.Title("🔐 Authentication");
    loginTable.Border = TableBorder.Rounded;
    loginTable.BorderColor(Color.SkyBlue1);
    loginTable.AddColumn("[bold skyblue1]Option[/]");
    loginTable.AddColumn("[bold skyblue1]Description[/]");

    loginTable.AddRow("[aquamarine1]1[/]", "[white]Login[/]");
    loginTable.AddRow("[aquamarine1]2[/]", "[white]Register New User[/]");
    loginTable.AddRow("[orangered1]0[/]", "[orangered1]Exit[/]");

    AnsiConsole.Write(loginTable);
}
else
{
    var functionTable = new Table();
    functionTable.Title("📋 FlowTask Menu");
    functionTable.Border = TableBorder.Rounded;
    functionTable.BorderColor(Color.SkyBlue1);
    functionTable.AddColumn("[bold skyblue1]Option[/]");
    functionTable.AddColumn("[bold skyblue1]Description[/]");

    functionTable.AddRow("[aquamarine1]3[/]", "[white]Add Task[/]");
    functionTable.AddRow("[aquamarine1]4[/]", "[white]Show Tasks[/]");
    functionTable.AddRow("[aquamarine1]5[/]", "[white]New category[/]");
    functionTable.AddRow("[aquamarine1]6[/]", "[white]Categories[/]");
    functionTable.AddRow("[lightSalmon1]8[/]", "[lightSalmon1]Logout[/]");
    functionTable.AddRow("[orangered1]0[/]", "[orangered1]Exit[/]");

    AnsiConsole.Write(functionTable);
}

            string? choice; // Initialize choice variable, can hold text or null

            do
            {
                AnsiConsole.Markup("[bold orange1]Choose an option:[/] ");
                choice = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(choice))
                {
                    AnsiConsole.Markup("[orangered1]Input cannot be empty. Please enter a valid option!\n[/] ");
                }

            } while (string.IsNullOrWhiteSpace(choice));

            choice = choice.Trim(); // Remove any leading or trailing whitespace

            switch (choice)
            {
                case "1":
        if (currentUser != null)
        {
            AnsiConsole.MarkupLine("[yellow]You are already logged in.[/]");
            break;
        }
        authService.Login();
        currentUser = authService.CurrentUser;
        break;
                case "2":
        if (currentUser != null)
        {
            AnsiConsole.MarkupLine("[yellow]You are already logged in.[/]");
            break;
        }
        authService.Register();
        break;
                case "3":
                    if (authService.CurrentUser != null)
                        {
                            taskService.AddTask(authService.CurrentUser, categoryService);
                        }
                    else
                        {
                            Console.WriteLine("You must be logged in to add tasks.");
                        }
                    break;
                case "4":
                    if (authService.CurrentUser != null)
                    {
                        taskService.ShowTasks(authService.CurrentUser, categoryService);

                        }
                    else
                    {
                        Console.WriteLine("You must be logged in to view tasks.");
                    }
                    break;
                case "5":
    if (authService.CurrentUser != null)
    {
        categoryService.AddCategory(authService.CurrentUser);
    }
    else
    {
        AnsiConsole.MarkupLine("[orangered1]You must be logged in to add a category.[/]");
    }
    break;

                case "6":
    if (authService.CurrentUser != null)
    {
        categoryService.ShowCategories(authService.CurrentUser);

        if (AnsiConsole.Confirm("Do you want to edit a category?"))
        {
            categoryService.EditCategory(authService.CurrentUser);
        }
    }
    else
    {
        Console.WriteLine("You must be logged in to view categories.");
    }
    break;

                case "8":
                    if (authService.CurrentUser != null)
                    {
                        authService.Logout();
                        AnsiConsole.MarkupLine("[bold orange1]You have been logged out.[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[orangered1]You are not logged in.[/]");
                    }
                    break;
                case "0":
                    running = false;
                    AnsiConsole.MarkupLine("[bold orange1]Goodbye![/]");
                    break;
                default:
                    AnsiConsole.MarkupLine("[orangered1]Unknown option, please try again.[/]");
                    break;
            }

            if (running)
            {
                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey();
            }
        }
    }
}