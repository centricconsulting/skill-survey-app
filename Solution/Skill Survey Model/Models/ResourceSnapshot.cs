using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Centric.SkillSurvey.Models
{
  [Table("ResourceSnapshot")]
  public class ResourceSnapshot : BaseEntity
  {

    // constructors
    public ResourceSnapshot() { }

    [Key, Column(Order = 0)]
    public string ResourceUID { get; set; }

    [Key, Column(Order = 1)]
    public DateTime SnapshotTimestamp { get; set; }

    [MaxLength(200)]
    public string EmployerDesc { get; set; }

    [MaxLength(200)]
    public string FirstName { get; set; }

    [MaxLength(200)]
    public string LastName { get; set; }
    
    [MaxLength(200)]
    public string EmailAddress { get; set; }

    [Required, MaxLength(200)]
    public string ResourceLabel { get; set; }

    [Required]
    public Byte CentricEmployerFlag { get; set; }
    [NotMapped]
    public bool CentricEmployer
    {
      get
      {
        return this.CentricEmployerFlag.Equals(TRUE_FLAG_VALUE);
      }
      set
      {
        this.CentricEmployerFlag = value ? TRUE_FLAG_VALUE : FALSE_FLAG_VALUE;
      }
    }

    public string GetRespondentInfo()
    {
      return string.Format("{0} | {1}", this.ResourceLabel, this.EmailAddress);
    }
  }
}
