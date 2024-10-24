using UsersTimeLog.Models;
using Microsoft.EntityFrameworkCore;
using UsersTimeLog.Utilities;

namespace UsersTimeLog.Repository;

public class UsersRepository : IUsersRepository
{
    private UsersTimeLogsDbContext _context;

    public UsersRepository(UsersTimeLogsDbContext context)
    {
        _context = context;
    }

    public async Task<(int TotalRecords, IList<Timelogs> Timelogs)> GetUsersAsync(DateTime? start, DateTime? end, int position, int take, string? sortColumn, string? sortOrder, CancellationToken token)
    {
        IQueryable<Timelogs> query = _context.Timelogs;

        //get total records number
        var totalRecords = await _context.Timelogs.CountAsync(token);

        //apply where clause
        query = query.Where(x => (!start.HasValue || x.Date >= start.Value.Date) && (!end.HasValue || x.Date <= end.Value.Date));

        query = query
            .AsNoTracking()
            .Include(x => x.Users)
            .Include(x => x.Projects)
            .Skip(position)
            .Take(take);

        // apply sorting
        if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
        {
            if (sortColumn == GlobalConstants.UsersSortColumn)
            {
                query = sortOrder.ToLower() switch
                {
                    "asc" => query.OrderBy(t => t.Users.FirstName).ThenBy(t => t.Users.LastName),
                    "desc" => query.OrderByDescending(t => t.Users.FirstName).ThenByDescending(t => t.Users.LastName),
                    _ => query
                };
            }
            else if (sortColumn == GlobalConstants.EmailsSortColumn)
            {
                query = sortOrder.ToLower() switch
                {
                    "asc" => query.OrderBy(t => t.Users.Email),
                    "desc" => query.OrderByDescending(t => t.Users.Email),
                    _ => query
                };
            }
            else if (sortColumn == GlobalConstants.ProjectsSortColumn)
            {
                query = sortOrder.ToLower() switch
                {
                    "asc" => query.OrderBy(t => t.Projects.Name),
                    "desc" => query.OrderByDescending(t => t.Projects.Name),
                    _ => query
                };
            }
            else if(sortColumn == GlobalConstants.DateSortColumn)
            {
                query = sortOrder.ToLower() switch
                {
                    "asc" => query.OrderBy(t => t.Date),
                    "desc" => query.OrderByDescending(t => t.Date),
                    _ => query
                };
            }
            else if (sortColumn == GlobalConstants.HoursSortColumn)
            {
                query = sortOrder.ToLower() switch
                {
                    "asc" => query.OrderBy(t => t.Hours),
                    "desc" => query.OrderByDescending(t => t.Hours),
                    _ => query
                };
            }
            else
            {
                query = sortOrder.ToLower() switch
                {
                    "asc" => query.OrderBy(t => t.Id),
                    "desc" => query.OrderByDescending(t => t.Id),
                    _ => query
                };
            }
        }

        var timelogs = await query.ToListAsync(token);

        return (totalRecords, timelogs);
    }

    public async Task<IList<Timelogs>> GetTopUsersAsync(DateTime? start, DateTime? end, int topCount, CancellationToken token)
    {
        var query = _context.Timelogs.Where(x => (!start.HasValue || x.Date >= start.Value.Date) && (!end.HasValue || x.Date <= end.Value.Date));

        return await query
            .AsNoTracking()
            .Include(x => x.Users)
            .Include(x => x.Projects)
            .OrderByDescending(x => x.Hours)
            .Take(topCount)
            .ToListAsync(token);
    }

    public async Task<Timelogs?> GetUserByIdAsync(int userId, CancellationToken token)
    {
        return await _context.Timelogs
            .SingleOrDefaultAsync(x => x.UsersId == userId, token);
    }
}

