using Microsoft.Identity.Web;
using Microsoft.Graph;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
ConfigurationManager Configuration = builder.Configuration; // allows both to access and to set up the config
IWebHostEnvironment environment = builder.Environment;
// Retrieve required permissions from appsettings
string[] initialScopes = Configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ');

//app or client id: ae2b5053-c7c4-481d-a0e1-68cbadc15558
//tenant id: 21af2c97-fc02-46fd-84fb-323961b73470
// Add services to the container.
builder.Services.AddControllersWithViews();

//added by hasne
builder.Services.AddMicrosoftIdentityWebAppAuthentication(Configuration)
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddInMemoryTokenCaches()
    .AddMicrosoftGraph();

builder.Services.AddMvc(options =>
{
    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser().Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
