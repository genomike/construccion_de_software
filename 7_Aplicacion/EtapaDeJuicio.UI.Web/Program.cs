using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using EtapaDeJuicio.UI.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configurar HttpClient para el API Gateway
builder.Services.AddHttpClient<AudienciaService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7000");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<InterrogatorioService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7000");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<PruebasService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7000");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<PruebaDocumentalService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7000");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<PruebaTestimonialService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7000");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<PruebaPericialesService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7000");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<SentenciaService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7000");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<DeliberacionService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7000");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Registrar el servicio de notificaciones
builder.Services.AddSingleton<NotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
