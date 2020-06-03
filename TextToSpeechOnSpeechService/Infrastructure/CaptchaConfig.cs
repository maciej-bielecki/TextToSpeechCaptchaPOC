using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace TextToSpeechOnSpeechService.Infrastructure
{
    public class CaptchaConfig
    {
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public int FontSize { get; set; }
        public string FontFamilyPath { get; set; }
        public string FontFamilyName { get; set; }
        public FontStyle FontStyle { get; internal set; }

        public bool DisturbtionLines { get; set; }

        public int DisturbtionLinesCount { get; set; }
        public int DisturbtionLinesWidth { get; set; }

        public bool DisturbString { get; set; }

        public int DisturbStringHeightDivideFactor { get; set; }
        public int DisturbStringWidthDivideFactor { get; set; }

        public int DisturbStringFontResizeFactor { get; set; }
        public int DisturbStringRotateFactor { get; set; }
    }
}
