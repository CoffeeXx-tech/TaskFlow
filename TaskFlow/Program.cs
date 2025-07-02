using System;
using Spectre.Console;
// TaskFlow - A simple task management application
class Program
{
    static void Main()
    {
        var escListener = new EscListener();
        escListener.StartListening();
        
        var authService = new AuthService(); 
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
            AnsiConsole.MarkupLine("[bold orange1]You are not logged in. Choose 1 to log in.[/]");
        }
            var table = new Table();
            table.Border = TableBorder.Rounded;
            table.BorderColor(Color.LightSeaGreen);
            table.AddColumn("[bold lightseagreen]Option[/]");
            table.AddColumn("[bold lightseagreen]Description[/]");

            table.AddRow("[aquamarine1]1[/]", "[white]Login[/]");
            table.AddRow("[aquamarine1]2[/]", "[white]Register New User[/]");
            table.AddRow("[aquamarine1]3[/]", "[white]Add Task[/]");
            table.AddRow("[aquamarine1]4[/]", "[white]Show Tasks[/]");
            table.AddRow("[aquamarine1]5[/]", "[white]Mark Task Completed[/]");
            table.AddRow("[aquamarine1]6[/]", "[white]Statistics[/]");
            table.AddRow("[aquamarine1]7[/]", "[white]Calendar Sync[/]");
            table.AddRow("[aquamarine1]8[/]", "[white]Logout[/]");
            table.AddRow("[lightSalmon1]0[/]", "[lightSalmon1]Exit[/]");

            AnsiConsole.Write(table);

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
                    AnsiConsole.MarkupLine("[green]Add task function (to be implemented)...[/]");
                    break;
                case "4":
                    AnsiConsole.MarkupLine("[green]Show tasks function (to be implemented)...[/]");
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