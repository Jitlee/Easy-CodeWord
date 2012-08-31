using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using EasyCodeword.Utilities;

namespace EasyCodeword.Core
{
    public class SoundPlayerViewModel
    {
        #region 变量

        private ILogger _logger = LoggerFactory.GetLogger(typeof(SoundPlayerViewModel).FullName);

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
                if (index > -1
                    && index < _files.Count)
                {
                    var file = _files[index];
                    if (File.Exists(file))
                    {
                        _soundPlayer.Open(new Uri(file, UriKind.Relative));
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
            try
            {
                if (Directory.Exists(folder))
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
                        AutoPlay();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("[Play] Exception : {0}", ex.Message);
            }
        }

        public bool IsPlaying { get { return null != _soundPlayer; } }

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
            catch (Exception ex)
            {
                _logger.Debug("[Stop] Exception : {0}", ex.Message);
            }
        }

        #endregion
    }
}
