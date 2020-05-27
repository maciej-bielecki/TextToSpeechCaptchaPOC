using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TextToSpeechOnHardcodedSounds.Speech
{
    public interface ISpeechFactory
    {
        MemoryStream SpeakToMemoryStream(string text);
    }
}
