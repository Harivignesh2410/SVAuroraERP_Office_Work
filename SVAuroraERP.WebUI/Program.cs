using SVAuroraERP.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Register IHttpContextAccessor (Added on 2024.09.02)
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddRazorPages().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Preserve case for JSON keys
});

//Add support to logging with SERILOG
#region "Serilog Configuration"
var logFolder = Path.Combine("logs", DateTime.Now.ToString("yyyyMM"));
var logFilePath = Path.Combine(logFolder, "log_.log");

// Ensure the log directory exists
Directory.CreateDirectory(logFolder);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: null)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddLogging();
#endregion

// Add DB Connection
builder.Services.AddDbContext<SVAuroraERPDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection"));
    options.EnableSensitiveDataLogging(); // Enable sensitive data logging
});

//Added on 2024.07.07
builder.Services.AddDbContext<SVAuroraERPLogDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LogDBConnection"));
    options.EnableSensitiveDataLogging(); // Enable sensitive data logging
});

//Add Version - Add services to the container.
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
var appSettingsSection = configuration.GetSection("ApplicationSettings");
var appSettings = appSettingsSection.Get<ApplicationSettings>();
builder.Services.AddSingleton(appSettings);

builder.Services.AddSingleton<AppVersionService>();
//builder.Services.AddSignalR();// Add SignalR services

//builder.Services.AddScoped<ITransLogRespository, TransLogRespository>();

builder.Services.AddPresentation(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSession();
builder.Services.AddScoped<SessionService>();
builder.Services.AddHttpContextAccessor();

//Enable Login
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {

            // Set the cookie expiration time
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Set session expiration to 30 minutes
            //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            options.LoginPath = "/SignIn";
            options.LogoutPath = "/SignOut"; // Update path as needed
            //options.AccessDeniedPath = "/AccessDenied";

            // Sliding expiration: reset the expiration time on each request
            options.SlidingExpiration = true;
        });

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"./keys"))  // Specify a path to store the keys
    .SetApplicationName("SV Aurora ERP");                    // Optional: set a unique app name

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;                // Make the session cookie HttpOnly
    options.Cookie.IsEssential = true;             // Make the session cookie essential
});

var app = builder.Build();
//app.MapHub<NotificationHub>("/notificationHub");

app.UseCookiePolicy(
   new CookiePolicyOptions
   {
       Secure = CookieSecurePolicy.Always
   });

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<SessionValidationMiddleware>();

app.UseStatusCodePages(context =>
{
    if (context.HttpContext.Response.StatusCode == 404)
    {
        context.HttpContext.Response.Redirect("/AccessDenied");
    }
    return Task.CompletedTask;
});

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
