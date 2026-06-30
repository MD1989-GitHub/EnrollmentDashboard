using EnrollmentDashboardApplication.Services;
using EnrollmentDashboardApplication.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// 1. REGISTER YOUR DATABASE CONTEXT HERE (The Missing Piece) 👇
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Your existing dependencies will now resolve perfectly
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

var app = builder.Build();

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Enrollments}/{action=Index}/{id?}");

app.Run();
