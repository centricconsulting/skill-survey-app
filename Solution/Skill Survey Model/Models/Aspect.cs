using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Centric.SkillSurvey.Models
{

  [Table("Aspect")]
  public class Aspect : BaseEntity
  {
    public Aspect() { }

    [Key, Column(Order = 0)]
    public string AspectUID { get; set; }

    [Required, MaxLength(200)]
    [Index("dbo_Aspect_U1", IsUnique = true, Order = 0)]
    public string AspectName { get; set; }

    [Required, MaxLength(200)]
    [Index("dbo_Aspect_U2", IsUnique = true, Order = 0)]
    public string AspectCode { get; set; }

    [Required, MaxLength(200)]
    public string AspectLabel { get; set; }

    [MaxLength(2000)]
    [Column("AspectDesc")]
    public string AspectDescription { get; set; }

  }
}
