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
using Shell32;
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

    private static Shell32.Shell Shell;

    [STAThread]
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
            case "--source-file":
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

            case "--fail":
              Program.FailedFolderPath = args[argpos + 1].Replace("\"", "");
              break;

            case "--archive-name":
              Program.ArchiveFolderName = args[argpos + 1].Replace("\"", "");
              break;

            case "--fail-name":
              Program.FailedFolderName = args[argpos + 1].Replace("\"", "");
              break;

            case "--help":

              // only process help if it's the first position otherwise ignore
              if (argpos == 0)
              {
                Console.WriteLine("--help                     Display help menu.\r\n");
                Console.WriteLine("--transfer                 Directive to transfer files to archive or failed paths after processing.");
                Console.WriteLine("--source-file {path}       Individual file to process.");
                Console.WriteLine("--source {path}            Folder path to process.");
                Console.WriteLine("--archive {path}           Path to move files after successful processing.");
                Console.WriteLine("--fail {path}              Path to move files after failed processing.");
                Console.WriteLine("--archive-name {folder}    Leaf folder name appended to the source path to derive the archive path.");
                Console.WriteLine("--fail-name {folder}       Leaf folder name appended to the source path to derive the failed path.");
                if (Console.Out != null) Console.Out.Flush();
                System.Environment.Exit(1);
              }

              return;

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
          if (SourceFolderPath == null)
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
          if (SourceFolderPath == null)
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

      Program.Shell = new Shell32.Shell();
      Excel.Application ExcelApp = null;

      try
      {
        EventLogger.Log(AppContext, "Start", "Excel", "Start Excel application");

#pragma warning disable IDE0017 // Simplify object initialization
        ExcelApp = new Excel.Application();
#pragma warning restore IDE0017 // Simplify object initialization

        ExcelApp.Visible = false;

        // loop through relevant files
        // sort by write date
        foreach (var FileInfo in RetrieveExcelFileInfoList().OrderBy(x => x.Value))
        {
          ProcessExcelFile(ExcelApp, AppContext, FileInfo.Key);
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

    private static Dictionary<string, DateTime> RetrieveExcelFileInfoList()
    {

      Dictionary<string, DateTime> FileInfoList = new Dictionary<string, DateTime>();

      // process the source folder if applicable
      if (Program.SourceFolderPath != null)
      {
        string[] Files = Directory.GetFiles(Program.SourceFolderPath, "*.xls*").ToArray<string>();

        foreach (string file in Files)
        {
          FileInfoList.Add(file, Program.GetLastSavedByDate(file));
        }
      }

      // add the source file if applicable
      if (SourceFilePath != null && !FileInfoList.Keys.Contains(SourceFilePath))
      {
        FileInfoList.Add(SourceFilePath, Directory.GetLastWriteTime(SourceFilePath));
      }

      return FileInfoList;

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

          // transfer files to proper target path
          if (loader.LoadSucceed)
          {
            // move to archive folder
            EventLogger.Log(AppContext, "File", ExcelFilePath, "Moving to archive folder.");

            // if the file exists, create a new version
            string TargetFilePath = Path.Combine(Program.ArchiveFolderPath, Path.GetFileName(ExcelFilePath));
            if (File.Exists(TargetFilePath))
            {
              TargetFilePath = Path.Combine(Program.ArchiveFolderPath,
                Path.GetFileNameWithoutExtension(ExcelFilePath) + "_" +
                DateTime.Now.ToString("yyyyMMddHHmmssffffff") + '.' +
                Path.GetExtension(ExcelFilePath));
            }

            Directory.Move(ExcelFilePath, TargetFilePath);
          }
          else
          {
            // move to failed folder
            EventLogger.Log(AppContext, "File", ExcelFilePath, "Moving to failed folder.");


            // if the file exists, create a new version
            string TargetFilePath = Path.Combine(Program.FailedFolderPath, Path.GetFileName(ExcelFilePath));
            if (File.Exists(TargetFilePath))
            {
              TargetFilePath = Path.Combine(Program.FailedFolderPath,
                Path.GetFileNameWithoutExtension(ExcelFilePath) + "_" +
                DateTime.Now.ToString("yyyyMMddHHmmssffffff") + '.' +
                Path.GetExtension(ExcelFilePath));
            }

            Directory.Move(ExcelFilePath, TargetFilePath);
          }
        }

        EventLogger.Log(AppContext, "Finished", ExcelFilePath, "Load is complete.");
      }
      catch (Exception ex)
      {
        EventLogger.Log(AppContext, "Error", ExcelFilePath, "Error: " + ex.Message);
      }
    }

    private static DateTime GetLastSavedByDate(string FilePath)
    {

      //https://blog.dotnetframework.org/2014/12/10/read-extended-properties-of-a-file-in-c/
      //https://stackoverflow.com/questions/10950477/how-can-i-optimize-shell32-method-calls

      List<string> AttributeList = new List<string>();

      Folder Folder = Program.Shell.NameSpace(Path.GetDirectoryName(FilePath));
      string FileName = Path.GetFileName(FilePath);

      // find the correct file index
      for (int i = 0; i < Folder.Items().Count - 1; i++)
      {

        FolderItem FolderItem = Folder.Items().Item(i);
        if (FolderItem.Name.Equals(FileName))
        {

          for (int ia = 0; ia < short.MaxValue; ia++)
          {
            string header = Folder.GetDetailsOf(null, ia);

            if (header.Equals("Date last saved"))
            {
              string value = Folder.GetDetailsOf(FolderItem, ia);
              string DateValue = string.Empty;
              foreach(Char c in value.ToCharArray())
              {
                if ((int)c < 255) DateValue = DateValue + c;
              }

              return DateTime.Parse(DateValue);
            }
          }
        }
      }

      // default is the last access time of the file
      return Directory.GetLastAccessTime(FilePath);
    }
      
  }
}
