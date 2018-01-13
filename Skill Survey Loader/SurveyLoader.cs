using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using Centric.SkillSurvey.Models;
using Centric.SkillSurvey.Repositories;

namespace Centric.SkillSurvey
{
 
  public class SnapshotExistsException : Exception
  {
    public SnapshotExistsException(string Message) : base(Message) { }
  }

  public static class SurveyLoader
  {
    public static void LoadSurveyFromExcel(ApplicationContext AppContext, string ExcelFilePath)
    {

      Excel.Application ExcelApp = null;
      Excel.Workbook wb = null;

      try
      {

        ExcelApp = new Excel.Application();
        ExcelApp.Visible = false;      
        wb = ExcelApp.Workbooks.Open(ExcelFilePath);

        // populate the resource snapshot info
        ResourceSnapshot respondant = new ResourceSnapshot()
        {
          ResourceUID = GetNameValue(wb, "Email"),
          SnapshotTimestamp = GetFileModifyTimestamp(ExcelFilePath)
        };

        // halt the process if the survey response already exists
        SurveyResponseSnapshotRepository srsrepo = new SurveyResponseSnapshotRepository(AppContext);
          if (srsrepo.SnapshotExists(respondant.ResourceUID, respondant.SnapshotTimestamp))
        {
          throw new SnapshotExistsException(
            string.Format("The snapshot for user \"{0}\" already exists in the system.", respondant.ResourceUID));
        }

        // loop through the workbooks
        foreach (Excel.Worksheet ws in wb.Worksheets)
        {
          if (ws.ListObjects.Count > 0)
          {
            switch (ws.Name)
            {

              case "Industry":
                LoadSurveyResponses(ws.ListObjects["Industries"], respondant, AppContext);
                break;

              case "Subject Area":
                LoadSurveyResponses(ws.ListObjects["SubjectAreas"], respondant, AppContext);
                break;

              case "Method":
                LoadSurveyResponses(ws.ListObjects["Methods"], respondant, AppContext);
                break;

              case "Language":
                LoadSurveyResponses(ws.ListObjects["Languages"], respondant, AppContext);
                break;

              case "Technology":
                LoadSurveyResponses(ws.ListObjects["Technologies"], respondant, AppContext);
                break;

              default:
                break;

            }
          }
        }
      }
      catch(Exception ex)
      {
        wb.Close();
        ExcelApp.Quit();

        throw ex;
      }
      
    }

    private static string GetNameValue(Excel.Workbook wb, string Name)
    {
      Excel.Name n1 = wb.Names.Item(Name);

      try
      { 
        return n1.RefersToRange.Value2.ToString();
      }
      catch
      {
        return null;
      }    
    }

    private static DateTime GetFileModifyTimestamp(string FilePath)
    {
      DateTime value = System.IO.File.GetLastWriteTime(FilePath);

      // truncate the milliseconds
      return new DateTime(
        value.Ticks - (value.Ticks % TimeSpan.TicksPerSecond), value.Kind);


    }

    private static void LoadSurveyResponses(Excel.ListObject table, ResourceSnapshot respondant, ApplicationContext AppContext)
    {
 
      // determine the skill column index (required)
      int SkillColumn = table.ListColumns["Skill#"].Index;

      // determine the optional column indexes
      int? ProficiencyColumn = null;
      int? InterestColumn = null;
      int? AdminColumn = null;

      try { ProficiencyColumn = table.ListColumns["P#"].Index; } catch { }
      try { InterestColumn = table.ListColumns["V#"].Index; } catch { }
      try { AdminColumn = table.ListColumns["A#"].Index; } catch { }

      string SkillUID = null;
      int ProficiencyValue = 0;
      int InterestValue = 0;
      int AdminValue = 0;

      List<SurveyResponseSnapshot> list = new List<SurveyResponseSnapshot>();

      foreach (Excel.ListRow row in table.ListRows)
      {
        // get the skillUID first (required)
        SkillUID = row.Range.Cells[1, SkillColumn].Text;

        // gather proficiency if it exists in the table
        if(ProficiencyColumn != null)
        {
          ProficiencyValue = 0;
          int.TryParse(row.Range.Cells[1, ProficiencyColumn].Text, out ProficiencyValue);

          if (ProficiencyValue > 0)
          {
            list.Add(new SurveyResponseSnapshot()
            {
              SkillUID = SkillUID,
              AspectUID = "PROFICIENCY",
              ResourceUID = respondant.ResourceUID,
              SnapshotTimestamp = respondant.SnapshotTimestamp,
              RatingValue = ProficiencyValue

            });
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
              ResourceUID = respondant.ResourceUID,
              SnapshotTimestamp = respondant.SnapshotTimestamp,
              RatingValue = InterestValue

            });
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
              ResourceUID = respondant.ResourceUID,
              SnapshotTimestamp = respondant.SnapshotTimestamp,
              RatingValue = AdminValue

            });
          }
        }        
      }

      if(list.Count > 0)
      {
        new SurveyResponseSnapshotRepository(AppContext).InsertAll(list);
      }
    }
  }  
}
