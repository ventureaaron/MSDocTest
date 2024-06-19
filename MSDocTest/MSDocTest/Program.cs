using Microsoft.Extensions.FileProviders;
using MSDocTest.Client.Pages;
using MSDocTest.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

//Adding secondary file serving and webroot tag helper to help with caching - see .net core 8 static file serving documentation for more
/* The way that works */
var secondaryProvider = new PhysicalFileProvider(
    builder.Configuration.GetValue<String>("serverDir:HomePath")!);

app.Environment.WebRootFileProvider = new CompositeFileProvider(app.Environment.WebRootFileProvider, secondaryProvider);

/* The documentation way that breaks */
var webRootProvider = new PhysicalFileProvider(builder.Environment.WebRootPath);
var compositeProvider = new CompositeFileProvider(webRootProvider, secondaryProvider);
app.Environment.WebRootFileProvider = compositeProvider;


app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MSDocTest.Client._Imports).Assembly);

app.Run();
