using System.Security.Cryptography;

var rsa = RSA.Create();
var privateKey = rsa.ExportRSAPrivateKey();

File.WriteAllBytes("key", privateKey);