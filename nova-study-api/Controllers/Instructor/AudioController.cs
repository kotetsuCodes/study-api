using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Web.Http;
using NAudio.Wave;
using NAudio.Lame;
using Newtonsoft.Json.Linq;

namespace nova_study_api.Controllers
{
  public class AudioController : ApiController
  {
    [HttpPost]
    public HttpResponseMessage GetSpeechFromText([FromBody] JObject requestBody)
    {
      string textInput = (string)requestBody["TextInput"];

      if (string.IsNullOrEmpty(textInput))
      {
        return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { Success = false, Errors = new string[] { "Parameter 'TextInput' is required" } });
      }

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

        using(var mp3FileWriter = new LameMP3FileWriter(outStream: mp3Stream, format: waveFormat, bitRate: bitRate))
        {
          synthWaveMemoryStream.CopyTo(mp3FileWriter);          
        }

        audioOutputBytes = mp3Stream.ToArray();
        audioOutputAsString = Convert.ToBase64String(audioOutputBytes);
      }

      return this.Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Data = audioOutputAsString });
    }    
  }
}
