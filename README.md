# Skill Survey Application
Loads skill surveys from Excel files into a target database.

## Solution Folder
Contains the .NET solution.
### >> Skill Survey Loader
Orchestrates load of Excel files into the target database, instructed by command line parameters.
### >> Skill Survey Database
Database solution for the SQL Server database that holds that survey data.
### >> Skill Survey Model
Object model and repositories used to interact with the database through Entity Framework.

## Application Folder
Compiled runtime of the Skill Survey Loader application.  Includes a sample survey file.
Modify configurations to control database access.
