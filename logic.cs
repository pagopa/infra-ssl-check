using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;

namespace PagoPA
{
  [Serializable()]
  public class SslExpirationException : Exception
  {
    public SslExpirationException(string msg) : base(msg) {}
  }

  public class Test
  {
    private ILogger log;
    private Options options;
    private DateTime certExpiration;

    public Test(Options options, ILogger log)
    {
      this.log = log;
      this.options = options;
    }

    public void Run(string url)
    {
      log.LogDebug("[DEBUG] Init HttpClient..");
      HttpClientHandler handler = new HttpClientHandler();
      handler.ServerCertificateCustomValidationCallback = CertCustomValidation;
      HttpClient client = new HttpClient(handler);

      var req = new HttpRequestMessage(HttpMethod.Get, url);
      HttpResponseMessage response = client.Send(req);
      response.EnsureSuccessStatusCode();

      if (certExpiration < DateTime.Now.AddDays(options.ExpirationDeltaInDays))
      {
        throw new SslExpirationException($"{url} - expiration: [{certExpiration.ToString("u")}]");
      }

      handler.Dispose();
      client.Dispose();
    }

    private bool CertCustomValidation(HttpRequestMessage req, X509Certificate2 cert, X509Chain chain, SslPolicyErrors sslErrors)
    {
      log.LogDebug($"[DEBUG] Requested URI: {req.RequestUri}");
      log.LogDebug($"[DEBUG] Effective date: {cert.GetEffectiveDateString()}");
      log.LogDebug($"[DEBUG] Exp date: {cert.GetExpirationDateString()}");
      log.LogDebug($"[DEBUG] Issuer: {cert.Issuer}");
      log.LogDebug($"[DEBUG] Subject: {cert.Subject}");

      certExpiration = DateTime.Parse(cert.GetExpirationDateString());

      return sslErrors == SslPolicyErrors.None;
    }
  }
}
