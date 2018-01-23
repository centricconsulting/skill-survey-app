using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Centric.SkillSurvey.Models
{
  [Table("Skill")]
  public class Skill: BaseEntity
  {
    // constructors
    public Skill() { }

    [Key, Column(Order = 0)]
    public string SkillUID { get; set; }

    [Required, MaxLength(200)]
    [Index("dbo_SkillSurvey_U1", IsUnique = true, Order = 0)]
    public string SkillClassUID { get; set; }

    [Required, MaxLength(200)]
    [Index("dbo_SkillSurvey_U1", IsUnique = true, Order = 1)]
    public string SkillName { get; set; }

    [Required, MaxLength(200)]
    public string SkillLabel { get; set; }

    [Required, MaxLength(20)]
    [Index("dbo_SkillSurvey_U2", IsUnique = true, Order = 0)]
    public string SkillCode { get; set; }

    [MaxLength(2000)]
    [Column("SkillDescription")]
    public string SkillDescription { get; set; }

    [Required, MaxLength(200)]
    public string SkillCategoryName { get; set; }

    [Required, MaxLength(2000)]
    public String SkillTagList { get; set; }

    [Required]
    public Byte OtherFlag { get; set; }
    [NotMapped]
    public bool Other
    {
      get
      {
        return this.OtherFlag.Equals(TRUE_FLAG_VALUE);
      }
      set
      {
        this.OtherFlag = value ? TRUE_FLAG_VALUE : FALSE_FLAG_VALUE;
      }
    }

  }
}
