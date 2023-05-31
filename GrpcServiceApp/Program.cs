using GrpcServiceApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
var app = builder.Build();

app.MapGrpcService<TranslatorService>();
app.MapGrpcService<StudentService>();

app.MapGet("/", () => "Hello World!");
app.MapGet("test", () => "test project");

app.Run();
