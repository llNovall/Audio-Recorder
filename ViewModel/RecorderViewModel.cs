using Audio_Recorder.Commands;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Audio_Recorder.ViewModel
{
    class RecorderViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isRecording;
        private string _fileName = "Recording";
        private IEnumerable<MMDevice> _captureDevices;
        private MMDevice _selectedCaptureDevice;
        private WasapiCapture _wasapiCapture;
        private TimeSpan _startTime;
        private string _recordingTime;
        private DispatcherTimer _timer;

        public string RecordingTime 
        { get => _recordingTime;
            set
            {
                if(_recordingTime != value)
                {
                    _recordingTime = value;
                    OnPropertyChanged("RecordingTime");
                }
            } 
        }

        public StartRecording StartRecording { get; }
        public StopRecording StopRecording { get; }
        
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    OnPropertyChanged(name: "FileName");
                }
            }
        }

        public bool IsRecording
        {
            get { return _isRecording; }
            set
            {
                _isRecording = value;
                OnPropertyChanged(name: "IsRecording");
            }
        }

        public IEnumerable<MMDevice> CaptureDevices
        {
            get { return _captureDevices; }
            set { 
                _captureDevices = value;
                OnPropertyChanged(name: "CaptureDevices");
            }
        }

        public MMDevice SelectedCaptureDevice
        {
            get { return _selectedCaptureDevice; }
            set
            {
                if(_selectedCaptureDevice != value)
                {
                    _selectedCaptureDevice = value;
                    OnPropertyChanged(name: "SelectedCaptureDevice");
                }
            }
        }

        public RecorderViewModel()
        {
            var enumerate = new MMDeviceEnumerator();
            CaptureDevices = enumerate.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            MMDevice defaultDevice = enumerate.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
            
            if (defaultDevice != null)
            {
                SelectedCaptureDevice = CaptureDevices.FirstOrDefault(c => c.ID == defaultDevice.ID);
            }

            StartRecording = new StartRecording(this);
            StopRecording= new StopRecording(this);

            RecordingTime = "00:00:00";
        }

        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public WasapiCapture GetWasapiCapture()
        {
            if (_wasapiCapture != null)
                return _wasapiCapture;

            _wasapiCapture = new WasapiCapture(SelectedCaptureDevice);
            _wasapiCapture.WaveFormat = new WaveFormat(rate: 48000, bits: 32, channels: 1);
            _wasapiCapture.ShareMode = AudioClientShareMode.Shared;

            return _wasapiCapture;
        }

        public void DisposeWasapiCapture()
        {
            if(_wasapiCapture != null)
            {
                _wasapiCapture.Dispose();
                _wasapiCapture = null;
            }
        }


        public void StartTimer()
        {
            _startTime = DateTime.Now.TimeOfDay;

            if(_timer == null)
                _timer = new DispatcherTimer(
                    interval: TimeSpan.FromMicroseconds(500), 
                    priority: DispatcherPriority.Background, 
                    callback: UpdateTime, 
                    dispatcher: Dispatcher.CurrentDispatcher);

            _timer.Start();
        }

        public void StopTimer()
        {
            if(_timer != null)
                _timer.Stop();
        }

        private void UpdateTime(object? sender, EventArgs e)
        {
            TimeSpan curTime = DateTime.Now.TimeOfDay - _startTime;
            RecordingTime = curTime.ToString("hh\\:mm\\:ss");
        }
    }
}
