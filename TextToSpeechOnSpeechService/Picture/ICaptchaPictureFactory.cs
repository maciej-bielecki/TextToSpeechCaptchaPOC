using System.IO;

namespace TextToSpeechOnSpeechService
{
    public interface ICaptchaPictureFactory
    {
        MemoryStream GetCaptchPictureStream(string text);
    }
}