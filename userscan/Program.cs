using Microsoft.EntityFrameworkCore;
using userscan.Models;
using userscan.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UserContext>(opt => opt.UseInMemoryDatabase("UserList"));
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
