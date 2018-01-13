using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Centric.SkillSurvey.Repositories
{
  internal class ApplicationDbInitializer: DropCreateDatabaseIfModelChanges<ApplicationContext>
  {
    protected override void Seed(ApplicationContext context)
    {

      // create stored procedures here
      this.DropCreateViewResource(context);
      this.DropCreateViewSurveyResponse(context);

      base.Seed(context);

    }

    private void DropCreateViewResource(ApplicationContext context)
    {
      context.Database.ExecuteSqlCommand("IF OBJECT_ID('dbo.Resource','V') IS NOT NULL DROP VIEW dbo.Resource");
      context.Database.ExecuteSqlCommand(Properties.Resources.CreateViewResource);
    }

    private void DropCreateViewSurveyResponse(ApplicationContext context)
    {
      context.Database.ExecuteSqlCommand("IF OBJECT_ID('dbo.SurveyResponse','V') IS NOT NULL DROP VIEW dbo.SurveyResponse");
      context.Database.ExecuteSqlCommand(Properties.Resources.CreateViewSurveyResponse);
    }
  }
}
