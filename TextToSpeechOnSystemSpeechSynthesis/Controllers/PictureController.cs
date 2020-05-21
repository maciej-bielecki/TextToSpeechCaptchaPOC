using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace TextToSpeechOnSystemSpeechSynthesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PictureController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICaptchaPictureFactory _captchaPictureFactory;

        public PictureController(IConfiguration configuration, ICaptchaPictureFactory captchaPictureFactory)
        {
            _configuration = configuration;
            _captchaPictureFactory = captchaPictureFactory;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetCaptcha(string text)
        {
            var stream = _captchaPictureFactory.GetCaptchPictureStream(text);
            return new FileStreamResult(stream, "image/png");
        }

    }


}

