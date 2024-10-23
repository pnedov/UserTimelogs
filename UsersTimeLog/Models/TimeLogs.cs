//using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace UsersTimeLog.Models;

[ExcludeFromCodeCoverage]

[Table("time_logs")]
public class Timelogs
{
    [Column("id", Order = 1)]
    [Key]
    public int Id { get; set; }

    [Column("date", Order = 2, TypeName = "date")]
    public DateTime Date { get; set; }

    [Column("hours", Order = 3, TypeName = "float")]
    public float Hours { get; set; }

    [Column("users_id", Order = 4, TypeName = "int")]
    public int UsersId { get; set; }

    [Column("projects_id", Order = 5, TypeName = "int")]
    public int ProjectsId { get; set; }

    public Projects Projects { get; set; } = new();
    public Users Users { get; set; } = new();
}







