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
  public class Resource : BaseEntity
  {

    public string ResourceUID { get; set; }
    public DateTime SnapshotTimestamp { get; set; }
    public string EmployerDesc { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string ResourceLabel { get; set; }
    public Byte CentricEmployerFlag { get; set; }

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
  }
}
