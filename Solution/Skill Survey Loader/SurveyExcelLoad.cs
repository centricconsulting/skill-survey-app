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

  public class InvalidRespondentException : Exception
  {
    public InvalidRespondentException(string Message) : base(Message) { }
  }


  public class SurveyExcelLoad
  {

    private ApplicationContext AppContext;
    private string ExcelFilePath = null;
    private string ExcelFileName = null;
    private ResourceSnapshot Respondent = null;

    public bool LoadSucceed { get; set; } = false;

    public SurveyExcelLoad(ApplicationContext AppContext, string ExcelFilePath)
    {

      this.AppContext = AppContext;
      this.ExcelFilePath = ExcelFilePath;
      this.ExcelFileName = System.IO.Path.GetFileName(ExcelFilePath);
      this.Respondent = new ResourceSnapshot();

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
        EventLogger.Log(this.AppContext, "File", this.ExcelFileName, "Loading Respondent information.");

        this.Respondent.EmailAddress = GetNameValue(wb, "Email");
        this.Respondent.ResourceUID = this.Respondent.EmailAddress;
        this.Respondent.ResourceLabel = GetNameValue(wb, "FullName");
        // use the processing date as the snapshot time
        this.Respondent.SnapshotTimestamp = DateTime.Now;

        // determine if the Respondent is valid
        if (this.Respondent.ResourceUID == null || this.Respondent.ResourceUID.Length < 1)
        {
          throw new InvalidRespondentException("The Respondent identifier is missing.");
        }

        // add the respondent i
        ResourceSnapshotRepository rrepo = new ResourceSnapshotRepository(this.AppContext);
        if(!rrepo.ResourceExists(this.Respondent.ResourceUID))
        {

          EventLogger.Log(this.AppContext, "File", this.ExcelFileName, string.Format("Adding resource {0}", this.Respondent.ResourceUID));

          AppContext.ResourceSnapshots.Add(new ResourceSnapshot()
          {
            ResourceUID = this.Respondent.ResourceUID,
            ResourceLabel = this.Respondent.ResourceLabel,
            SnapshotTimestamp = this.Respondent.SnapshotTimestamp,
            EmailAddress = this.Respondent.EmailAddress
          });
        }        

        // halt the process if the survey response already exists
        EventLogger.Log(this.AppContext, "File", this.ExcelFileName, "Verify the survey snapshot does not already exist.");

        SurveyResponseSnapshotRepository srsrepo = new SurveyResponseSnapshotRepository(this.AppContext);
        if (srsrepo.SnapshotExists(this.Respondent.ResourceUID, this.Respondent.SnapshotTimestamp))
        {

          EventLogger.Log(this.AppContext, "File", this.ExcelFileName, "Snapshot already exists. Exiting.");

          throw new SnapshotExistsException(
            string.Format("The snapshot for user \"{0}\" already exists in the system.", this.Respondent.ResourceUID));
        }


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

              case "Discipline":
              case "Method": // backward compatability
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

          //Marshal.FinalReleaseComObject(wb);

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
      string RespondentInfo = this.Respondent.GetRespondentInfo();

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
          if (int.TryParse(row.Range.Cells[1, ProficiencyColumn].Text, out ProficiencyValue))
          { 
            list.Add(new SurveyResponseSnapshot()
            {
              SkillUID = SkillUID,
              AspectUID = "PROFICIENCY",
              ResourceUID = this.Respondent.ResourceUID,
              SnapshotTimestamp = this.Respondent.SnapshotTimestamp,
              RatingValue = ProficiencyValue,
              RespondentInfo = RespondentInfo,
              OtherSkillInfo = (OtherSkillFlag) ? SkillName : null
            });
            ProcessedLineCount++;
          }
        }

        // gather interest if it exists in the table
        if (InterestColumn != null)
        {
          InterestValue = 0;          
          if (int.TryParse(row.Range.Cells[1, InterestColumn].Text, out InterestValue))
          {
            list.Add(new SurveyResponseSnapshot()
            {
              SkillUID = SkillUID,
              AspectUID = "INTEREST",
              ResourceUID = this.Respondent.ResourceUID,
              SnapshotTimestamp = this.Respondent.SnapshotTimestamp,
              RatingValue = InterestValue,
              RespondentInfo = RespondentInfo,
              OtherSkillInfo = (OtherSkillFlag) ? SkillName : null
            });
            ProcessedLineCount++;
          }
        }

        // gather admin if it exists in the table
        if (AdminColumn != null)
        {
          AdminValue = 0;          
          if (int.TryParse(row.Range.Cells[1, AdminColumn].Text, out AdminValue))
          {
            list.Add(new SurveyResponseSnapshot()
            {
              SkillUID = SkillUID,
              AspectUID = "ADMINISTRATION",
              ResourceUID = this.Respondent.ResourceUID,
              SnapshotTimestamp = this.Respondent.SnapshotTimestamp,
              RatingValue = AdminValue,
              RespondentInfo = RespondentInfo,
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

      EventLogger.Log(this.AppContext, "Respondent", this.Respondent.ResourceUID,
        string.Format("Processed {1} of {2} rows from {0} table.",
        table.Name, ProcessedLineCount.ToString(), LineCount.ToString()));

      Marshal.FinalReleaseComObject(table);

    }
  }
}
