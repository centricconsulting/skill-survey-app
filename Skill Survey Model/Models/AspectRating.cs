using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Centric.SkillSurvey.Models
{

  [Table("AspectRating")]
  public class AspectRating : BaseEntity
  {
    public AspectRating() { }

    [Key, Column(Order = 0)]
    [Index("dbo_Rating_U1", IsUnique = true, Order = 0)]
    public string AspectUID { get; set; }
    //[ForeignKey("AspectUID")]
    //public virtual Aspect Aspect{ get; set; }

    [Key, Column(Order = 1)]
    public int RatingValue { get; set; }

    [Required, MaxLength(200)]
    [Index("dbo_Rating_U1", IsUnique = true, Order = 1)]
    public string RatingName { get; set; }

    [Required, MaxLength(200)]
    public string RatingLabel { get; set; }

    [MaxLength(2000)]
    [Column("RatingDesc")]
    public string RatingDescription { get; set; }

  }
}
