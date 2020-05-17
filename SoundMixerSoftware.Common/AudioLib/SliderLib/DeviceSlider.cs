﻿using System;
using System.Threading;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace SoundMixerSoftware.Common.AudioLib.SliderLib
{
    public class DeviceSlider : IVirtualSlider
    {

        #region Private Fields

        private readonly AudioEndpointVolume _endpoint;

        private float _lastVolume = 0;
        private bool _lastMute = false;

        private Task _refreshTask;
        private readonly CancellationTokenSource _token = new CancellationTokenSource();

        #endregion

        #region Public Properties

        public float Volume { get; set; } = float.NaN;
        public bool IsMute { get; set; } = false;
        public bool IsMasterVolume => true;
        public SliderType SliderType { get; }

        #endregion

        #region Constructor

        public DeviceSlider(MMDevice device)
        {
            _endpoint = device.AudioEndpointVolume;
            SliderType = device.DataFlow == DataFlow.Capture ? SliderType.MASTER_CAPTURE : SliderType.MASTER_RENDER;
            _lastVolume = _endpoint.MasterVolumeLevelScalar;
            _lastMute = _endpoint.Mute;
            StartRefreshTask(10, _token.Token);
        }

        #endregion

        #region Private Methods

        private void StartRefreshTask(int interval, CancellationToken token)
        {
            _refreshTask = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (_lastVolume != Volume)
                    {
                        _endpoint.MasterVolumeLevelScalar = Volume;
                        _lastVolume = Volume;
                    }

                    if (_lastMute != IsMute)
                    {
                        _endpoint.Mute = IsMute;
                        _lastMute = IsMute;
                    }
                    await Task.Delay(interval, token);
                }
            }, token);
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            _token.Cancel();
        }

        #endregion

    }
}