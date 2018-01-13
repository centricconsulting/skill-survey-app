using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Centric.SkillSurvey.Models;

namespace Centric.SkillSurvey.Repositories
{
  public class AspectRepository : BaseRepository<Aspect>
  {
    public AspectRepository(ApplicationContext Context) : base(Context) { }   
  }

  public class AspectRatingRepository : BaseRepository<AspectRating>
  {
    public AspectRatingRepository(ApplicationContext Context) : base(Context) { }
  }

  public class SkillRepository : BaseRepository<Skill>
  {
    public SkillRepository(ApplicationContext Context) : base(Context) { }
  }

  public class EventLogRepository : BaseRepository<EventLog>
  {
    public EventLogRepository(ApplicationContext Context) : base(Context) { }
  }

}
