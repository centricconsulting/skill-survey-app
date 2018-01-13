using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Centric.SkillSurvey.Models;

namespace Centric.SkillSurvey.Repositories
{
  public class ResourceSnapshotRepository : BaseRepository<ResourceSnapshot>
  {
    public ResourceSnapshotRepository(ApplicationContext Context) : base(Context) { }

    public bool SnapshotExists(string ResourceUID, DateTime SnapshotTimestamp)
    {
      return this.AppContext.ResourceSnapshots
        .Any(x => x.SnapshotTimestamp.Equals(SnapshotTimestamp) && x.ResourceUID.Equals(ResourceUID));
    }

    public IQueryable<Resource> GetCurrentResources()
    {

      IQueryable<Resource> list = 
        from rs in this.AppContext.ResourceSnapshots
        group rs by rs.ResourceUID into grp
        let MaxSnapshotTimestamp = grp.Max(g => g.SnapshotTimestamp)
        from p in grp
        where p.SnapshotTimestamp == MaxSnapshotTimestamp
        select new Resource () {
          ResourceUID = p.ResourceUID,
          ResourceLabel = p.ResourceLabel,
          FirstName = p.FirstName,
          LastName = p.LastName,
          EmployerDesc = p.EmployerDesc,
          CentricEmployerFlag = p.CentricEmployerFlag,
          SnapshotTimestamp = p.SnapshotTimestamp,
          EmailAddress = p.EmailAddress
        };

      return list;

    }
  }
}
