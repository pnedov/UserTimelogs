using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsersTimeLog.Models;

[ExcludeFromCodeCoverage]

[Table("projects")]
public class Projects
{
    [Column("id", Order = 1)]
    [Key]
    public int Id { get; set; }

    [Column("name", Order = 2, TypeName = "nvarchar(32)")]
    public string Name { get; set; }

    [ForeignKey("ProjectsId")]
    public ICollection<Timelogs> TimeLogs { get; set; }
}

