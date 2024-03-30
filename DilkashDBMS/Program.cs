using DilkashDBMS.DAL;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = builder.Configuration;

string connStr = config.GetConnectionString("Dilkash")
    .Replace("|DataDirectory|", builder.Environment.ContentRootPath);

builder.Services.AddScoped<IFoodRepository>(
    x => new FoodAdoNetRepository(connStr)
    );

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

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
