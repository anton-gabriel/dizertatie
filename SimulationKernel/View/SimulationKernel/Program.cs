using DataMapper.SimulationKernel.Context;
using DataMapper.SimulationKernel.Repository;
using DomainModel.SimulationKernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.SimulationKernel;
using ServiceLayer.SimulationKernel.Validators;
using SimulationKernel.Areas.Identity;
using SimulationKernel.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("sqlServer");

builder.Services.AddDbContextFactory<SimulationKernelContext>(options =>
{
  options
    .UseLazyLoadingProxies()
    .UseSqlServer(connectionString);
});

builder.Services
  .AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
  .AddEntityFrameworkStores<SimulationKernelContext>();

// Add services to the container.
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
  options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "Login");
  options.Conventions.AddAreaPageRoute("Identity", "/Account/Register", "Register");
  options.Conventions.AddAreaPageRoute("Identity", "/Account/Logout", "Logout");
});

builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<TokenProvider>();

//Repositories
builder.Services.AddSingleton<ISimulationMetadataRepository, SimulationMetadataRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();

//Validators
builder.Services.AddSingleton<IValidator<SimulationMetadata>, SimulationMetadataValidator>();

//Services
builder.Services.AddSingleton<ProcessedDataService>();
builder.Services.AddSingleton<ITransferDataService, TransferDataService>();
builder.Services.AddSingleton<ISimulationMetadataService, SimulationMetadataService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.Run();
