using System.Security.Cryptography;

var rsa = RSA.Create();
rsa.ImportRSAPrivateKey(File.ReadAllBytes("key"), out _);

var builder = WebApplication.CreateBuilder(args);

var application = builder.Build();

application.MapGet("/", () => "Hello World!");

application.Run();