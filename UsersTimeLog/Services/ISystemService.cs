namespace UsersTimeLog.Services;

public interface ISystemService
{
    Task InitializeDatabaseAsync(CancellationToken token);
}

