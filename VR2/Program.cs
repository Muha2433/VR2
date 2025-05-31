using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using VR2.DataBaseServices;
using VR2.HubRealTime_Playing;
using VR2.Models;
using VR2.MyTools;
using VR2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext and Identity
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSignalR();
// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = TokenService.GetTokenValidationParameters(builder.Configuration);
   options.Events= new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            // If the request is for our SignalR hub
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

// Configure CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAllOrigins", policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173") // React origin
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // لازم تكون موجودة
    });

    // سياسة بديلة تسمح للجوال أو أي جهة بدون credentials
    options.AddPolicy("AllowAllWithoutCredentials", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// Add Health Checks
builder.Services.AddHealthChecks();

// Add Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

//
builder.Services.AddTransient<IGroupSaleTracker, GroupSaleTracker>();

// Cryptographic Services
builder.Services.AddSingleton<ICryptoService, CryptoService>();
builder.Services.AddTransient<IKeyManagementService,KeyManagementService>();
//
builder.Services.AddScoped<AgentService>();

//
builder.Services.AddSingleton<GroupSaleBackgroundWorker>();
//
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<FileTools>(); // If not already registered
//builder.Services.AddScoped<IPropertiesService, PropertiesService>();
builder.Services.AddScoped<CustomerServices>();
builder.Services.AddScoped<PropertiesService>();
builder.Services.AddTransient<TokenService>();
//builder.Services.AddSingleton<FileTools>();
builder.Services.AddTransient<AccountService>();
builder.Services.AddScoped<SaleService>();
/////////////////////////////
builder.Services.AddScoped<crudRequestSale>();

////////////////////////////

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 20 * 1024 * 1024; // 20MB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 20 * 1024 * 1024; // 20MB
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
//builder.Services.AddSignalR();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();

// Enable CORS
//app.UseCors("AllowAllOrigins");
app.UseCors("DefaultCorsPolicy");

// Global error handling
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}
app.MapHub<NotificationHub>("/notificationHub");

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();  // Ensure static files are enabled

app.MapControllers();
// Add this line right here:
app.MapGet("/", () => "API is running").ExcludeFromDescription();
// Health check endpoint
app.MapHealthChecks("/health");

app.Run();






