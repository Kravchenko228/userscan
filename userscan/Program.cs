using Microsoft.EntityFrameworkCore;
using userscan.Models;
using userscan.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<ContactsDbContext>(options =>
    options.UseSqlite("Data Source=contacts.db"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});



var app = builder.Build();
var dbContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<ContactsDbContext>();
Console.WriteLine("Using DB file: " + dbContext.Database.GetDbConnection().DataSource);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}





app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
