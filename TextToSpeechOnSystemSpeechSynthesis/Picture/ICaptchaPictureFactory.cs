using System.IO;

namespace TextToSpeechOnSystemSpeechSynthesis
{
    public interface ICaptchaPictureFactory
    {
        MemoryStream GetCaptchPictureStream(string text);
    }
}