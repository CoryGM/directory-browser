# Overview
This is a project for an interview test project written in C# and JavaScript

The project instructions specifically mentioned it was not important to make the UI pretty. 

# Instructions for Use
The project can simply be cloned or downloaded from this repo. It is written in .NET 9.

## Configuration
Requirements specified the server side home directory should be configurable via a variable. 

The default home directory is defaulted to c:\temp. If running the project on a server that does not have that directory, update the value in `appsettings.json` with the key "Browse:HomeDirectory" to the desired location. 

## Running the project
- Option 1: Once the code is obtained, the solution can be opened with Visual Studio 2022 and run normally.
- Option 2: It can be run from the command line if desired. Simply open a command window in the same location as the Browser.csproj file and enter `dotnet run`. Then open a browser to (http://localhost:5120).

# Assumptions
These are some of the assumptions I made during the creation of the project. 

- The target OS of the server was not specified. Because this is written in .NET and can run on multiple OSes I spent a little extra time making sure it would work for Windows and Linux. I do not have a MacOS machine to test on.
- The project does not search on sub-directories by default. This was done because pointing to a large directory with thousands of deeply nested files such as C:\Windows or C:\Project Files would likely result in poor performance. The user should be able to choose if they want to search deeply in such an environment.
- The requirements called for searching on files **and** folders. As such the API returns separate results for both file names and directory names that matched the search terms. The tab for files includes the directory structure of the file location that matched the search criteria. These are NOT the directories that matched. The matching directories are presented on their own tab. 
- Because this is an evaluation project and it's frequently useful to see the raw data returned from the API, I included a Raw Data tab. This tab shows the JSON returned from API without the need to look at a browser's network tab to see the raw payload.
- It is usually a bad idea to give the end user too much information about internal systems. The project allows the user to search for files and directories but the user does not know where the base directory is located. All locations shown in the UI are relative to that location and the server handles the translation to the actual physical location.
- A corollary to the previous point, to hide the server details from the user even in the API results, the full detail returned from the browsing service is transformed into a redacted result for consumption by the UI.
- File system operations in .NET are synchronous by nature. In a web service environment reducing blocking operations improves server performance and increases the number of clients a single server can handle. The browsing service wraps the synchronous operations in an asynchronous operation so the file operations can be carried out on a background thread while the service is handling other clients.

# Original Requirements

Ensure your solution builds in Visual Studio (any version 2022 or newer is acceptable), Rider, VS Code, or command line SDK tools.

- Web API that allows Browse and Search Files & Folders and returns JSON
- Deep linkable URL pattern
- SPA (Single Page App using JavaScript)
- Upload/download files from the browser
- Show file and folder counts and sizes for the current view
- Build the UI using vanilla JavaScript or TypeScript (without React, Angular, or other UI library)
- Focus on Code: Spend most of your time actually writing code so that we can have a substantial work product to review. For the purpose of this exercise we really need to see original code and are not interested in seeing a lot of framework or template usage.
  
### Bonus
- Any cool stuff you want to show :)
- Entire component contained in a dialog widget, with a trigger element (button, etc)
- Delete, move, copy files and folders
- Performance - performance is highly value
