using Common.DI;
using Core.Models.Auth;
using Core.ViewModels.Dto.Auth.Auth;
using Infrastructure.Repositories.Data;
using Lead.Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


// Configure Serilog to log only to SQL Server
var columnOptions = new ColumnOptions
{
    AdditionalColumns =
    [
        new SqlColumn("UserName", System.Data.SqlDbType.NVarChar, dataLength: 256),
        new SqlColumn("Path", System.Data.SqlDbType.NVarChar, dataLength: 256)
    ]
};

var sinkOptions = new MSSqlServerSinkOptions
{
    TableName = "Logs",
    AutoCreateSqlTable = true,
    // Optionally adjust batching:
    BatchPostingLimit = 100,
    BatchPeriod = TimeSpan.FromSeconds(5)
};

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("AspNetCore", LogEventLevel.Warning)
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: sinkOptions,
        columnOptions: columnOptions,
        restrictedToMinimumLevel: LogEventLevel.Information
    )
    .Filter.ByExcluding(e =>
        e.Properties.ContainsKey("SourceContext") &&
        (e.Properties["SourceContext"].ToString().Contains("Microsoft.EntityFrameworkCore") ||
         e.MessageTemplate.Text.Contains("HTTP")))
    .CreateLogger();

builder.Host.UseSerilog();


try
{
    // Add services to the container.
    builder.Services.AddDbContext<LeadContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    //  For code first approach
    //  ->Specify where migrations will live
    //builder.Services.AddDbContext<LeadContext>(options =>
    //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    //        b => b.MigrationsAssembly("Infrastructure.Repositories")));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<LeadContext>()
        .AddDefaultTokenProviders();

    // Configure Identity options
    builder.Services.Configure<IdentityOptions>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;
    });


    // Configure JWT Authentication
    builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWT"));
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "LeadSolution", Version = "v1" });

        // JWT Auth Definition
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"Enter 'Bearer' [space] and then your valid token.<br>Example: Bearer eyJhbGciOiJIUzI1NiIsInR...",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        // Attach Security to all operations
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
    });


    builder.Services.AddHttpContextAccessor();
    builder.Services.AddAuthorization();

    // Add CORS policy
    //var allowedOrigins = new[] { "https://localhost:7131", "http://localhost:5186" };

    //builder.Services.AddCors(options =>
    //{
    //    options.AddPolicy("AllowSpecificOrigins", policy =>
    //        policy.WithOrigins(allowedOrigins) // Allow multiple origins
    //              .AllowAnyMethod()            // Allow all HTTP methods (GET, POST, etc.)
    //              .AllowAnyHeader());          // Allow all headers
    //});
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
            policy.AllowAnyOrigin()   // ✅ Allow requests from any origin
                  .AllowAnyMethod()   // ✅ Allow all HTTP methods
                  .AllowAnyHeader()); // ✅ Allow all headers
    });

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    //builder.Services.AddOpenApi();
    builder.Services.AddScopedAppServices();
    var app = builder.Build();

    // Seed roles on startup
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var roles = new[] { "Admin", "User" };

        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        //app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }


    // Use the CORS policy
    app.UseCors("AllowAll");
    //app.UseCors("AllowSpecificOrigins");

    // Register your middleware before routing
    app.UseMiddleware<RequestLoggingMiddleware>();

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();


    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}