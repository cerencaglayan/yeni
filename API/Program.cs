using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using yeni.Application.Services;
using yeni.Data.Repository;
using yeni.Domain.DTO.Requests;
using yeni.Domain.Repositories;
using yeni.Domain.Repositories;
using yeni.Infrastructure.Configuration;
using yeni.Infrastructure.Configuration.Jobs;
using yeni.Infrastructure.Persistence;
using yeni.Infrastructure.Persistence.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ✅ SWAGGER 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Yeni API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.Configure<GmailConfig>(
    builder.Configuration.GetSection(GmailConfig.GmailOptionKey));


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("ApplicationDbConnectionString")
    );
});

var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            )
        };
    })
    
    
    
    ;

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IMemoryAttachmentRepository, MemoryAttachmentRepository>();
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
// ✅ AWS / B2 CONFIG
// ✅ B2 OPTIONS
builder.Services.Configure<B2StorageOptions>(
    builder.Configuration.GetSection("B2Storage"));

// ✅ AWS S3 CLIENT (B2 S3-COMPATIBLE)
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var options = sp.GetRequiredService<IOptions<B2StorageOptions>>().Value;

    var config = new AmazonS3Config
    {
        ServiceURL = options.ServiceUrl,
        ForcePathStyle = true
    };

    var client =  new AmazonS3Client(
        options.AccessKey,
        options.SecretKey,
        config
    );
    PutObjectRequest request = new PutObjectRequest();
    var stream = new MemoryStream();
    
    request.InputStream = stream;
    request.BucketName = options.BucketName;      
    request.Key = "data/";    
    request.CannedACL = S3CannedACL.PublicRead;

    client.PutObjectAsync(request);  
    
    return client;
});

// ✅ STORAGE SERVICE INTERFACE KAYDI
builder.Services.AddScoped<IFileStorageService, B2StorageService>();


// ✅ INTERFACE ÜZERİNDEN KAYIT
builder.Services.AddScoped<B2StorageService>();
builder.Services.AddScoped<MemoryService>();
builder.Services.AddScoped<IMemoryRepository, MemoryRepository>();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100MB
});
builder.Services.AddHostedService<TimerService>();
builder.Services.AddScoped<MailJob>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Yeni API v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthentication();
app.UseAuthorization();


var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

app.MapPost("/email", async (
            [FromBody] SendMailRequest request, IMailService mailService)
        =>
    {
        await mailService.SendMailAsync(request);
    }
);



app.MapControllers();
app.Run();