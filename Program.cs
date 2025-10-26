using Microsoft.EntityFrameworkCore;
using QuizMaker.Data;
using QuizMaker.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Register the database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


// MVC + runtime compilation optional
builder.Services.AddControllersWithViews();

// Repos
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

// Logging is configured by default; you can tweak levels in appsettings.json

var app = builder.Build();

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // server-side error page
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage(); // detailed stack trace in dev
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Quizzes}/{action=Index}/{id?}");

app.Run();

