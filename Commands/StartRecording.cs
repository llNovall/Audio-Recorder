using Accessibility;
using Audio_Recorder.ViewModel;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Audio_Recorder.Commands
{
    class StartRecording : ICommand
    { 
        private RecorderViewModel _recorderViewModel { get; }
        private WaveFileWriter _waveFileWriter;
        public event EventHandler? CanExecuteChanged;

        public StartRecording(RecorderViewModel recorderViewModel)
        {
            _recorderViewModel = recorderViewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return !_recorderViewModel.IsRecording;
        }

        public void Execute(object? parameter)
        {
            if (!_recorderViewModel.IsRecording)
            {
                try
                {
                    WasapiCapture wasapiCapture = _recorderViewModel.GetWasapiCapture();
                    wasapiCapture.DataAvailable += CaptureOnDataAvailable;
                    wasapiCapture.StartRecording();
                    _recorderViewModel.IsRecording= true;
                    wasapiCapture.RecordingStopped += WasapiCapture_RecordingStopped;
                    _recorderViewModel.StartTimer();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private void WasapiCapture_RecordingStopped(object? sender, StoppedEventArgs e)
        {
            _waveFileWriter.Dispose();
            _waveFileWriter = null;

            _recorderViewModel.DisposeWasapiCapture();
        }

        private void CaptureOnDataAvailable(object? sender, WaveInEventArgs waveInEventArgs)
        {
            if (_waveFileWriter == null)
            {
                _waveFileWriter = new WaveFileWriter(_recorderViewModel.FileName + ".wav", _recorderViewModel.GetWasapiCapture().WaveFormat);
            }

            _waveFileWriter.Write(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);
        }

    }
}
