using System.IO;
using System.Threading.Tasks;

namespace TextToSpeechOnSpeechService.Speech
{
    public interface ISpeechFactory
    {
        Task<MemoryStream> SpeakToMemoryStream(string text);

        Task<MemoryStream> SpeakToMemoryStreamWithHeaderAdded(string text);

        Task<FileStream> SpeakToWaveFile(string text);
    }
}