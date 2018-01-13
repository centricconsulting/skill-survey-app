using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Migrations;
using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using Centric.SkillSurvey.Models;

namespace Centric.SkillSurvey
{
 
  public class ApplicationContext : DbContext
  {
 
    public ApplicationContext(string ConnectionString) : base(ConnectionString)
    {
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      // prevent cascading deletes
      // foreign keys handled in model objects through annotations
      modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
      modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
      modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
      modelBuilder.Conventions.Remove<ForeignKeyDiscoveryConvention>();
      modelBuilder.HasDefaultSchema("dbo");
      base.OnModelCreating(modelBuilder);
      
    }

    public virtual DbSet<Skill> Skills { get; set; }
    public virtual DbSet<ResourceSnapshot> ResourceSnapshots { get; set; }
    public virtual DbSet<Aspect> Aspects { get; set; }
    public virtual DbSet<AspectRating> AspectRatings { get; set; }
    public virtual DbSet<SurveyResponseSnapshot> SurveyResponseSnapshots { get; set; }
    public virtual DbSet<EventLog> EventLogs { get; set; }


    public void DropCreateViews()
    {
      this.DropCreateViewResource();
      this.DropCreateViewSurveyResponse();
    }

    private void DropCreateViewResource()
    {
      this.Database.ExecuteSqlCommand("IF OBJECT_ID('dbo.Resource','V') IS NOT NULL DROP VIEW dbo.Resource");
      this.Database.ExecuteSqlCommand(Properties.Resources.CreateViewResource);
    }

    private void DropCreateViewSurveyResponse()
    {
      this.Database.ExecuteSqlCommand("IF OBJECT_ID('dbo.SurveyResponse','V') IS NOT NULL DROP VIEW dbo.SurveyResponse");
      this.Database.ExecuteSqlCommand(Properties.Resources.CreateViewSurveyResponse);
    }


  }
}