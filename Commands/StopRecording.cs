using Audio_Recorder.ViewModel;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Audio_Recorder.Commands
{
    class StopRecording: ICommand
    {
        private RecorderViewModel _recorderViewModel { get; }
        private WaveFileWriter _waveFileWriter;
        public event EventHandler? CanExecuteChanged;

        public StopRecording(RecorderViewModel recorderViewModel)
        {
            _recorderViewModel = recorderViewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return _recorderViewModel.IsRecording;
        }

        public void Execute(object? parameter)
        {
            if (_recorderViewModel.IsRecording)
            {
                WasapiCapture wasapiCapture = _recorderViewModel.GetWasapiCapture();
                wasapiCapture.StopRecording();
                _recorderViewModel.IsRecording = false;
                _recorderViewModel.StopTimer();
            }
        }
    }
}
