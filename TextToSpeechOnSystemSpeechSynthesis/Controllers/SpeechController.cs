using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TextToSpeechOnSystemSpeechSynthesis.Infrastructure;

namespace TextToSpeechOnSystemSpeechSynthesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeechController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SpeechController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// </summary>
        /// <param name="text">text to read</param>
        /// <returns></returns>
        [HttpGet()]
        [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult SpeakWithPauses(string text)
        {
            var speechConfig = _configuration.GetSection("Speech").Get<SpeechConfig>();

            text = string.Join("".PadRight(speechConfig.SpaceBetweenLetters, ' '), text.ToCharArray());

            //some sources says that it fails if not used in separate task
            //concider using it in case of problems
            //  return await Task.Factory.StartNew(() =>
            //  {
            using (var synthesizer = new SpeechSynthesizer())
            {
                synthesizer.Rate = speechConfig.SpeechRate;
                var ms = new MemoryStream();
                synthesizer.SetOutputToWaveStream(ms);
                synthesizer.Speak(text);

                ms.Position = 0;
                return new FileStreamResult(ms, "audio/wav");
            }
            //  });
        }

    }
}
