using UsersTimeLog.Models;
using UsersTimeLog.Repository;

namespace UsersTimeLog.Services;

public class UsersService : IUsersService
{
    private IUsersRepository _repo;

    public UsersService(IUsersRepository repo)
    {
        _repo = repo;
    }

    public async Task<(int TotalRecords, IList<Timelogs> Timelogs)> GetUsersAsync(string start, string end, int position, int take, string sortBy, string sortOrder, CancellationToken token)
    {
        DateTime? startDate = string.IsNullOrEmpty(start) ? null : DateTime.Parse(start);
        DateTime? endDate = string.IsNullOrEmpty(end) ? null : DateTime.Parse(end);
        var result = await _repo.GetUsersAsync(startDate, endDate, position, take, sortBy, sortOrder, token);

        return result;
    }

    public async Task<IList<Timelogs>> GetTopUsersAsync(string start, string end, int topCount, CancellationToken token)
    {
        DateTime? startDate = string.IsNullOrEmpty(start) ? null : DateTime.Parse(start);
        DateTime? endDate = string.IsNullOrEmpty(end) ? null : DateTime.Parse(end);

        return await _repo.GetTopUsersAsync(startDate, endDate, topCount, token);
    }

    public async Task<Timelogs?> GetUserByIdAsync(int userId, CancellationToken token)
    {
        return await _repo.GetUserByIdAsync(userId, token);
    }
}

