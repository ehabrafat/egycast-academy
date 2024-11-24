using Microsoft.AspNetCore.Mvc;

namespace EgycastApi.Test;

[ApiController]
public class TestController : ControllerBase
{
   [Route("/test")]
   public string Test()
   {
      return "Test\n";
   }
}