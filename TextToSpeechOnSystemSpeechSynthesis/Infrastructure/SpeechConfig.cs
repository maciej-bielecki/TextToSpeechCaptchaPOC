using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextToSpeechOnSystemSpeechSynthesis.Infrastructure
{
    public class SpeechConfig
    {
        public int SpeechRate { get; set; }//must be between -10 and 10

        public int SpaceBetweenLetters { get; set; }//i would say 2 as minimum - with 1 "7g" was readen as "7 grams"
    }
}
