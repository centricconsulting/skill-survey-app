using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Centric.SkillSurvey.Models
{
  /// <summary>
  /// Provides attributes common to all objects in the model as well as helper functions.
  /// </summary>
  /// 
  public abstract class BaseEntity
  {

    public BaseEntity()
    { 
      this.UpdateCreateInfo();
    }

    public static System.Byte TRUE_FLAG_VALUE = 1;
    public static System.Byte FALSE_FLAG_VALUE = 0;

    public void UpdateCreateInfo()
    {

      this.CreateTimestamp = DateTime.Now;

      // assume that create and modify are always the same
      // ensure that modify and create timestamps are identical
      this.ModifyTimestamp = this.CreateTimestamp;
    }

    public void UpdateModifyInfo()
    {
      this.ModifyTimestamp = DateTime.Now;
    }

    [Required]
    public DateTime CreateTimestamp { get; set; }

    [Required]
    public DateTime ModifyTimestamp { get; set; }

   
  }
}
