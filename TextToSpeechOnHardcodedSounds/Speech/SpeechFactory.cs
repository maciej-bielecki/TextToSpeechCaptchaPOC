using Microsoft.Extensions.Configuration;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextToSpeechOnHardcodedSounds.Infrastructure;

namespace TextToSpeechOnHardcodedSounds.Speech
{
    public class SpeechFactory : ISpeechFactory
    {
        private readonly WavCharactersProvider wavCharactersProvider;
        private readonly IConfiguration configuration;
        private static Regex whitespaceRegex = new Regex(@"\s+");

        public SpeechFactory(WavCharactersProvider wavCharactersProvider, IConfiguration configuration)
        {
            this.wavCharactersProvider = wavCharactersProvider;
            this.configuration = configuration;
        }
        public MemoryStream SpeakToMemoryStream(string text)
        {
            var speechConfig = configuration.GetSection("SpeechConfig").Get<SpeechConfig>();
            var newFormat = new WaveFormat(8000, 16, 1);

            var playlist = CreatePlaylistFromText(text, newFormat);

            if (speechConfig.AddBackgroundAudio)
            {
                ISampleProvider backgroundSampleChannel = CreateBackgroundProvider(speechConfig, newFormat);
                playlist = new MixingSampleProvider(new[] { backgroundSampleChannel, playlist });
            }

            return CreateMemoryStreamFromSample(playlist);
        }

        private static MemoryStream CreateMemoryStreamFromSample(ISampleProvider playlist)
        {
            var waveProvider = playlist.ToWaveProvider();
            byte[] buffer = new byte[16 * 1024];
            var outputStream = new MemoryStream();
            var waveFileWriter = new WaveFileWriter(outputStream, waveProvider.WaveFormat);
            {
                int read;
                while ((read = waveProvider.Read(buffer, 0, buffer.Length)) > 0)
                {
                    waveFileWriter.Write(buffer, 0, read);
                }
                waveFileWriter.Flush();
                return outputStream;
            }
        }

        private static ISampleProvider CreateBackgroundProvider(SpeechConfig speechConfig, WaveFormat newFormat)
        {
            var backgroundReader = new WaveFileReader(speechConfig.BackgroundAudioPath);
            var backgroundConversionStream = new WaveFormatConversionStream(newFormat, backgroundReader);
            var backgroundSampleChannel = new SampleChannel(backgroundConversionStream)
            {
                Volume = speechConfig.BackgroundAudioVolume
            };
            return backgroundSampleChannel;
        }

        private ISampleProvider CreatePlaylistFromText(string text, WaveFormat newFormat)
        {
            text = whitespaceRegex.Replace(text, "");
            var charProviders = text.ToCharArray().ToList()
                .Select(character => wavCharactersProvider.GetCharacterProvider(character, newFormat));
            var concatenatingPlaylist = new ConcatenatingSampleProvider(charProviders);
            return concatenatingPlaylist;
        }
    }
}
