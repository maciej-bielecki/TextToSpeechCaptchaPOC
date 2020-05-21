using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TextToSpeechOnSpeechService.Infrastructure;

namespace TextToSpeechOnSpeechService.Speech
{
    public class SpeechFactory : ISpeechFactory
    {
        private readonly IConfiguration configuration;
        private readonly ISsmlFactory ssmlFactory;

        public SpeechFactory(IConfiguration configuration, ISsmlFactory ssmlFactory)
        {
            this.configuration = configuration;
            this.ssmlFactory = ssmlFactory;
        }


        public async Task<MemoryStream> SpeakToMemoryStream(string text)
        {
            var speechConfig = configuration.GetSection("CognitiveSpeech").Get<CognitiveSpeechConfig>();
            var config = SpeechConfig.FromSubscription(speechConfig.SubscriptionKey, speechConfig.Region);
            config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff16Khz16BitMonoPcm);

            byte[] data = await SynthesisToByteArrayAsync(ssmlFactory.CreateSsmlForText(text), config);
            if (data == null)
                return null;
            MemoryStream ms = new MemoryStream();
            ms.Write(data);
            ms.Position = 0;

            return ms;
        }

        public async Task<MemoryStream> SpeakToMemoryStreamWithHeaderAdded(string text)
        {
            var speechConfig = configuration.GetSection("CognitiveSpeech").Get<CognitiveSpeechConfig>();
            var config = SpeechConfig.FromSubscription(speechConfig.SubscriptionKey, speechConfig.Region);
            config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff16Khz16BitMonoPcm);

            byte[] data = await SynthesisWithByteStreamToByteArrayAsync(ssmlFactory.CreateSsmlForText(text), config);
            if (data == null)
                return null;
            MemoryStream ms = new MemoryStream();
            HeaderWriter.WriteHeader(ms, data.Length, 1, 16000);
            ms.Write(data);

            ms.Position = 0;

            return ms;
        }

        public async Task<FileStream> SpeakToWaveFile(string text)
        {
            var speechConfig = configuration.GetSection("CognitiveSpeech").Get<CognitiveSpeechConfig>();
            var config = SpeechConfig.FromSubscription(speechConfig.SubscriptionKey, speechConfig.Region);
            config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff16Khz16BitMonoPcm);

            if (await SynthesisToWaveFileAsync(speechConfig.OutputFormat, ssmlFactory.CreateSsmlForText(text), config))
            {
                var fileStream = new FileStream(speechConfig.OutputFormat, FileMode.Open, FileAccess.Read);
                return fileStream;
            }
            else
            {
                return null;
            }
        }

        private async Task<byte[]> SynthesisToByteArrayAsync(string ssmlInput, SpeechConfig config)
        {
            using var synthesizer = new SpeechSynthesizer(config, null);
            using var result = await synthesizer.SpeakSsmlAsync(ssmlInput);
            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                return result.AudioData;
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }
            }
            return null;
        }

        private async Task<byte[]> SynthesisWithByteStreamToByteArrayAsync(string ssmlInput, SpeechConfig config)
        {
            var callback = new PushAudioOutputStreamSampleCallback();

            using var stream = AudioOutputStream.CreatePushStream(callback);
            using var streamConfig = AudioConfig.FromStreamOutput(stream);
            using var synthesizer = new SpeechSynthesizer(config, streamConfig);
            using var result = await synthesizer.SpeakSsmlAsync(ssmlInput);
            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                return callback.GetAudioData();
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }
            }
            return null;
        }

        private async Task<bool> SynthesisToWaveFileAsync(string filename, string ssmlInput, SpeechConfig config)
        {
            using var synthesizer = new SpeechSynthesizer(config, null);
            using var result = await synthesizer.SpeakSsmlAsync(ssmlInput);

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                using var audioDataStream = AudioDataStream.FromResult(result);
                await audioDataStream.SaveToWaveFileAsync(filename);
                return true;
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }
            }
            return false;

        }
    }
}
