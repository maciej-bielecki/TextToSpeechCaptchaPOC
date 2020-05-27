using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using TextToSpeechOnHardcodedSounds.Speech;

namespace TextToSpeechOnHardcodedSounds.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpeechController : ControllerBase
    {
        private readonly ISpeechFactory speechFactory;

        public SpeechController(ISpeechFactory speechFactory)
        {
            this.speechFactory = speechFactory;
        }

        /// <summary>
        /// TODO: 
        /// Trimm background sound if longer than characters
        /// Loop background sound if shorter than characters
        /// </summary>
        /// <param name="text">text to read</param>
        /// <returns></returns>
        [HttpGet("Speak")]
        [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult SpeakToMemoryStreamAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest();

            var ms = speechFactory.SpeakToMemoryStream(text);
            if (ms == null)
                return BadRequest();
            ms.Position = 0;
            return File(ms, "audio/wav");        
        }

    }
}
