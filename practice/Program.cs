using practice;
using practice.Pages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

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

app.UseAuthorization();

app.MapRazorPages();

//Data.LoadHistory(new DateTime(2023, 06, 23), DateTime.Today);

Thread checkThread = new Thread(() =>
{
    Data.CheckRates(DateTime.Today);

    Thread.Sleep(new TimeSpan(12, 0, 0));
});
checkThread.Start();

Thread.Sleep(1500);
app.Run();
