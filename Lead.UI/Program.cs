using Lead.UI.Interfaces;
using Lead.UI.Services;
using Lead.UI.Settings;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>();

// Register HttpClient + HttpService
builder.Services.AddHttpClient<IHttpService, HttpService>(client =>
{
    client.BaseAddress = new Uri(apiSettings!.BaseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".LeadUI.Session";
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<RazorViewEngineOptions>(o =>
{
    o.ViewLocationFormats.Clear();
    o.ViewLocationFormats.Add("~/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
    o.ViewLocationFormats.Add("~/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
    o.ViewLocationFormats.Add("~/Views/Common/{1}/{0}" + RazorViewEngine.ViewExtension);
    o.ViewLocationFormats.Add("~/Views/Auth/{1}/{0}" + RazorViewEngine.ViewExtension);
    o.ViewLocationFormats.Add("~/Views/Menu/{1}/{0}" + RazorViewEngine.ViewExtension);
    o.ViewLocationFormats.Add("~/Views/Lead/{1}/{0}" + RazorViewEngine.ViewExtension);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// ✅ Serve CSS/JS/images first
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

