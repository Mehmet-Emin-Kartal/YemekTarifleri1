// --- GLOBAL USING SATIRLARI ---
global using YemekTarifleri;
global using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// --- DB CONTEXT SERVÝSÝNÝN EKLENMESÝ ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- SESSION SERVÝSÝNÝN EKLENMESÝ (KRÝTÝK) ---
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// --- SESSION MIDDLEWARE'ÝNÝN KULLANILMASI (KRÝTÝK) ---
app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();