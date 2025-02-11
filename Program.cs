using Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Authentication
builder.Services.AddAuthentication()
                .AddBearerToken(IdentityConstants.BearerScheme);

// Register MailKit Email Sender
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Configure Authorization
builder.Services.AddAuthorizationBuilder();

// Configure Database Connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

var app = builder.Build();

// Map Identity API Endpoints
app.MapIdentityApi<IdentityUser>();

// Minimal API for User Greeting
app.Map("/", (ClaimsPrincipal user) => $"Hello {user.Identity!.Name} from minimal APIs.")
    .RequireAuthorization();

// Enable Swagger for API documentation
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
