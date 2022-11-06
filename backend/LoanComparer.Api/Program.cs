using FluentValidation;
using LoanComparer.Api.Middleware;
using LoanComparer.Application;
using LoanComparer.Application.Model;
using LoanComparer.Application.Services;
using LoanComparer.Application.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo { Title = "LoanComparer.Api", Version = "v1" }));

builder.Services.AddMvc();
builder.Services.AddValidatorsFromAssemblyContaining<UserForRegistrationDTOValidator>();

builder.Services.AddDbContext<LoanComparerContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<LoanComparerContext>();

builder.Services.AddTransient<JobTypeService>();
builder.Services.AddTransient<UserService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy( // TODO
        "Allow ALL",
        builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin());
});

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

app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();
