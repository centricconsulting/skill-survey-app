using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using Centric.SkillSurvey.Models;
using Centric.SkillSurvey.Repositories;

namespace Centric.SkillSurvey
{

  public class SnapshotExistsException : Exception
  {
    public SnapshotExistsException(string Message) : base(Message) { }
  }

  public class InvalidRespondantException : Exception
  {
    public InvalidRespondantException(string Message) : base(Message) { }
  }


  public class SurveyExcelLoad
  {

    private ApplicationContext AppContext;
    private string ExcelFilePath = null;
    private string ExcelFileName = null;
    private ResourceSnapshot Respondant = null;

    public bool LoadSucceed { get; set; } = false;

    public SurveyExcelLoad(ApplicationContext AppContext, string ExcelFilePath)
    {

      this.AppContext = AppContext;
      this.ExcelFilePath = ExcelFilePath;
      this.ExcelFileName = System.IO.Path.GetFileName(ExcelFilePath);
      this.Respondant = new ResourceSnapshot();

    }

    public void Load(Excel.Application ExcelApp)
    {

      Excel.Workbook wb = null;

      try
      {
        // open the workbook
        EventLogger.Log(this.AppContext, "File", this.ExcelFileName, "Opening workbook.");

        // turn off events to prevent modal form from opening
        if (ExcelApp.EnableEvents) ExcelApp.EnableEvents = false;
        wb = ExcelApp.Workbooks.Open(ExcelFilePath);


        // populate the resource snapshot info
        EventLogger.Log(this.AppContext, "File", this.ExcelFileName, "Loading respondant information.");

        this.Respondant.EmailAddress = GetNameValue(wb, "Email");
        this.Respondant.ResourceUID = this.Respondant.EmailAddress;
        this.Respondant.ResourceLabel = GetNameValue(wb, "FullName");
        this.Respondant.SnapshotTimestamp = GetFileModifyTimestamp(ExcelFilePath);

        // determine if the respondant is valid
        if (this.Respondant.ResourceUID == null || this.Respondant.ResourceUID.Length < 1)
        {
          throw new InvalidRespondantException("The respondant identifier is missing.");
        }

        // halt the process if the survey response already exists
        EventLogger.Log(this.AppContext, "File", this.ExcelFileName, "Verify the survey snapshot does not already exist.");

        SurveyResponseSnapshotRepository srsrepo = new SurveyResponseSnapshotRepository(this.AppContext);
        if (srsrepo.SnapshotExists(this.Respondant.ResourceUID, this.Respondant.SnapshotTimestamp))
        {

          EventLogger.Log(this.AppContext, "File", this.ExcelFileName, "Snapshot already exists. Exiting.");

          throw new SnapshotExistsException(
            string.Format("The snapshot for user \"{0}\" already exists in the system.", this.Respondant.ResourceUID));
        }

        EventLogger.Log(this.AppContext, "File", this.ExcelFileName, "Looping through worksheets");

        // loop through the workbooks
        foreach (Excel.Worksheet ws in wb.Worksheets)
        {
          if (ws.ListObjects.Count > 0)
          {
            switch (ws.Name)
            {

              case "Industry":
                LoadSurveyResponses(ws.ListObjects["Industries"]);
                break;

              case "Subject Area":
                LoadSurveyResponses(ws.ListObjects["SubjectAreas"]);
                break;

              case "Method":
                LoadSurveyResponses(ws.ListObjects["Methods"]);
                break;

              case "Language":
                LoadSurveyResponses(ws.ListObjects["Languages"]);
                break;

              case "Technology":
                LoadSurveyResponses(ws.ListObjects["Technologies"]);
                break;

              default:
                break;

            }
          }

          Marshal.FinalReleaseComObject(wb);

        }

        this.LoadSucceed = true;


      }
      catch (Exception ex)
      {
        EventLogger.Log(this.AppContext, "Error", this.ExcelFileName, "Error: " + ex.Message);
      }
      finally
      {
        EventLogger.Log(this.AppContext, "File", this.ExcelFileName, "Closing the Excel workbook.");
        wb.Close();
        Marshal.FinalReleaseComObject(wb);
      }

    }

    private string GetNameValue(Excel.Workbook wb, string Name)
    {

      Excel.Name n1 = null;

      try
      {
        n1 = wb.Names.Item(Name);
        string value = n1.RefersToRange.Value2.ToString().Trim();

        return value;

      }
      catch
      {
        return null;
      }
      finally
      {
        Marshal.FinalReleaseComObject(n1);
      }
    }

    private DateTime GetFileModifyTimestamp(string FilePath)
    {
      DateTime value = System.IO.File.GetLastWriteTime(FilePath);

      // truncate the milliseconds
      return new DateTime(
        value.Ticks - (value.Ticks % TimeSpan.TicksPerSecond), value.Kind);
    }

    private void LoadSurveyResponses(Excel.ListObject table)
    {

      // force the skill column index to 1 (required)
      int SkillColumn = 1;
      int SkillNameColumn = 3;

      // determine the optional column indexes
      int? ProficiencyColumn = null;
      int? InterestColumn = null;
      int? AdminColumn = null;

      // identify the rating value columns present in the table
      foreach(Excel.ListColumn col in table.ListColumns)
      {
        switch(col.Name)
        {
          case "P#":
            ProficiencyColumn = col.Index;
            break;

          case "V#":
            InterestColumn = col.Index;
            break;

          case "A#":
            AdminColumn = col.Index;
            break;

          default:
            break;
        }

        Marshal.FinalReleaseComObject(col);
      }
      
      string SkillUID = null;
      string SkillName = null;
      int ProficiencyValue = 0;
      int InterestValue = 0;
      int AdminValue = 0;
      int LineCount = 0;
      int ProcessedLineCount = 0;
      string RespondantInfo = this.Respondant.GetRespondantInfo();

      bool OtherSkillFlag = false;

      List<SurveyResponseSnapshot> list = new List<SurveyResponseSnapshot>();

      foreach (Excel.ListRow row in table.ListRows)
      {

        LineCount++;
        SkillName = null;

        // get the skillUID first (required)
        SkillUID = row.Range.Cells[1, SkillColumn].Text;

        // identify other skills
        OtherSkillFlag = (SkillUID.Substring(2, 1) == "X");

        if (OtherSkillFlag)
        {
          SkillName = row.Range.Cells[1, SkillNameColumn].Text;
        }

        // gather proficiency if it exists in the table
        if (ProficiencyColumn != null)
        {
          ProficiencyValue = 0;
          int.TryParse(row.Range.Cells[1, ProficiencyColumn].Text, out ProficiencyValue);

          if (ProficiencyValue > 0)
          {
            list.Add(new SurveyResponseSnapshot()
            {
              SkillUID = SkillUID,
              AspectUID = "PROFICIENCY",
              ResourceUID = this.Respondant.ResourceUID,
              SnapshotTimestamp = this.Respondant.SnapshotTimestamp,
              RatingValue = ProficiencyValue,
              RespondantInfo = RespondantInfo,
              OtherSkillInfo = (OtherSkillFlag) ? SkillName : null
            });
            ProcessedLineCount++;
          }
        }

        // gather interest if it exists in the table
        if (InterestColumn != null)
        {
          InterestValue = 0;
          int.TryParse(row.Range.Cells[1, InterestColumn].Text, out InterestValue);

          if (InterestValue > 0)
          {
            list.Add(new SurveyResponseSnapshot()
            {
              SkillUID = SkillUID,
              AspectUID = "INTEREST",
              ResourceUID = this.Respondant.ResourceUID,
              SnapshotTimestamp = this.Respondant.SnapshotTimestamp,
              RatingValue = InterestValue,
              RespondantInfo = RespondantInfo,
              OtherSkillInfo = (OtherSkillFlag) ? SkillName : null
            });
            ProcessedLineCount++;
          }
        }

        // gather admin if it exists in the table
        if (AdminColumn != null)
        {
          AdminValue = 0;
          int.TryParse(row.Range.Cells[1, ProficiencyColumn].Text, out AdminValue);

          if (AdminValue > 0)
          {
            list.Add(new SurveyResponseSnapshot()
            {
              SkillUID = SkillUID,
              AspectUID = "ADMINISTRATION",
              ResourceUID = this.Respondant.ResourceUID,
              SnapshotTimestamp = this.Respondant.SnapshotTimestamp,
              RatingValue = AdminValue,
              RespondantInfo = RespondantInfo,
              OtherSkillInfo = (OtherSkillFlag) ? SkillName : null
            });
            ProcessedLineCount++;
          }
        }

        Marshal.FinalReleaseComObject(row);
      }

      if (list.Count > 0)
      {
        new SurveyResponseSnapshotRepository(this.AppContext).InsertAll(list);
      }

      EventLogger.Log(this.AppContext, "Respondant", this.Respondant.ResourceUID,
        string.Format("Processed {1} of {2} rows from {0} table.",
        table.Name, ProcessedLineCount.ToString(), LineCount.ToString()));

      Marshal.FinalReleaseComObject(table);

    }
  }
}
