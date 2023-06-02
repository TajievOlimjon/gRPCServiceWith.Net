using GrpcServiceApp.Data;
using GrpcServiceApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddDbContext<ApplicationDbContext>(config =>
{
    config.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
var app = builder.Build();

app.MapGrpcService<TranslatorService>();
app.MapGrpcService<StudentService>();
app.MapGrpcService<ServerMessageToClientService>();
app.MapGrpcService<ClientMessageToServerService>();
app.MapGrpcService<ClientServerMessengerService>();
app.MapGrpcService<MessengerService>();
app.MapGrpcService<UserApiService>();

app.MapGet("/", () => "Hello World!");
app.MapGet("test", () => "test project");

app.Run();
