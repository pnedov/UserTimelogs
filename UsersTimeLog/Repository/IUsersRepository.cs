using Microsoft.EntityFrameworkCore;
using UsersTimeLog.Models;

namespace UsersTimeLog.Repository;

public interface IUsersRepository
{
    public Task<(int TotalRecords, IList<Timelogs> Timelogs)> GetUsersAsync(DateTime? start, DateTime? end, int skip, int take, string? sortColumn, string? sortOrder, CancellationToken token);
    public Task<IList<Timelogs>> GetTopUsersAsync(DateTime? start, DateTime? end, int topCount, CancellationToken token);
    public Task<Timelogs?> GetUserByIdAsync(int userId, CancellationToken token);

}



