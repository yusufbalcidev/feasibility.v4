using feasibility.Business.Extensions;
using feasibility.Business.Middlewares;
using feasibility.DataAccess.Context;
using feasibility.DataAccess.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddBusiness(builder.Configuration);

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/AccessDenied");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/NotFound");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ActivityLogMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
