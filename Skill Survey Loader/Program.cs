using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Centric.SkillSurvey.Models;
using Centric.SkillSurvey.Repositories;
using System.Configuration;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace Centric.SkillSurvey
{
  class Program
  {
    private static string SourceFolderPath;
    
    private static string ArchiveFolderName = "Archive";
    private static string ArchiveFolderPath;

    private static string FailedFolderName = "Failed";
    private static string FailedFolderPath;

    private static string SourceFilePath;
    
    static void Main(string[] args)
    {
    
      CollectCommandLineParameters(args);
      VerifyTargetPaths();

      // create the application context
      string ConnectionString = ConfigurationManager.ConnectionStrings["APPREPO"].ConnectionString;
      ApplicationContext AppContext = new ApplicationContext(ConnectionString);
      //BootstrapApplication(AppContext); 
      
      ProcessExcelFiles(AppContext);
      
    }

    private static void ProcessExcelFiles(ApplicationContext AppContext)
    {
      Excel.Application ExcelApp = null;

      try
      {
        EventLogger.Log(AppContext, "Start", "Excel", "Start Excel application");

        ExcelApp = new Excel.Application();
        ExcelApp.Visible = false;

        // loop through relevant files
        foreach(string ExcelFilePath in RetrieveExcelFilePaths())
        {
          ProcessExcelFile(ExcelApp, AppContext, ExcelFilePath);
        }

        EventLogger.Log(AppContext, "Finished", "Application", "Load is complete.");
      }
      catch (Exception ex)
      {
        EventLogger.Log(AppContext, "Error", "Application", "Error: " + ex.Message);
      }
      finally
      {
        EventLogger.Log(AppContext, "Quit", "Excel", "Quitting Excel application.");
        ExcelApp.Quit();
      }
    }

    private static List<string> RetrieveExcelFilePaths()
    {
      List<string> ExcelFilePaths = Directory.GetFiles(Program.SourceFolderPath, "*.xls*").ToList<string>();

      if(SourceFilePath != null && !ExcelFilePaths.Exists(x => x.Equals(SourceFilePath)))
      {
        ExcelFilePaths.Add(SourceFilePath);
      }

      return ExcelFilePaths;

    }

    private static void ProcessExcelFile(Excel.Application ExcelApp, ApplicationContext AppContext, string ExcelFilePath)
    {

      SurveyExcelLoad loader = null;
      
      try
      {
        EventLogger.Log(AppContext, "Started", ExcelFilePath, "Starting the loader.");

        loader = new SurveyExcelLoad(AppContext, ExcelFilePath);
        loader.Load(ExcelApp);

        // post-processing file management

        string FileExtension = Path.GetExtension(ExcelFilePath);
        string FileNameRaw = Path.GetFileNameWithoutExtension(ExcelFilePath);
        string TargetFileName = FileNameRaw + "." + (Guid.NewGuid().ToString("D")) + FileExtension;

        if (loader.LoadSucceed)
        {
          // move to archive folder
          EventLogger.Log(AppContext, "File", ExcelFilePath, "Moving to archive folder.");

          // delete old target path if exists
          string TargetPath = Path.Combine(ArchiveFolderPath, TargetFileName);            
          if (File.Exists(TargetPath)) File.Delete(TargetPath);

          Directory.Move(ExcelFilePath, TargetPath);
        }
        else
        {
          // move to failed folder
          EventLogger.Log(AppContext, "File", ExcelFilePath, "Moving to failed folder.");

          // delete old target path if exists
          string TargetPath = Path.Combine(FailedFolderPath, TargetFileName);
          if (File.Exists(TargetPath)) File.Delete(TargetPath);

          Directory.Move(ExcelFilePath, TargetPath);
        }
        

        EventLogger.Log(AppContext, "Finished", ExcelFilePath, "Load is complete.");
      }
      catch (Exception ex)
      {
        EventLogger.Log(AppContext, "Error", ExcelFilePath, "Error: " + ex.Message);
      }
      finally
      {
        ExcelApp.Quit();
      }
    }

    private static void VerifyTargetPaths()
    {

      // verify the source folder exists
      if (!Directory.Exists(SourceFolderPath))
      {
        throw new ApplicationException("The source folder path does not exist.");
      }

      // determine or create the archive folder
      if (Program.ArchiveFolderPath == null || Program.ArchiveFolderPath.Trim().Length == 0)
      {
        ArchiveFolderPath = SourceFolderPath + @"\" + ArchiveFolderName;

        if (!Directory.Exists(ArchiveFolderPath))
        {
          Directory.CreateDirectory(ArchiveFolderPath);
        }
      }

      if (!Directory.Exists(ArchiveFolderPath))
      {
        throw new ApplicationException("The archive folder path does not exist.");
      }

      // determine or create the failed folder
      if (Program.FailedFolderPath == null || Program.FailedFolderPath.Trim().Length == 0)
       {
        FailedFolderPath = SourceFolderPath + @"\" + FailedFolderName;        

        if(!Directory.Exists(FailedFolderPath))
        {
          Directory.CreateDirectory(FailedFolderPath);
        }
       }

      if (!Directory.Exists(FailedFolderPath))
      {
        throw new ApplicationException("The failed folder path does not exist.");
      }


      if (ArchiveFolderPath.Equals(FailedFolderPath))
      {
        throw new ApplicationException("The archive and folder path must be different.");
      }       
    }

    private static void CollectCommandLineParameters(string[] args)
    {
      int argpos = 0;

      foreach (string arg in args)
      {

        if (arg.StartsWith("--"))
        {
          switch (arg)
          {
            case "--file":
              Program.SourceFilePath = args[argpos + 1].Replace("\"", "");
              break;

            case "--source":
              Program.SourceFolderPath = args[argpos + 1].Replace("\"", "");
              break;

            case "--archive":
              Program.ArchiveFolderPath = args[argpos + 1].Replace("\"", "");
              break;

            case "--failed":
              Program.FailedFolderPath = args[argpos + 1].Replace("\"", "");
              break;

            case "--archive-name":
              Program.ArchiveFolderName = args[argpos + 1].Replace("\"", "");
              break;

            case "--failed-name":
              Program.FailedFolderName = args[argpos + 1].Replace("\"", "");
              break;

            default:
              break;

          }
        }
        argpos++;
      }
    }

    private static void BootstrapApplication(ApplicationContext AppContext)
    {
      Bootstrap.InitializeDatabase(AppContext);
      //AppContext.DropCreateViews();
      Bootstrap.Execute(AppContext);
    }


  }
}
