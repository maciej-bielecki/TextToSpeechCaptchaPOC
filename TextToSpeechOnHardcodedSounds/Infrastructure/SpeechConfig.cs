using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextToSpeechOnHardcodedSounds.Infrastructure
{
    public class SpeechConfig
    {
        public string CharactersPath { get; set; }

        public string BackgroundAudioPath { get;  set; }

        public bool AddBackgroundAudio { get; set; }

        public float BackgroundAudioVolume { get; set; }
    }
}
