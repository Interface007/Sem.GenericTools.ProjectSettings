# Sem.GenericTools.ProjectSettings
A tool to process project settings of multiple VS2008 and VS2010 C# projects in Excel. The tool scans the project files and exports some settings into a CSV file which can be edited and written back into the project files. This is usefull in solutions with many project files.

This project has helped me with one solution of about 40 C#-projects and another one with 90 C#-projects. I found some projects to not execute the code analysis in the correct build definition and different runtime targets, so I wrote this little tool to export some of the settings of all projects in the subfolders of this executable into a csv file that can be edited with Microsoft Excel. Currently this project has only be tested with two solutions. Also you should put your solution under source control (which is always a good idea) or make any other type of backup BEFORE running this tool.

It's a command line tool, so you will have to copy it in the parent folder of the project folders that should be scanned. When starting the tool it will ask you what to do - the sequence is C-O-W-E:
press "c" + ENTER to copy the project settings into a csv file (will be generated inside the same folder the exe has been started).
press "o" + ENTER to open the file - if you have installed Microsoft Excel, it will open the file.
now edit the settings and save the csv (I would recommend only to copy settings from here to there without typing text manually, because you might enter values that are not understood by Visual Studio)
press "w" + ENTER to write the settings from the csv file into the project files
press "e" + ENTER to quit the program - you did finish

This project does has been tested with C#-project-files from Microsoft Visual Studio 2008 and 2010. If you have any reproducable problem: send me a sample solution and I will fix the problem.
