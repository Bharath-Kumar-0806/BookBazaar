using BookBazaarApi.DAL;
using BookBazaarApi.Models;
using BookBazaarApi.Repos.Classes;
using BookBazaarApi.Repos.Interfaces;
using BookBazaarApi.Services.Classes;
using BookBazaarApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
                  options.UseSqlServer(builder.Configuration.GetConnectionString("dbconnection")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICategory,CategoryRepository>();
builder.Services.AddScoped<IBookRepository,BookRepository>();
builder.Services.AddScoped<IBookService, BookServices>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAdminServices,AdminServices>();
builder.Services.AddScoped<IAdminRepo,AdminRepo>();
builder.Services.AddScoped<PasswordHasherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // 1. Ensure Role exists
    var adminRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
    if (adminRole == null)
    {
        adminRole = new Role { Name = "Admin" };
        db.Roles.Add(adminRole);
        await db.SaveChangesAsync();
    }

    // 2. Ensure Admin user exists
    var adminUser = await db.Users.FirstOrDefaultAsync(u => u.Username == "admin");
    if (adminUser == null)
    {
        var passwordHasher = new PasswordHasher<User>();
        adminUser = new User
        {
            Username = "admin",
            Email = "admin@example.com",
            CreatedAt = DateTime.Now,
            IsActive = true
        };
        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin@123"); // set a strong password

        db.Users.Add(adminUser);
        await db.SaveChangesAsync();

        // 3. Assign Role
        db.UserRoles.Add(new UserRole
        {
            UserId = adminUser.Id,
            RoleId = adminRole.Id
        });
        await db.SaveChangesAsync();
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
