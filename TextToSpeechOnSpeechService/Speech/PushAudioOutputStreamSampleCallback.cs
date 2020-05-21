using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextToSpeechOnSpeechService.Speech
{
    //source: https://github.com/Azure-Samples/cognitive-services-speech-sdk/blob/master/samples/csharp/sharedcontent/console/helper.cs


    /// <summary>
    /// Implements a custom class for PushAudioOutputStreamCallback.
    /// This is to receive the audio data when the synthesizer has produced audio data.
    /// </summary>
    public sealed class PushAudioOutputStreamSampleCallback : PushAudioOutputStreamCallback
    {
        private byte[] audioData;

        /// <summary>
        /// Constructor
        /// </summary>
        public PushAudioOutputStreamSampleCallback()
        {
            audioData = new byte[0];
        }

        /// <summary>
        /// A callback which is invoked when the synthesizer has a output audio chunk to write out
        /// </summary>
        /// <param name="dataBuffer">The output audio chunk sent by synthesizer</param>
        /// <returns>Tell synthesizer how many bytes are received</returns>
        public override uint Write(byte[] dataBuffer)
        {
            int oldSize = audioData.Length;
            Array.Resize(ref audioData, oldSize + dataBuffer.Length);
            for (int i = 0; i < dataBuffer.Length; ++i)
            {
                audioData[oldSize + i] = dataBuffer[i];
            }

            Console.WriteLine($"{dataBuffer.Length} bytes received.");

            return (uint)dataBuffer.Length;
        }

        /// <summary>
        /// A callback which is invoked when the synthesizer is about to close the stream
        /// </summary>
        public override void Close()
        {
            Console.WriteLine("Push audio output stream closed.");
        }

        /// <summary>
        /// Get the received audio data
        /// </summary>
        /// <returns>The received audio data in byte array</returns>
        public byte[] GetAudioData()
        {
            return audioData;
        }
    }
}
