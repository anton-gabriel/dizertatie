using ServiceLayer.SimulationKernel;
using SimulationKernel.Data;
using Microsoft.EntityFrameworkCore;
using DataMapper.SimulationKernel.Context;
using DomainModel.SimulationKernel;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SimulationKernelContextConnection");

builder.Services.AddDbContext<SimulationKernelContext>(options =>
  options.UseSqlServer(connectionString));

builder.Services
  .AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
  .AddEntityFrameworkStores<SimulationKernelContext>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<ProcessedDataService>();
builder.Services.AddSingleton<ITransferDataService, TransferDataService>();

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

app.Run();
