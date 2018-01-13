using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Centric.SkillSurvey.Models
{

  [Table("EventLog")]
  public class EventLog
  {
    public EventLog()
    {
      this.EventTimestamp = DateTime.Now;
    }

    [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EventKey { get; set; }

    [Required]
    public DateTime EventTimestamp { get; set; }

    [Required, MaxLength(20)]
    public string EventType { get; set; }

    [Required, MaxLength(200)]
    public string EventContext { get; set; }

    [Required, MaxLength(2000)]
    public string EventMessage { get; set; }
    
  }
}
