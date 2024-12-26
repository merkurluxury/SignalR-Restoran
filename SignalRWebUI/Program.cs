using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using SignalR.DataAccessLayer.Concrete;
using SignalR.EntityLayer.Entities;

var builder = WebApplication.CreateBuilder(args);

// ESKÝ KOD: var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

// DbContext ve Identity ayarlarýný ekliyoruz.
builder.Services.AddDbContext<SignalRContext>();
builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<SignalRContext>();
builder.Services.AddHttpClient();

// Tüm controller/action’lara yetkilendirme uygulamak yerine, 
// Authorize iþlemine proje seviyesi yerine ihtiyaca göre controller veya action’da [Authorize] ekleyerek devam edeceðiz.
builder.Services.AddControllersWithViews();
// ESKÝ KOD: .AddControllersWithViews(opt =>
// {
//     opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
// });

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Login/Index";
});

var app = builder.Build();

app.UseStatusCodePages(async x =>
{
    if (x.HttpContext.Response.StatusCode == 404)
    {
        x.HttpContext.Response.Redirect("/Error/NotFound404Page/");
    }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication ve Authorization middleware'lerini kullanýyoruz.
// Bu sayede ihtiyacýmýz olan controller/action’lara `[Authorize]` ekleyebiliriz.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
