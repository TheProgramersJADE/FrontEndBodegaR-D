using FrontEndBodega.Components;
using FrontEndBodega.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// Cliente para Autenticación (ya lo tienes)
builder.Services.AddScoped(o => new HttpClient
{
    BaseAddress = new Uri("https://api-gateway-nodejs-ryd-miih.onrender.com/ApiAutenticacion/")
});

// Cliente para Administración (nuevo)
builder.Services.AddHttpClient("Administracion", client =>
{
    client.BaseAddress = new Uri("https://api-gateway-nodejs-ryd-miih.onrender.com/ApiAdministracion/");
});

// Dentro de Program.cs
builder.Services.AddScoped<RegistroUserService>();
builder.Services.AddScoped<AutheService>();
builder.Services.AddScoped<RolService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<ProductoService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
