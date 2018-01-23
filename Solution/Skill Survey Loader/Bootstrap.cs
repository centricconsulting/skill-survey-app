using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Centric.SkillSurvey.Models;
using Centric.SkillSurvey.Repositories;
using Microsoft.VisualBasic.FileIO;

namespace Centric.SkillSurvey
{
  public static class Bootstrap
  {

     public static void Execute(ApplicationContext AppContext) 
    {

      if (AppContext.Aspects.Count() == 0) CreateAspects(AppContext);
      if (AppContext.AspectRatings.Count() == 0) CreateAspectRatings(AppContext);
      if (AppContext.Skills.Count() == 0) CreateSkills(AppContext);
     
    }

    private static void CreateAspects(ApplicationContext AppContext)
    {

      AspectRepository repo = new AspectRepository(AppContext);

      {
        List<Aspect> list = GetAspectList();
        repo.InsertAll(list);
      }
    }

    private static void CreateAspectRatings(ApplicationContext AppContext)
    {


      AspectRatingRepository repo = new AspectRatingRepository(AppContext);
      {
        List<AspectRating> list = GetAspectRatingList();
        repo.InsertAll(list);
      }
    }

    private static void CreateSkills(ApplicationContext AppContext)
    {

      SkillRepository repo = new SkillRepository(AppContext);
      {
        List<Skill> list = GetSkillList();
        repo.InsertAll(list);
      }
    }

    private static List<Aspect> GetAspectList()
    {

      string FilePath = @"C:\Development\Solutions\GitHub\skill-survey-app\Solution\Skill Survey Loader\Data\Aspects.txt";
      // File Columns: AspectUID, AspectCode, AspectName, AspectLabel, AspectDesc

      List<Aspect> list = new List<Aspect>();
      string[] fields = null;
      int LineCount = 0;
      
      using (TextFieldParser parser = new TextFieldParser(FilePath))
      {

        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters("|");
        parser.HasFieldsEnclosedInQuotes = true;

        while (!parser.EndOfData)
        {
          LineCount++;

          fields = parser.ReadFields();
          //if (fields == null || fields[0].Trim().Length == 0) break;

          if (LineCount > 1)
          {
            list.Add(new Aspect()
            {
              AspectUID = fields[0],
              AspectCode = fields[1],
              AspectName = fields[2],
              AspectLabel = fields[3],
              AspectDescription = fields[4]
            });
          }
        }
      }

      return list;
    }

    private static List<AspectRating> GetAspectRatingList()
    {

      string FilePath = @"C:\Development\Solutions\GitHub\skill-survey-app\Solution\Skill Survey Loader\Data\AspectRatings.txt";
      // File Columns: AspectUID, RatingValue, RatingName, RatingLabel, RatingDesc

      List<AspectRating> list = new List<AspectRating>();
      string[] fields = null;
      int LineCount = 0;


      using (TextFieldParser parser = new TextFieldParser(FilePath))
      {

        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters("|");
        parser.HasFieldsEnclosedInQuotes = true;

        while (!parser.EndOfData)
        {
          LineCount++;
          fields = parser.ReadFields();
          //if (fields == null || fields[0].Trim().Length == 0) break;

          if (LineCount > 1)
          {          
            list.Add(new AspectRating()
            {
              AspectUID = fields[0],
              RatingValue = int.Parse(fields[1]),
              ScaledRatingValue = int.Parse(fields[2]),
              RatingName = fields[3],
              RatingLabel = fields[4],
              RatingDescription = fields[5]
            });
          }
        }
      }

      return list;
    }


    private static List<Skill> GetSkillList()
    {

      string FilePath = @"C:\Development\Solutions\GitHub\skill-survey-app\Solution\Skill Survey Loader\Data\Skills.txt";
      // File Columns: SkillUID, SkillCode, SkillClassUID, SkillName, SkillCategoryName, SkillLabel, SkillDesc

      List<Skill> list = new List<Skill>();
      string[] fields = null;
      int LineCount = 0;

      using (TextFieldParser parser = new TextFieldParser(FilePath))
      {

        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters("|");
        parser.HasFieldsEnclosedInQuotes = true;

        while (!parser.EndOfData)
        {
          LineCount++;
          fields = parser.ReadFields();
          //if (fields == null || fields[0].Trim().Length == 0) break;

          if (LineCount > 1)
          {
            list.Add(new Skill()
            {
              SkillUID = fields[0],
              SkillCode = fields[1],
              SkillClassUID = fields[2],
              SkillName = fields[3],
              SkillCategoryName = fields[4],
              SkillLabel = fields[5],
              SkillDescription = fields[6],
              SkillTagList = fields[7],
              OtherFlag = byte.Parse(fields[8])
            });
          }
        }
      }

      return list;
    }
    
  }
}
