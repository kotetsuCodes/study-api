using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Threading;
using System.Web;
using NAudio.Lame;
using NAudio.Wave;

namespace nova_study_api.Helpers
{
  public static class Misc
  {
    public static void CheckAddBinPath()
    {
      // find path to 'bin' folder
      var binPath = Path.Combine(new string[]
        { AppDomain.CurrentDomain.BaseDirectory, "bin" });
      // get current search path from environment
      var path = Environment.GetEnvironmentVariable("PATH") ?? "";

      // add 'bin' folder to search path if not already present
      if (!path.Split(Path.PathSeparator).Contains(binPath, StringComparer.CurrentCultureIgnoreCase))
      {
        path = string.Join(Path.PathSeparator.ToString
          (CultureInfo.InvariantCulture), new string[] { path, binPath });
        Environment.SetEnvironmentVariable("PATH", path);
      }
    }

    public static string GetBase64Audio(string textInput)
    {
      var speechAudioFormatConfig = new SpeechAudioFormatInfo(samplesPerSecond: 8000, bitsPerSample: AudioBitsPerSample.Sixteen, channel: AudioChannel.Stereo);
      var waveFormat = new WaveFormat(speechAudioFormatConfig.SamplesPerSecond, speechAudioFormatConfig.BitsPerSample, speechAudioFormatConfig.ChannelCount);
      var prompt = new PromptBuilder
      {
        Culture = CultureInfo.CreateSpecificCulture("en-US")
      };

      prompt.StartVoice(prompt.Culture);
      prompt.StartSentence();
      prompt.StartStyle(new PromptStyle()
      {
        Emphasis = PromptEmphasis.Reduced,
        Rate = PromptRate.Slow
      });
      prompt.AppendText(textInput);
      prompt.EndStyle();
      prompt.EndSentence();
      prompt.EndVoice();

      var mp3Stream = new MemoryStream();
      byte[] audioOutputBytes;
      string audioOutputAsString = null;

      using (var synthWaveMemoryStream = new MemoryStream())
      {
        var resetEvent = new ManualResetEvent(false);
        ThreadPool.QueueUserWorkItem(arg =>
        {
          try
          {
            var siteSpeechSynth = new SpeechSynthesizer();
            siteSpeechSynth.SetOutputToAudioStream(synthWaveMemoryStream, speechAudioFormatConfig);
            siteSpeechSynth.Speak(prompt);

          }
          finally
          {
            resetEvent.Set();
          }

        });
        WaitHandle.WaitAll(new WaitHandle[] { resetEvent });
        var bitRate = (speechAudioFormatConfig.AverageBytesPerSecond * 8);

        synthWaveMemoryStream.Position = 0;

        using (var mp3FileWriter = new LameMP3FileWriter(outStream: mp3Stream, format: waveFormat, bitRate: bitRate))
        {
          synthWaveMemoryStream.CopyTo(mp3FileWriter);
        }

        audioOutputBytes = mp3Stream.ToArray();
        audioOutputAsString = $"data:audio/mp3;base64,{Convert.ToBase64String(audioOutputBytes)}";
      }

      return audioOutputAsString;
    }
  }
}