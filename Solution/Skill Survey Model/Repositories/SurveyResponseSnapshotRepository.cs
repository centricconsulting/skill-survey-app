using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Centric.SkillSurvey.Models;

namespace Centric.SkillSurvey.Repositories
{
  public class SurveyResponseSnapshotRepository : BaseRepository<SurveyResponseSnapshot>
  {
    public SurveyResponseSnapshotRepository(ApplicationContext Context) : base(Context) { }

    public bool SnapshotExists(string ResourceUID, DateTime SnapshotTimestamp)
    {
      bool value = this.AppContext.SurveyResponseSnapshots
        .Any(x => x.SnapshotTimestamp == SnapshotTimestamp && x.ResourceUID.Equals(ResourceUID));

      return value;
    }

    public IQueryable<SurveyResponse> GetCurrentSurveyResponses()
    {

      IQueryable<SurveyResponse> list = 
        from rs in this.AppContext.SurveyResponseSnapshots
        group rs by new { rs.ResourceUID, rs.SkillUID, rs.AspectUID } into grp
        let MaxSnapshotTimestamp = grp.Max(g => g.SnapshotTimestamp)
        from p in grp
        where p.SnapshotTimestamp == MaxSnapshotTimestamp
        select new SurveyResponse() {
          ResourceUID = p.ResourceUID,
          SkillUID = p.SkillUID,
          AspectUID = p.AspectUID,
          SnapshotTimestamp = p.SnapshotTimestamp,
          RatingValue = p.RatingValue,
          RespondentInfo = p.RespondentInfo
        };

      return list;

    }
  }
}
