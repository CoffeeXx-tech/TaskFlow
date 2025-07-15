# FlowTask

**FlowTask** is a lightweight, Spectre.Console‑powered CLI written in **C# / .NET 8**.  
It lets you log in, create color‑coded categories, add tasks with priorities and deadlines, and track task progress — all stored in local JSON files.

---

## ✨ Key Features
| Area | Details |
|------|---------|
| Authentication | Register, login, logout (credentials saved in `users.json`) |
| Tasks | Title, description, deadline, **High / Medium / Low priority**, start / stop timer, completion flag, auto‑calculated duration |
| Categories | Per‑user list (`categories.json`) with **name, description, color**; edit or delete; tasks can be assigned to any category or left *Uncategorized* |
| Colorful CLI | Spectre.Console tables, figlet banner, selection prompts with inline ANSI colors |
| Persistence | All data stored locally in `users.json`, `tasks.json`, `categories.json` |

---

## 🎨 Console UI – Powered by Spectre.Console

FlowTask uses the **[Spectre.Console](https://spectreconsole.net/)** library  
(imported with `using Spectre.Console;`) to build a rich, interactive command-line interface:

- `AnsiConsole.MarkupLine()` — output with inline colors and styles  
- `SelectionPrompt<T>()`, `TextPrompt<T>()` — interactive menus and inputs  
- `Table`, `FigletText`, `Panel`, `ProgressBar` — styled layout and widgets

To install the package manually:
```bash
dotnet add package Spectre.Console

```
All CLI styling (colors, layouts, prompts) is implemented using Spectre's built-in tools.

🗂️ Project Structure
├── Program.cs             # Main menu / entry-point
├── Models
│   ├── User.cs            # User + personal task list
│   ├── TaskItem.cs        # Task entity
│   └── Category.cs        # Category entity
├── Services
│   ├── AuthService.cs     # Register / login / logout
│   ├── TaskService.cs     # CRUD + timers + colored priorities
│   └── CategoryService.cs # CRUD + colored selection
└── *.json                 # Local data stores

🚀 Running the App
# 1. Clone
git clone https://github.com/<your‑user>/FlowTask.git
cd FlowTask

# 2. Build & run (requires .NET 8 SDK)
dotnet run
Tip: JSON data files are created automatically on first run.
Delete them to reset all data.

🖥️ Usage Overview

    Login / Register
    Credentials are validated (strong-password rules)

    Main Menu (after login)
    3 Add Task 4 Show Tasks 5 New Category 6 Categories / Edit 8 Logout

    Colored Prompts

        Category color picker shows live ANSI swatches

        Task priority picker shows:

            High → red

            Medium → orange

            Low → yellow

    Task Actions
    Start ⟶ Stop ⟶ Complete / Unmark / Edit / Change Category / Delete

🔧 Dependencies
Package	Purpose
Spectre.Console v0.48	Rich TUI (tables, prompts, figlet)
System.Text.Json	Lightweight JSON persistence
📄 License

MIT (see LICENSE).

FlowTask is a hobby / learning project — feel free to fork, improve and open PRs!