using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ServerMCP.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer()
    .WithToolsFromAssembly(Assembly.GetExecutingAssembly()) 
    .WithHttpTransport();

var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlite(connStr));

var app = builder.Build();

// MCP HTTP endpoints
app.MapMcp();

// Apply EF Core migrations on startup (ensure target DB is writeable)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
