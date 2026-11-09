using Microsoft.EntityFrameworkCore;
using IF133.Tugas1.Api.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Metadata;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "Connection string 'DefaultConnection' belum diatur.")
    )
);

// Tambahkan CORS untuk development di port 3000
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    policy.WithOrigins("http://localhost:5173") // tambahkan port untuk frontend
    .AllowAnyHeader()
    .AllowAnyMethod());
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("FrontendDev");

// menambahkan Global exception handler
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var logger = context.RequestServices
    .GetRequiredService<ILogger<Program>>();
        var exception = context.Features
    .Get<IExceptionHandlerFeature>()?.Error;
        logger.LogError(exception, "Unhandled exception occurred");
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await Results.Problem(
    title: "Internal Server Error",
    detail: "Terjadi kesalahan pada server.",
    statusCode: StatusCodes.Status500InternalServerError)
    .ExecuteAsync(context);
    });
});

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
