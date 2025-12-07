using Grosu_Andrada_Lab4.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<Grosu_Andrada_Lab4Context>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Grosu_Andrada_Lab4Context")
            ?? throw new InvalidOperationException("Connection string 'Grosu_Andrada_Lab4Context' not found.")
    ));

var app = builder.Build();

// ?? Creeaz? baza de date dac? nu exist?
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Grosu_Andrada_Lab4Context>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
