namespace UsersTimeLog.Models;

public class CombinedModel
{
	public IList<Timelogs> UsersTimelogs { get; set; } = [];

	public IList<Timelogs> TopUsersTimelogs { get; set; } = [];

	public int TotalPagescount { get; set; }

	public int PageNumber { get; set; }
}

