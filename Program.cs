using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MVCMovie.Data;
using MVCMovie.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MVCMovieContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MVCMovieContext") ?? throw new InvalidOperationException("Connection string 'MVCMovieContext' not found."),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, // Maximum number of retries on transient failures
                maxRetryDelay: TimeSpan.FromSeconds(30), // Maximum delay between retries
                errorNumbersToAdd: null); // SQL error numbers to consider transient (or you can leave this null)
        }));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
