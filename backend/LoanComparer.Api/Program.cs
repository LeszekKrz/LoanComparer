using FluentValidation;
using LoanComparer.Api.Middleware;
using LoanComparer.Application;
using LoanComparer.Application.Configuration;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services;
using LoanComparer.Application.Services.JwtFeatures;
using LoanComparer.Application.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SendGrid.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo { Title = "LoanComparer.Api", Version = "v1" }));

builder.Services.AddMvc();
builder.Services.AddValidatorsFromAssemblyContaining<UserForRegistrationDTOValidator>();

builder.Services.AddDbContext<LoanComparerContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    // options.Stores.ProtectPersonalData = true; maybe thats a good idea check it later
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<LoanComparerContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    options.TokenLifespan = TimeSpan.FromHours(2));

var jwtSettings = builder.Configuration.GetSection("JWTSettings");
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
            ValidIssuer = jwtSettings["validIssuer"],
            ValidAudience = jwtSettings["validAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["securityKey"]))
        };
    });

builder.Services.AddScoped<JwtHandler>();

builder.Services.AddTransient<JobTypeService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<EmailService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy( // TODO
        "Allow ALL",
        builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin());
});

builder.Services.AddOptions<FromEmailConfiguration>()
    .Bind(builder.Configuration.GetSection("FromEmail"))
    .ValidateDataAnnotations();
builder.Services.AddSendGrid(options => options.ApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseCors("Allow ALL");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();
