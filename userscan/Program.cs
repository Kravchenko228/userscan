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

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "userscan V1");
    c.RoutePrefix = "swagger";
});





app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();
    db.Database.Migrate();

    if (!db.Contacts.Any())
    {
        db.Contacts.Add(new Contact
        {
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            Cell = "1234567890",
            FullAddress = "123 Sample St.",
            ImageUrl = "https://example.com/image.jpg"
        });
        db.SaveChanges();
        Console.WriteLine("Seeded initial contact.");
    }
}


app.Run();
