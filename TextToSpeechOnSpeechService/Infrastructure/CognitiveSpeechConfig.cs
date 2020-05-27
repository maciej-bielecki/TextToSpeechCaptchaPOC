using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextToSpeechOnSpeechService.Infrastructure
{
    public class CognitiveSpeechConfig
    {
        public string SubscriptionKey { get; set; }

        public string Region { get; set; }

        public int BreakAtBeginning { get; set; }

        public string VoiceName { get; set; }

        public decimal ProsodyRate { get;  set; }

        public bool AddBackgroundAudio { get;  set; }

        public string BackgroundAudioSource { get; set; }

        public string OutputFilename { get; set; }
    }
}
