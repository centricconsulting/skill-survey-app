using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Centric.SkillSurvey.Models;
using Centric.SkillSurvey.Repositories;
using System.Configuration;

namespace Centric.SkillSurvey
{
  class Program
  {
    static void Main(string[] args)
    {
      string ConnectionString = ConfigurationManager.ConnectionStrings["APPREPO"].ConnectionString;
      ApplicationContext AppContext = new ApplicationContext(ConnectionString);

      
      //Bootstrap.InitializeDatabase(AppContext);
      //AppContext.DropCreateViews();
      //Bootstrap.Execute(AppContext);

      string SurveyFilePath = @"C:\Users\jeff.kanel\Downloads\Survey Test.xlsm";
      SurveyLoader.LoadSurveyFromExcel(AppContext, SurveyFilePath);
      
    }


  }
}
