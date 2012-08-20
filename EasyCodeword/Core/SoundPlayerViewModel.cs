﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace EasyCodeword.Core
{
    public class SoundPlayerViewModel
    {
        #region 变量

        private static SoundPlayerViewModel _instance = new SoundPlayerViewModel();

        MediaPlayer _soundPlayer;

        List<string> _files;

        int _index = -1;

        #endregion

        #region 属性

        public static SoundPlayerViewModel Instance { get { return _instance; } }

        #endregion

        #region 构造方法

        private SoundPlayerViewModel()
        {

        }

        ~SoundPlayerViewModel()
        {
            try
            {
                if (null != _soundPlayer)
                {
                    _soundPlayer.Close();
                    _soundPlayer = null;
                }
            }
            catch
            {
            }
        }

        #endregion

        #region 私有方法

        private void AutoPlay()
        {
            _index++;
            if (null != _files)
            {
                var index = _index % _files.Count;
                if (index > 0
                    && index < _files.Count)
                {
                    if (File.Exists(_files[_index % _files.Count]))
                    {
                        _soundPlayer.Open(new Uri(_files[_index % _files.Count], UriKind.Relative));
                        _soundPlayer.Play();
                    }
                    else
                    {
                        AutoPlay();
                    }
                }
            }
        }

        private void SoundPlayer_MediaEnded(object sender, EventArgs e)
        {
            AutoPlay();
        }

        private void SoundPlayer_MediaFailed(object sender, ExceptionEventArgs e)
        {
            AutoPlay();
        }

        #endregion

        #region 公共方法

        public void Play(string folder)
        {
            if(Directory.Exists(folder))
            {
                _files = Directory.GetFiles(folder, "*.mp3").Union(Directory.GetFiles(folder, "*.wav")).ToList();
                _index = -1;
                if (_files.Count > 0)
                {
                    if (null == _soundPlayer)
                    {
                        _soundPlayer = new MediaPlayer();
                        _soundPlayer.MediaEnded += SoundPlayer_MediaEnded;
                        _soundPlayer.MediaFailed += SoundPlayer_MediaFailed;
                    }
                    else
                    {
                        _soundPlayer.Stop();
                    }
                }
            }
        }

        public void Stop()
        {
            try
            {
                if (null != _soundPlayer)
                {
                    _soundPlayer.MediaEnded -= SoundPlayer_MediaEnded;
                    _soundPlayer.MediaFailed -= SoundPlayer_MediaFailed;
                    _soundPlayer.Close();
                    _soundPlayer = null;
                }
            }
            catch { }
        }

        #endregion
    }
}
