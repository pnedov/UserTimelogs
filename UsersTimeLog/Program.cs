//using UsersTimeLog.ApiClients;
//using UsersTimeLog.App_Code;
using UsersTimeLog.Repository;
using UsersTimeLog.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
//builder.Services.AddSwaggerGen();

// configure DI for application services
builder.Services.AddScoped<ISystemService, SystemService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

var configuration = builder.Configuration;
builder.Services.AddScoped<UsersTimeLogsDbContext>();

var services = builder.Services;
var app = builder.Build();


//builder.Services.AddControllers();
//builder.Services.AddSwaggerGen();

//var configuration = builder.Configuration;
//builder.Services.AddScoped<UsersTimeLogsDbContext>();

//var services = builder.Services;
//var app = builder.Build();
var scope = app.Services.CreateScope();
var db_context = scope.ServiceProvider.GetService<UsersTimeLogsDbContext>();
db_context.Database.EnsureCreated();

app.UseRouting();
app.UseHttpsRedirection();
app.MapControllers();

//app.Run("http://localhost:3000");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Set the Swagger UI at the root URL
//app.UseSwagger();
//app.UseSwaggerUI(options =>
//{
//    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
//    options.RoutePrefix = string.Empty;
//});

app.Run();

