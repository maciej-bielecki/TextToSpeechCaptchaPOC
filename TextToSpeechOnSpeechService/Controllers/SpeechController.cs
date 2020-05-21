using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.IO;
using TextToSpeechOnSpeechService.Speech;

namespace TextToSpeechOnSpeechService.Controllers
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
        /// </summary>
        /// <param name="text">text to read</param>
        /// <returns></returns>
        [HttpGet("Speak")]
        [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SpeakToMemoryStreamAsync(string text)
        {
            var ms = await speechFactory.SpeakToMemoryStream(text);
            if (ms == null)
                return BadRequest();
            return File(ms, "audio/wav");
        }

        /// <summary>
        /// </summary>
        /// <param name="text">text to read</param>
        /// <returns></returns>
        [HttpGet("SpeakWithModifiedHeader")]
        [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SpeakToMemoryStreamWithHeaderAddedAsync(string text)
        {
            var ms = await speechFactory.SpeakToMemoryStreamWithHeaderAdded(text);
            if (ms == null)
                return BadRequest();
            return File(ms, "audio/wav");
        }

        /// <summary>
        /// </summary>
        /// <param name="text">text to read</param>
        /// <returns></returns>
        [HttpGet("SpeakWithDiscSave")]
        [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SpeakWithDiskSaveAsync(string text)
        {
            var fileStream = await speechFactory.SpeakToWaveFile(text);
            return File(fileStream, "audio/wav");
        }


    }
}
