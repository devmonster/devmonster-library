using Devmonster.Core.SuperBasicAuth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSuperBasicAuthentication(o => {
    o.BasicCredentials = "username:password";
});


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseSuperBasicAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
