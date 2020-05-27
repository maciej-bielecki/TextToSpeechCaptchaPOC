using Microsoft.Extensions.Configuration;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TextToSpeechOnHardcodedSounds.Infrastructure;

namespace TextToSpeechOnHardcodedSounds.Speech
{
    public class WavCharactersProvider
    {
        private readonly Dictionary<char, byte[]> charactersWavBytes;

        public WavCharactersProvider(IConfiguration configuration)
        {
            var speechConfig = configuration.GetSection("SpeechConfig").Get<SpeechConfig>();

            charactersWavBytes = new Dictionary<char, byte[]>();
            var characters = new[] {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J','K', 'L', 'M',
                'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U','V', 'W', 'X', 'Y', 'Z',
                '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};

            byte[] readBuffer = new byte[16 * 1024];
            foreach (var character in characters)
            {
                //FileNotFoundException can be thrown
                using var waveFileReader = new WaveFileReader(Path.Combine(speechConfig.CharactersPath, $"{character}.wav"));
                using var ms = new MemoryStream();
                using var waveFileWriter = new WaveFileWriter(ms, waveFileReader.WaveFormat);

                int read;
                while ((read = waveFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
                {
                    waveFileWriter.Write(readBuffer, 0, read);
                }
                waveFileWriter.Flush();
                charactersWavBytes[character] = ms.ToArray();
            }
        }


        public ISampleProvider GetCharacterProvider(char character, WaveFormat waveFormat)
        {
            if (!charactersWavBytes.TryGetValue(char.ToUpper(character), out byte[] byteArray))
            {
                throw new KeyNotFoundException();
            }

            var ms = new MemoryStream(byteArray);
            var waveFileReader = new WaveFileReader(ms);

            if (waveFormat != null)
            {
                var backgroundConversionStream = new WaveFormatConversionStream(waveFormat, waveFileReader);
                return backgroundConversionStream.ToSampleProvider();
            }
            else
            {
                return waveFileReader.ToSampleProvider();
            }
        }
    }
}
