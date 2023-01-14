using System.Security.Authentication;
using FluentValidation;
using LoanComparer.Api.Middleware;
using LoanComparer.Application;
using LoanComparer.Application.Configuration;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services;
using LoanComparer.Application.Services.Inquiries;
using LoanComparer.Application.Services.JwtFeatures;
using LoanComparer.Application.Services.Offers;
using LoanComparer.Application.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SendGrid.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().
    AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "LoanComparer.Api", Version = "v1" }));

builder.Services.AddMvc();
builder.Services.AddValidatorsFromAssemblyContaining<UserForRegistrationDTOValidator>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "Allow ALL",
        policyBuilder => policyBuilder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin());
});

ConfigureOptions(builder.Services, builder.Configuration);
ConfigureUserIdentity(builder.Services);
ConfigureJwt(builder.Services, builder.Configuration);
ConfigureServices(builder.Services);
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if(app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseCors("Allow ALL");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => {
    if (app.Environment.IsDevelopment())
        endpoints.MapControllers().AllowAnonymous();
    else
        endpoints.MapControllers();
});

app.Run();

void ConfigureOptions(IServiceCollection services, IConfiguration config)
{
    services.AddOptions<FromEmailConfiguration>()
        .Bind(config.GetSection("FromEmail"))
        .ValidateDataAnnotations();
    services.AddOptions<InquiryConfiguration>()
        .Bind(config.GetSection(InquiryConfiguration.SectionName))
        .ValidateDataAnnotations();
}

void ConfigureUserIdentity(IServiceCollection services)
{
    services.AddIdentity<User, Role>(options =>
        {
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        }).AddUserStore<
            UserStore<
                User,
                Role,
                LoanComparerContext,
                string,
                IdentityUserClaim<string>,
                UserRole,
                IdentityUserLogin<string>,
                IdentityUserToken<string>,
                IdentityRoleClaim<string>>>()
        .AddRoleStore<RoleStore<Role, LoanComparerContext, string, UserRole, IdentityRoleClaim<string>>>()
        .AddDefaultTokenProviders();
}

void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<LoanComparerContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
    
    services.AddScoped<JwtHandler>();
    services.AddTransient<JobTypeService>();
    services.AddTransient<UserService>();
    services.AddTransient<IEmailService, EmailService>();
    services.AddScoped<IInquiryCommand, InquiryCommand>();
    services.AddScoped<IInquiryQuery, InquiryQuery>();
    services.AddScoped<IInquirySender, InquirySender>();
    services.AddScoped<IInquiryRefresher, InquiryRefresher>();
    services.AddScoped<IBankInterfaceFactory, BankInterfaceFactory>();
    services.AddScoped<IOfferCommand, OfferCommand>();

    services.AddHostedService<InquiryRefreshBackgroundService>();
    services.AddHostedService<InquiryCleanupBackgroundService>();

    services.AddSendGrid(options => options.ApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY") ??
                                                     throw new InvalidCredentialException(
                                                         "Environment variable SENDGRID_API_KEY is not defined"));
}

void ConfigureJwt(IServiceCollection services, IConfiguration config)
{
    services.Configure<DataProtectionTokenProviderOptions>(options =>
        options.TokenLifespan = TimeSpan.FromHours(2));

    var jwtSettings = new JwtSettings(config.GetSection("JWTSettings"));
    services.AddSingleton(jwtSettings);

    services.AddAuthentication(options =>
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
                ValidIssuer = jwtSettings.ValidIssuer,
                ValidAudience = jwtSettings.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(jwtSettings.SecurityKey)
            };
        });
}
