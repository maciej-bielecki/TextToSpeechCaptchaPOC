using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextToSpeechOnSpeechService.Infrastructure;

namespace TextToSpeechOnSpeechService.Speech
{
    public class SsmlFactory : ISsmlFactory
    {
        private readonly IConfiguration configuration;

        public SsmlFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        /// <summary>
        /// not very well written, better use some ssml builder from nuget repository
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string CreateSsmlForText(string text)
        {
            var speechConfig = configuration.GetSection("CognitiveSpeech").Get<CognitiveSpeechConfig>();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(@"<speak version=""1.0"" xml:lang=""en-US"" xmlns:mstts=""http://www.w3.org/2001/mstts"">");
           
            if (speechConfig.AddBackgroundAudio)
            {
                stringBuilder.Append($@"<mstts:backgroundaudio src=""{speechConfig.BackgroundAudioSource}""/>");
            }

            stringBuilder.Append($@"<voice name =""{speechConfig.VoiceName}"">");

            if (speechConfig.BreakAtBeginning > 0)
            {
                stringBuilder.Append($@"<break time=""{speechConfig.BreakAtBeginning}"" />");
            }

            stringBuilder.Append($@"<prosody rate=""{speechConfig.ProsodyRate}"">");
            stringBuilder.Append($@"<say-as interpret-as=""characters"">{text}</say-as>");
            stringBuilder.Append(@"</prosody>");
            stringBuilder.Append(@"</voice>");
            stringBuilder.Append(@"</speak>");

            return stringBuilder.ToString() ;
        }


    }
}
