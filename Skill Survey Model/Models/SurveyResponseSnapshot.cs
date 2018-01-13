using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Centric.SkillSurvey.Models
{
  [Table("SurveyResponseSnapshot")]
  public class SurveyResponseSnapshot : BaseEntity
  {
    // constructors
    public SurveyResponseSnapshot() { }
    
    [Key, Column(Order = 0)]
    public string ResourceUID { get; set; }
    //[ForeignKey("ResourceUID")]
    //public virtual Resource Resource { get; set; }

    [Key, Column(Order = 1)]
    public string SkillUID { get; set; }
    //[NotMapped, ForeignKey("SkillUID")]
    //public virtual Skill Skill { get; set; }

    [Key, Column(Order = 2)]
    public string AspectUID { get; set; }
    //[NotMapped, ForeignKey("AspectUID")]
    //public virtual Aspect Aspect { get; set; }

    [Key, Column(Order = 3)]
    public DateTime SnapshotTimestamp { get; set; }

    [Required]
    public int RatingValue { get; set; }

  }
}
