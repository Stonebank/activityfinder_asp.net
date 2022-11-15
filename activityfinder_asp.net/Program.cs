using activityfinder_asp.net.Models.Activities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Activity = activityfinder_asp.net.Models.Activities.Activity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (StreamReader reader = File.OpenText("activity.json"))
{
    string json = reader.ReadToEnd();
    Debug.WriteLine(json);
    Activity.activities = JsonConvert.DeserializeObject<List<Activity>>(json);
    Debug.WriteLine(Activity.activities[0].Link);
}


app.Run();
