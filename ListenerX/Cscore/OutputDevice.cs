using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using ListenerX.Classes;
using ListenerX.DSP;
using ListenerX.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerX.Cscore
{
    public sealed class OutputDevice : IDisposable
    {
        private WasapiCapture _soundIn;
        private ISoundOut _soundOut;
        private IWaveSource _source;
        private LineSpectrum _lineSpectrum;

        public string DeviceId { get; private set; }
        public static OutputDevice ActiveDevice { get; private set; }


        static OutputDevice()
        {
            var defaultDevice = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            ActiveDevice = new OutputDevice();
            ActiveDevice.Start(defaultDevice);
        }

        private OutputDevice()
        {

        }

        private void Start(MMDevice device)
        {
            Stop();
            this.DeviceId = device.DeviceID;

            try
            {
                _soundIn = new WasapiLoopbackCapture();
                _soundIn.Device = device;
                _soundIn.Initialize();

            }
            catch
            {
                _soundIn = new WasapiLoopbackCapture(100, new WaveFormat(48000, 24, 2));
                _soundIn.Device = device;
                _soundIn.Initialize();
            }
            //Our loopback capture opens the default render device by default so the following is not needed
            //_soundIn.Device = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Console);
            

            var soundInSource = new SoundInSource(_soundIn);
            var source = soundInSource.ToSampleSource().AppendSource(x => new BiQuadFilterSource(x));//.AppendSource(x => new PitchShifter(x), out _);
            //source.Filter = new LowpassFilter(source.WaveFormat.SampleRate, 4000);
            //source.Filter = new HighpassFilter(source.WaveFormat.SampleRate, 1000);
            SetupSampleSource(source);
            // We need to read from our source otherwise SingleBlockRead is never called and our spectrum provider is not populated
            byte[] buffer = new byte[_source.WaveFormat.BytesPerSecond / 2];
            soundInSource.DataAvailable += (s, aEvent) =>
            {
                int read;
                while ((read = _source.Read(buffer, 0, buffer.Length)) > 0) ;
            };


            //play the audio
            _soundIn.Start();
        }

        private void Stop()
        {
            if (_soundOut != null)
            {
                _soundOut.Stop();
                _soundOut.Dispose();
                _soundOut = null;
            }
            if (_soundIn != null)
            {
                _soundIn.Stop();
                _soundIn.Dispose();
                _soundIn = null;
            }
            if (_source != null)
            {
                _source.Dispose();
                _source = null;
            }
        }

        private void SetupSampleSource(ISampleSource aSampleSource)
        {
            const FftSize fftSize = FftSize.Fft4096;
            //create a spectrum provider which provides fft data based on some input
            var spectrumProvider = new BasicSpectrumProvider(aSampleSource.WaveFormat.Channels,
                aSampleSource.WaveFormat.SampleRate, fftSize);

            //linespectrum and voiceprint3dspectrum used for rendering some fft data
            //in oder to get some fft data, set the previously created spectrumprovider 
            _lineSpectrum = new LineSpectrum(fftSize)
            {
                SpectrumProvider = spectrumProvider,
                BarCount = AbstractKeyGrid.GetDefaultGrid().ColumnCount,
                BarSpacing = 2,
                IsXLogScale = true,
                ScalingStrategy = ScalingStrategy.Decibel
            };


            //the SingleBlockNotificationStream is used to intercept the played samples
            var notificationSource = new SingleBlockNotificationStream(aSampleSource);
            //pass the intercepted samples as input data to the spectrumprovider (which will calculate a fft based on them)
            notificationSource.SingleBlockRead += (s, a) => spectrumProvider.Add(a.Left, a.Right);

            _source = notificationSource.ToWaveSource(16);

        }

        public double[] GetSpectrums() => this._lineSpectrum.CreateSpectrumData();
        public void Dispose()
        {
            this.Stop();
        }

        /// <summary>
        /// First item is device id, second item is friendly name.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<(string, string)> GetDevices()
        {
            using var enumerator = MMDeviceEnumerator.EnumerateDevices(DataFlow.Render);
            foreach (var deviceName in enumerator.Select(x => (x.DeviceID, x.FriendlyName)))
            {
                yield return deviceName;
            }
        }

        public static void ChangeActiveDevice(string deviceId)
        {
            var device = MMDeviceEnumerator.EnumerateDevices(DataFlow.Render).SingleOrDefault(x => x.DeviceID == deviceId);
            if (device != null)
            {
                ActiveDevice.Start(device);
            }
        }
    }
}
