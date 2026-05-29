## Life Pro Manager
A to-do list manager in C# with WinForms and a SQLite database for offline storage.

Includes birthday reminders and topic-based lists.

This fork is the actively maintained version, as the original co-author has stopped development.

[![Release](https://img.shields.io/badge/release-stable-64B07B)](https://github.com/laurentbarraud/LifeProManager/releases)
[![Downloads](https://img.shields.io/github/downloads/laurentbarraud/LifeProManager/latest/total?color=88aacc&style=flat)](https://github.com/laurentbarraud/LifeProManager/releases/latest)

<p align="center">
  <img src="https://raw.githubusercontent.com/laurentbarraud/LifeProManager/refs/heads/master/LifeProManager/doc/main-window.jpg"
       alt="Main window screenshot"
       style="width: 65%;">
</p>

<p align="center">
  <img src="https://raw.githubusercontent.com/laurentbarraud/LifeProManager/refs/heads/master/LifeProManager/doc/add-task-window.jpg"
       alt="Add task window screenshot"
       style="width: 30%;" min-width: 180px;>
  <img src="https://raw.githubusercontent.com/laurentbarraud/LifeProManager/refs/heads/master/LifeProManager/doc/birthday-calendar.jpg"
       alt="Birthday calendar screenshot"
       style="width: 48%;" min-width: 180px;>
</p>

### Features
- 🚀 Fast and responsive — even with 1,000+ tasks
- 🧩 Straightforward architecture with clear responsibilities and predictable UI logic  
- 🔍 Task search engine with natural language date parsing, fuzzy lexical matching and semantic relevance scoring.
🧪 Built‑in test runner validating all search cases at once using 40 mock‑tasks, which can all be inserted via the provided script in the /scripts folder. 
- 📦 Smart file-based architecture — everything fits in a single, portable DB file
- 📤 Export tasks to a clean, date‑sorted HTML page
- 🎂 Birthday calendar listing all names and the age they’ll reach this year
- 🪟 Responsive main window with a sliding right panel and persistent user‑defined width
- 🌐 Localized in English, French and Spanish.

(the MonthCalendar control follows the OS culture settings, as per WinForms design).

### How to Run
- Clone this repository with Git
- Open in Visual Studio 2022
- Build the project with CTRL+B and run it.

### Download
Go to the [Releases](https://github.com/laurentbarraud/LifeProManager/releases) section to download a ready-to-use installer,  
designed for x64-based Windows systems (Windows 7 and above). 

### Contributing
For any suggestion of improvement or bug report, feel free to:

- Open an issue
- Submit a PR if you can code it yourself
- Or simply contact me by mail to talk about it. 

If this project inspires you or helped you learn something, feel free to drop a star — it’s always appreciated.
