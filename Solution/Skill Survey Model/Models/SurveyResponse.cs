using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Centric.SkillSurvey.Models
{
  [NotMapped]
  public class SurveyResponse : BaseEntity
  {
    // constructors
    public SurveyResponse() { }
    
    public string ResourceUID { get; set; }
    public string SkillUID { get; set; }
    public string AspectUID { get; set; }
    public DateTime SnapshotTimestamp { get; set; }
    public int RatingValue { get; set; }
    public string RespondantInfo { get; set; }
  }
}
