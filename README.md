## Life Pro Manager
A to-do list manager in C# with WinForms and a SQLite database for local storage.

Includes topic-based lists and a birthday calendar covering the entire year.

This fork is the actively maintained version, as the original co-author has stopped its development.

  <a href="https://github.com/laurentbarraud/LifeProManager/releases">
  <img src="https://img.shields.io/badge/release-stable-64B07B" alt="Release"></a>
<a href="https://github.com/laurentbarraud/LifeProManager/releases/latest">
  <img src="https://img.shields.io/github/downloads/laurentbarraud/LifeProManager/latest/total?color=88aacc&style=flat" alt="Downloads"></a>

<p align="center">
<img src="https://raw.githubusercontent.com/laurentbarraud/LifeProManager/master/doc/main-window.jpg"
alt="Main window"
width="300" />
</p>
<p align="center">
<img src="https://raw.githubusercontent.com/laurentbarraud/LifeProManager/master/doc/add-task-window.jpg"
alt="Add task window"
width="200" />
<img src="https://raw.githubusercontent.com/laurentbarraud/LifeProManager/master/doc/birthday-calendar.jpg"
alt="Add task window"
width="300" />
</p>

### Features
- 🚀 Fast, even with over 1,000 tasks.
- 🧩 Straightforward architecture with clear responsibilities and predictable UI logic.
- 📦 Everything fits in a single DB file.
- 🔍 Task search engine with natural‑language date parsing (similar to Outlook’s agenda), fuzzy lexical matching and semantic relevance scoring for results display.
- 🧪 Built‑in test runner validating all search cases at once using 40 mock tasks, which can all be inserted via the provided script.
- 📤 Export tasks to a date‑sorted webpage for easy access on the go.
- 📤 Export tasks to an SQL script for personal backups.
- 🎂 Birthday calendar listing all names and the age they’ll reach this year.
- 📜 Panel visible only when a task is selected, letting you read key details at the last moment.
- 🪟 Responsive main window with a sliding right panel and persistent user‑defined width.
- 🎨 Customizable colors with pre‑made themes for instantly changing the application's appearance.
- ⌨️ Keyboard navigation and shortcuts to manage your tasks (up/down arrows, Enter, Delete key)
- 🌐 Localized in English, French, and Spanish.

(the MonthCalendar control follows the OS culture settings, as per WinForms design).

### How to Run
- Clone this repository with Git
- Open in Visual Studio 2022
- Build the project with CTRL+B and run it.

### Download
Go to the [Releases](https://github.com/laurentbarraud/LifeProManager/releases) section to download a packaged installer.

### Contributing
For any suggestion of improvement or bug report, feel free to:
- Open an issue
- Submit a PR if you can code it yourself 
- Or contact me by mail.
