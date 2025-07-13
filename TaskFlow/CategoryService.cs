using System.Text.Json;
using Spectre.Console;

public class CategoryService
{
    private const string CategoryFilePath = "categories.json";
    private Dictionary<string, List<Category>> userCategories = new();

    public CategoryService()
    {
        LoadCategories();
    }

    public void AddCategory(User currentUser)
{
    if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.Username))
    {
        AnsiConsole.MarkupLine("[orangered1]You must be logged in to create a category.[/]");
        return;
    }

    string name = AnsiConsole.Ask<string>(
        "Enter [green]category name[/] (max 20 characters):").Trim();

    if (string.IsNullOrWhiteSpace(name) || name.Length > 20 ||
        name.Equals("None", StringComparison.OrdinalIgnoreCase) ||
        name.Equals("Uncategorized", StringComparison.OrdinalIgnoreCase))
    {
        AnsiConsole.MarkupLine("[orangered1]Invalid or reserved name.[/]");
        return;
    }

    string description = AnsiConsole.Ask<string>(
        "Enter [green]optional description[/] (or leave empty):").Trim();

    string[] colors = {
        "red","green","blue","yellow","purple",
        "aqua","orange1","deepskyblue1","skyblue1","white"
    };

    string color = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Select a [green]color[/] for the category")
            .PageSize(10)
            .HighlightStyle("purple on black")
            .UseConverter(c => $"[{c}]{c}[/]")
            .AddChoices(colors));

    string username = currentUser.Username;
    if (!userCategories.ContainsKey(username))
        userCategories[username] = new List<Category>();

    userCategories[username].Add(new Category
    {
        Name = name,
        Color = color,
        Description = description
    });

    SaveCategories();
    AnsiConsole.MarkupLine($"[green]Category '{name}'[/] with color '[{color}]{color}[/]' added.");
}


    public List<Category> GetCategories(User currentUser)
{
    var defaultCategory = new Category
    {
        Name = "Uncategorized",
        Color = "gray",
        Description = "No specific category"
    };

    if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.Username))
        return new List<Category> { defaultCategory };

    if (userCategories.TryGetValue(currentUser.Username, out var categories))
        return [..categories, defaultCategory];

    return new List<Category> { defaultCategory };
}


    private void LoadCategories()
    {
        if (!File.Exists(CategoryFilePath)) return;

        string json = File.ReadAllText(CategoryFilePath);
        userCategories = JsonSerializer.Deserialize<Dictionary<string, List<Category>>>(json) ?? new();
    }

    private void SaveCategories()
    {
        string json = JsonSerializer.Serialize(userCategories, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(CategoryFilePath, json);
    }
public void ShowCategories(User currentUser)
{
    if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.Username))
    {
        AnsiConsole.MarkupLine("[orangered1]You must be logged in to view categories.[/]");
        return;
    }

    string username = currentUser.Username;

    if (!userCategories.ContainsKey(username) || userCategories[username].Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]No categories found.[/]");
        return;
    }

    var categories = userCategories[username];

    var table = new Table();
    table.AddColumn("[bold]ID[/]");
    table.AddColumn("[bold]Name[/]");
    table.AddColumn("[bold]Color[/]");
    table.AddColumn("[bold]Description[/]");

    int index = 1;
        foreach (var cat in categories)
        {
            table.AddRow(
                $"[grey]{index++}[/]",
                $"[{cat.Color}]{cat.Name}[/]",
                $"[{cat.Color}]{cat.Color}[/]",
                string.IsNullOrWhiteSpace(cat.Description) ? "[grey]No description[/]" : cat.Description
            );
        }

    AnsiConsole.Write(table);
}

public void EditCategory(User currentUser)
{

    if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.Username))
        {
            AnsiConsole.MarkupLine("[orangered1]You must be logged in to edit categories.[/]");
            return;
        }

    var username = currentUser.Username;
    if (!userCategories.ContainsKey(username) || userCategories[username].Count == 0)
    {
        AnsiConsole.MarkupLine("[orangered1]No categories to edit.[/]");
        return;
    }

    var categories = userCategories[username];

    var selection = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Select [skyblue1]category[/] to edit:")
            .PageSize(10)
            .HighlightStyle("skyblue1 on black")
            .AddChoices(categories.Select((cat, i) => $"{i + 1}. [{cat.Color}]{cat.Name}[/]")));

    int selectedIndex = int.Parse(selection.Split('.')[0]) - 1;
    var selectedCategory = categories[selectedIndex];

    string newName = AnsiConsole.Prompt(
    new TextPrompt<string>($"Enter new name (or press Enter to keep '[{selectedCategory.Color}]{selectedCategory.Name}[/]'):")
        .AllowEmpty() 
        .PromptStyle("skyblue1")
);

if (!string.IsNullOrWhiteSpace(newName) && newName.Length <= 20)
{
    selectedCategory.Name = newName.Trim();
}


    var colors = new List<string>
{
    "Keep current",
    "red", "green", "blue", "yellow", "aqua",
    "orange1", "deepskyblue1", "skyblue1", "white"
};

string newColor = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Choose a new color (or Keep current):")
        .PageSize(10)
        .HighlightStyle("purple on black")
        .AddChoices(colors));

if (newColor != "Keep current")          
    selectedCategory.Color = newColor;

string newDescription = AnsiConsole.Prompt(
    new TextPrompt<string>("Enter new description (or press Enter to keep current):")
        .AllowEmpty()               
        .PromptStyle("skyblue1")       
);

if (!string.IsNullOrWhiteSpace(newDescription))
{
    selectedCategory.Description = newDescription.Trim();
}

SaveCategories();

    AnsiConsole.MarkupLine($"Category updated [skyblue1]successfully![/]");
}

}
