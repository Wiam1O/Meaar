
using Meaar5.Data;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using OfficeOpenXml;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
// ⁄‘«‰ «›⁄· «·„ﬂ »… »œÊ‰ „‘«ﬂ· «· —ŒÌ’
ExcelPackage.License.SetNonCommercialPersonal("Meaar");

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
//to setup the Rotativa configuration with the web root path and the folder name for the Rotativa tools

RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");


app.UseSession();


app.Run();
