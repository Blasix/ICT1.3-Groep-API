using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOwnsResource", policy =>
        policy.RequireAssertion(context =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var resourceUserId = context.Resource as string; // Assuming resourceUserId is passed as a string

            return userId == resourceUserId;
        }));
});

string connStr = builder.Configuration.GetValue<string>("ConnectionString");
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 10;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;

}).AddRoles<IdentityRole>()
    .AddDapperStores(options =>
    {
        options.ConnectionString = connStr;
    });
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(connStr);
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/", () => $"The API is up. Connection string found: {(sqlConnectionStringFound ? "Yes" : "No")}");
app.MapControllers();
app.MapGroup("/account").MapIdentityApi<IdentityUser>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.Run();
