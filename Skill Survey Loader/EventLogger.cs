using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Centric.SkillSurvey.Models;
using Centric.SkillSurvey.Repositories;

namespace Centric.SkillSurvey
{
  public static class EventLogger
  {
    public static void Log(ApplicationContext AppContext, string EventType, string EventContext, string EventMessage)
    {
      new EventLogRepository(AppContext).Insert(new EventLog()
      {
        EventType = EventType,
        EventContext = EventContext,
        EventMessage = EventMessage     
      });
    }
  }
}
