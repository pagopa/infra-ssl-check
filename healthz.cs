using System;
using System.Diagnostics;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace PagoPA
{
  public class Liveness
  {
    [FunctionName("Liveness")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "healthz/live")]HttpRequest req, ILogger log)
    {
      log.LogInformation("Alive..");
      return new NoContentResult();
    }
  }

  public class Readiness
  {
    [FunctionName("Readiness")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "healthz/ready")]HttpRequest req, ILogger log)
    {
      log.LogInformation("Ready..");
      return new NoContentResult();
    }
  }
}
