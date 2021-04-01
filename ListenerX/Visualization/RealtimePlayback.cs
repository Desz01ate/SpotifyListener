using System;
using System.Collections.Generic;
using ListenerX.Classes;
using NAudio.CoreAudioApi;
using NAudio.Dsp;
using NAudio.Wave;

namespace ListenerX.Visualization
{
    public class RealTimePlayback : IDisposable
    {
        public delegate void SelectDeviceChanged(RealTimePlayback device);
        const int MAXIMUM_FREQ = 20_000;
        const int MINIMUM_FREQ = 20;
        const double MinDbValue = -90;
        const double MaxDbValue = 0;
        const double DbScale = MaxDbValue - MinDbValue;
        const int ScaleFactorLinear = 9;
        const int ScaleFactorSqr = 2;

        private IWaveIn _capture;
        private readonly ISettings settings;
        private object _lock;
        private int _fftPos;
        private int _fftLength;
        private Complex[] _fftBuffer;
        private float[] _lastFftBuffer;
        private bool _fftBufferAvailable;
        private int _m;
        private readonly int _fftSize = 4096;
        private int _maxFftIndex => this._fftSize / 2 - 1;

        private int _barCount;
        private double[] _backedDataPoints;

        public int BarCount
        {
            get { return _barCount; }
            private set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                if (_barCount == value)
                    return;
                _barCount = value;
                _backedDataPoints = new double[value];
                UpdateFrequencyMapping();
            }
        }

        private int _maximumFrequencyIndex, _minimumFrequencyIndex;
        private int[] _spectrumIndexMax, _spectrumLogScaleIndexMax;

        public event SelectDeviceChanged DeviceChanged;

        public RealTimePlayback(ISettings settings) : this(new WasapiLoopbackCapture(WasapiLoopbackCapture.GetDefaultLoopbackCaptureDevice()), settings)
        {

        }

        private RealTimePlayback(IWaveIn captureDevice, ISettings settings)
        {
            this._lock = new object();

            this._capture = captureDevice;
            this.settings = settings;
            this._capture.DataAvailable += this.DataAvailable;

            this._fftLength = 2048; // 44.1kHz.
            this._m = (int)Math.Log(this._fftLength, 2.0);
            this._fftBuffer = new Complex[this._fftLength];
            this._lastFftBuffer = new float[this._fftLength];
            ActivePlayback = this;
        }

        public WaveFormat Format
        {
            get
            {
                return this._capture.WaveFormat;
            }
        }

        private float[] ConvertByteToFloat(byte[] array, int length)
        {
            int samplesNeeded = length / 4;
            float[] floatArr = new float[samplesNeeded];

            for (int i = 0; i < samplesNeeded; i++)
            {
                floatArr[i] = BitConverter.ToSingle(array, i * 4);
            }

            return floatArr;
        }

        private void DataAvailable(object sender, WaveInEventArgs e)
        {
            // Convert byte[] to float[].
            float[] data = ConvertByteToFloat(e.Buffer, e.BytesRecorded);

            // For all data. Skip right channel on stereo (i += this.Format.Channels).
            for (int i = 0; i < data.Length; i += this.Format.Channels)
            {
                this._fftBuffer[_fftPos].X = (float)(data[i] * FastFourierTransform.HannWindow(_fftPos, _fftLength));
                this._fftBuffer[_fftPos].Y = 0;
                this._fftPos++;

                if (this._fftPos >= this._fftLength)
                {
                    this._fftPos = 0;

                    // NAudio FFT implementation.
                    FastFourierTransform.FFT(true, this._m, this._fftBuffer);

                    // Copy to buffer.
                    lock (this._lock)
                    {
                        for (int c = 0; c < this._fftLength; c++)
                        {
                            float amplitude = (float)Math.Sqrt(this._fftBuffer[c].X * this._fftBuffer[c].X + this._fftBuffer[c].Y * this._fftBuffer[c].Y);
                            this._lastFftBuffer[c] = amplitude;
                        }

                        this._fftBufferAvailable = true;
                    }
                }
            }
        }

        public void Start()
        {
            this._capture.StartRecording();
        }

        public void Stop()
        {
            this._capture.StopRecording();
        }

        public bool GetFFTData(float[] fftDataBuffer)
        {
            lock (this._lock)
            {
                // Use last available buffer.
                if (this._fftBufferAvailable)
                {
                    this._lastFftBuffer.CopyTo(fftDataBuffer, 0);
                    this._fftBufferAvailable = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool GetFrequency(int n, double amplitudeMultiplier, out double[] data)
        {
            var fftBuffer = new float[this._fftLength];
            if (!GetFFTData(fftBuffer))
            {
                data = null;
                return false;
            }
            if (BarCount != n)
            {
                BarCount = n;
            }

            double value0 = 0, value = 0;
            double lastValue = 0;
            double actualMaxValue = 100;
            int spectrumPointIndex = 0;

            for (int i = _minimumFrequencyIndex; i <= _maximumFrequencyIndex; i++)
            {
                value0 = (ScalingStrategy)this.settings.RgbRenderScalingStrategy switch
                {
                    ScalingStrategy.Decibel => (((20 * Math.Log10(fftBuffer[i])) - MinDbValue) / DbScale) * actualMaxValue,
                    ScalingStrategy.Linear => (fftBuffer[i] * ScaleFactorLinear) * actualMaxValue,
                    ScalingStrategy.Sqrt => ((Math.Sqrt(fftBuffer[i])) * ScaleFactorSqr) * actualMaxValue,
                    _ => throw new NotImplementedException()
                };

                bool recalc = true;

                value = Math.Max(0, Math.Max(value0, value));

                var spectrumSample = _spectrumLogScaleIndexMax;
                while (spectrumPointIndex <= _spectrumIndexMax.Length - 1 &&
                       i == spectrumSample[spectrumPointIndex])
                {
                    if (!recalc)
                        value = lastValue;

                    if (value > 100)
                        value = 100;

                    var useAverage = this.settings.RgbRenderAverageSpectrum;
                    if (useAverage && spectrumPointIndex > 0)
                        value = (lastValue + value) / 2.0;

                    //dataPoints.Add(new SpectrumPointData { SpectrumPointIndex = spectrumPointIndex, Value = value });
                    //_backedDataPoints.Add(value * amplitudeMultiplier);
                    // code smell it is, i know but memory cost are at stake!
                    _backedDataPoints[spectrumPointIndex] = Math.Min(value * amplitudeMultiplier, 100);

                    lastValue = value;
                    value = 0.0;
                    spectrumPointIndex++;
                    recalc = false;
                }
            }

            data = _backedDataPoints;
            return true;
        }
        public int GetFftBandIndex(float frequency)
        {
            var fftSize = _fftSize;
            double f = this._capture.WaveFormat.SampleRate / 2.0;
            // ReSharper disable once PossibleLossOfFraction
            return (int)((frequency / f) * (fftSize / 2));
        }
        private void UpdateFrequencyMapping()
        {
            _maximumFrequencyIndex = Math.Min(GetFftBandIndex(MAXIMUM_FREQ) + 1, _maxFftIndex);
            _minimumFrequencyIndex = Math.Min(GetFftBandIndex(MINIMUM_FREQ), _maxFftIndex);

            int actualResolution = BarCount;

            int indexCount = _maximumFrequencyIndex - _minimumFrequencyIndex;
            double linearIndexBucketSize = Math.Round(indexCount / (double)actualResolution, 3);

            _spectrumIndexMax = CheckBuffer(_spectrumIndexMax, actualResolution, true);
            _spectrumLogScaleIndexMax = CheckBuffer(_spectrumLogScaleIndexMax, actualResolution, true);

            double maxLog = Math.Log(actualResolution, actualResolution);
            for (int i = 1; i < actualResolution; i++)
            {
                int logIndex =
                    (int)((maxLog - Math.Log((actualResolution + 1) - i, (actualResolution + 1))) * indexCount) +
                    _minimumFrequencyIndex;

                _spectrumIndexMax[i - 1] = _minimumFrequencyIndex + (int)(i * linearIndexBucketSize);
                _spectrumLogScaleIndexMax[i - 1] = logIndex;
            }

            if (actualResolution > 0)
            {
                _spectrumIndexMax[_spectrumIndexMax.Length - 1] =
                    _spectrumLogScaleIndexMax[_spectrumLogScaleIndexMax.Length - 1] = _maximumFrequencyIndex;
            }
        }

        public void Dispose()
        {
            this._capture?.Dispose();
        }

        private static T[] CheckBuffer<T>(T[] inst, long size, bool exactSize = false)
        {
            if (inst == null || (!exactSize && inst.Length < size) || (exactSize && inst.Length != size))
                return new T[size];
            return inst;
        }

        public static IEnumerable<(bool IsDefaultDevice, MMDevice Device)> EnumerateLoopbackDevices()
        {
            using var deviceEnumerator = new MMDeviceEnumerator();
            var defaultDeviceId = WasapiLoopbackCapture.GetDefaultLoopbackCaptureDevice().ID;
            foreach (var device in deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                yield return (defaultDeviceId == device.ID, device);
            }
        }

        public static RealTimePlayback ActivePlayback { get; private set; }
        public static void InitLoopbackCapture(MMDevice device, ISettings settings)
        {
            var oldDevice = ActivePlayback;
            var playback = new RealTimePlayback(new WasapiLoopbackCapture(device), settings);
            playback.Start();
            oldDevice?.Dispose();
        }
    }
}
