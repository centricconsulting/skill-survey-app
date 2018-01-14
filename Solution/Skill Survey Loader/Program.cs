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
using System.Runtime.InteropServices;

namespace Centric.SkillSurvey
{
  class Program
  {
    private static string SourceFolderPath;
    private static string SourceFilePath;

    private static string ArchiveFolderName = "Archive";
    private static string ArchiveFolderPath;

    private static string FailedFolderName = "Failed";
    private static string FailedFolderPath;
    private static bool TransferFiles;
    
    static void Main(string[] args)
    {
    
      CollectCommandLineParameters(args);
      VerifyTargetPaths();

      // create the application context
      string ConnectionString = ConfigurationManager.ConnectionStrings["APPREPO"].ConnectionString;
      ApplicationContext AppContext = new ApplicationContext(ConnectionString);
      Bootstrap.Execute(AppContext);

      ProcessExcelFiles(AppContext);
      
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

            case "--transfer":
              Program.TransferFiles = true;
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

            case "--help":

              // only process help if it's the first position otherwise ignore
              if (argpos == 0)
              {
                Console.WriteLine("--help                     Display help menu.\r\n");
                Console.WriteLine("--transfer                 Directive to transfer files to archive or failed paths after processing.");
                Console.WriteLine("--file {path}              Individual file to process.");
                Console.WriteLine("--source {path}            Folder path to process.");
                Console.WriteLine("--archive {path}           Path to move files after successful processing.");
                Console.WriteLine("--failed {path}            Path to move files after failed processing.");
                Console.WriteLine("--archive-name {folder}    Leaf folder name appended to the source path to derive the archive path.");
                Console.WriteLine("--failed-name {folder}     Leaf folder name appended to the source path to derive the failed path.");
                if (Console.Out != null) Console.Out.Flush();
                System.Environment.Exit(1);
              }

              break;

            default:
              break;

          }
        }
        argpos++;
      }
    }

    private static void VerifyTargetPaths()
    {
      // ensure one source paramter is specfied
      if (SourceFolderPath == null && SourceFilePath == null)
      {
        throw new ApplicationException("Either a source folder or file must be specfified.");
      }

      // verify the source folder exists
      if (SourceFolderPath != null && !Directory.Exists(SourceFolderPath))
      {
        throw new ApplicationException("The source folder path does not exist.");
      }

      // verify the source file exists
      if (SourceFilePath != null && !File.Exists(SourceFilePath))
      {
        throw new ApplicationException("The source file does not exist.");
      }

      // only worry
      if (Program.TransferFiles)
      {

        // determine or create the archive folder
        if (Program.ArchiveFolderPath == null || Program.ArchiveFolderPath.Trim().Length == 0)
        {

          // verify the source folder path is provided
          if (SourceFilePath == null)
          {
            throw new ApplicationException("Unable to derive the archive folder path");
          }

          // derive the new folder path
          ArchiveFolderPath = SourceFolderPath + @"\" + ArchiveFolderName;

          // attempt to create the new folder
          if (!Directory.Exists(ArchiveFolderPath))
          {
            Directory.CreateDirectory(ArchiveFolderPath);
          }
        }

        // determine or create the failed folder
        if (!Directory.Exists(ArchiveFolderPath))
        {
          throw new ApplicationException("The archive folder path does not exist.");
        }

        // determine or create the failed folder
        if (Program.FailedFolderPath == null || Program.FailedFolderPath.Trim().Length == 0)
        {

          // verify the source folder path is provided
          if (SourceFilePath == null)
          {
            throw new ApplicationException("Unable to derive the archive folder path");
          }

          // derive the new folder path
          FailedFolderPath = SourceFolderPath + @"\" + FailedFolderName;

          // attempt to create the new folder
          if (!Directory.Exists(FailedFolderPath))
          {
            Directory.CreateDirectory(FailedFolderPath);
          }
        }

        // finall, verify the archive folder path
        if (ArchiveFolderPath.Equals(FailedFolderPath))
        {
          throw new ApplicationException("The archive and folder path must be different.");
        }

        // finall, verify the failed folder path
        if (!Directory.Exists(FailedFolderPath))
        {
          throw new ApplicationException("The failed folder path does not exist.");
        }
      }
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

      }
      catch (Exception ex)
      {
        EventLogger.Log(AppContext, "Error", "Application", "Error: " + ex.Message);
      }
      finally
      {
        EventLogger.Log(AppContext, "Quit", "Excel", "Quitting Excel application.");
        ExcelApp.Quit();
        Marshal.FinalReleaseComObject(ExcelApp);
      }

      
    }

    private static List<string> RetrieveExcelFilePaths()
    {

      List<string> ExcelFilePaths = null;

      // process the source folder if applicable
      if (Program.SourceFolderPath != null)
      {
        ExcelFilePaths = Directory.GetFiles(Program.SourceFolderPath, "*.xls*").ToList<string>();
      }
      else
      {
        ExcelFilePaths = new List<string>();
      }

      // add the source file if applicable
      if (SourceFilePath!= null && !ExcelFilePaths.Exists(x => x.Equals(SourceFilePath)))
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
        
        if (Program.TransferFiles)
        {

          string FileExtension = Path.GetExtension(ExcelFilePath);
          string FileNameRaw = Path.GetFileNameWithoutExtension(ExcelFilePath);
          string TargetFileName = FileNameRaw + "." + (Guid.NewGuid().ToString("D")) + FileExtension;

          // transfer files to proper target path
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


  }
}
