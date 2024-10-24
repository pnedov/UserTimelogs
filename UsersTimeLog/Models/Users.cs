using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace UsersTimeLog.Models;

[ExcludeFromCodeCoverage]

[Table("users")]
[Index(nameof(FirstName), nameof(LastName), nameof(Email))]
public class Users
{
    [Column("id", Order = 1)]
    [Key]
    public int Id { get; set; }

    [Column("fname", Order = 2, TypeName = "nvarchar(64)")]
    public string FirstName { get; set; }

    [Column("lname", Order = 3, TypeName = "nvarchar(64)")]
    public string LastName { get; set; }

    [Column("email", Order = 4, TypeName = "nvarchar(128)")]
    public string Email { get; set; }

    [ForeignKey("UsersId")]
    public ICollection<Timelogs> Timelogs { get; set; }
}
