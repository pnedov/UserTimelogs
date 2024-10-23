using UsersTimeLog.Models;

namespace UsersTimeLog.Services;

public interface IUsersService
{
    public Task<(int TotalRecords, IList<Timelogs> Timelogs)> GetUsersAsync(string start, string end, int position, int take, string sortBy, string sortOrder, CancellationToken token);
    public Task<IList<Timelogs>> GetTopUsersAsync(string start, string end, int topCount, CancellationToken token);
    public Task<Timelogs?> GetUserByIdAsync(int userId, CancellationToken token);

}

