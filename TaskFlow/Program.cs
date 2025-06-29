using System;
using Spectre.Console;
// TaskFlow - A simple task management application
class Program
{
    static void Main()
    {
        bool running = true;

        while (running)
        {
            
            Console.Clear();

AnsiConsole.Write(
                new FigletText("FlowTask")
                    .Color(Color.SkyBlue1)
                    .Centered());

            AnsiConsole.MarkupLine("[bold lightseagreen]Welcome to FlowTask![/]");
            var table = new Table();
            table.Border = TableBorder.Rounded;
            table.BorderColor(Color.LightSeaGreen);
            table.AddColumn("[bold lightseagreen]Option[/]");
            table.AddColumn("[bold lightseagreen]Description[/]");

            table.AddRow("[aquamarine1]1[/]", "[white]Login[/]");
            table.AddRow("[aquamarine1]2[/]", "[white]Add Task[/]");
            table.AddRow("[aquamarine1]3[/]", "[white]Show Tasks[/]");
            table.AddRow("[aquamarine1]4[/]", "[white]Mark Task Completed[/]");
            table.AddRow("[aquamarine1]5[/]", "[white]Statistics[/]");
            table.AddRow("[aquamarine1]6[/]", "[white]Calendar Sync[/]");
            table.AddRow("[lightSalmon1]0[/]", "[lightSalmon1]Exit[/]");

            AnsiConsole.Write(table);



//
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
                    AnsiConsole.MarkupLine("[green]Login function (to be implemented)...[/]");
                    break;
                case "2":
                    AnsiConsole.MarkupLine("[green]Add task function (to be implemented)...[/]");
                    break;
                case "3":
                    AnsiConsole.MarkupLine("[green]Show tasks function (to be implemented)...[/]");
                    break;
                case "4":
                    AnsiConsole.MarkupLine("[green]Mark task as completed function (to be implemented)...[/]");
                    break;
                case "5":
                    AnsiConsole.MarkupLine("[green]Statistics function (to be implemented)...[/]");
                    break;
                case "6":
                    AnsiConsole.MarkupLine("[green]Calendar sync function (to be implemented)...[/]");
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