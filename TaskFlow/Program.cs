using System;
using Spectre.Console;
// TaskFlow - A simple task management application
class Program
{
    static void Main()
    {        
        var authService = new AuthService(); 
        var taskService = new TaskService();
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
    loginTable.AddRow("[lightSalmon1]0[/]", "[lightSalmon1]Exit[/]");

    AnsiConsole.Write(loginTable);
}
else
{
    var functionTable = new Table();
    functionTable.Title("📋 FlowTask Menu");
    functionTable.Border = TableBorder.Rounded;
    functionTable.BorderColor(Color.LightSeaGreen);
    functionTable.AddColumn("[bold lightseagreen]Option[/]");
    functionTable.AddColumn("[bold lightseagreen]Description[/]");

    functionTable.AddRow("[aquamarine1]3[/]", "[white]Add Task[/]");
    functionTable.AddRow("[aquamarine1]4[/]", "[white]Show Tasks[/]");
    functionTable.AddRow("[aquamarine1]5[/]", "[white]New category[/]");
    functionTable.AddRow("[aquamarine1]6[/]", "[white]Categories[/]");
    functionTable.AddRow("[yellow]8[/]", "[yellow]Logout[/]");
    functionTable.AddRow("[lightSalmon1]0[/]", "[lightSalmon1]Exit[/]");

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
                    authService.Login();
                    break;
                case "2":
                    authService.Register();
                    break;
                case "3":
                    if (authService.CurrentUser != null)
                        {
                            taskService.AddTask(authService.CurrentUser);
                        }
                    else
                        {
                            Console.WriteLine("You must be logged in to add tasks.");
                        }
                    break;
                case "4":
                    if (authService.CurrentUser != null)
                        {
                            taskService.ShowTasks(authService.CurrentUser);
                        }
                    else
                        {
                            Console.WriteLine("You must be logged in to view tasks.");
                        }
                    break;
                case "5":
                    AnsiConsole.MarkupLine("[green]Mark task as completed function (to be implemented)...[/]");
                    break;
                case "6":
                    AnsiConsole.MarkupLine("[green]Statistics function (to be implemented)...[/]");
                    break;
                case "7":
                    AnsiConsole.MarkupLine("[green]Calendar sync function (to be implemented)...[/]");
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