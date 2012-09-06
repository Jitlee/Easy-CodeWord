using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using EasyCodeword.Utilities;

namespace EasyCodeword.Core
{
    public class SoundPlayerViewModel : EntityObject
    {
        #region 变量

        private ILogger _logger = LoggerFactory.GetLogger(typeof(SoundPlayerViewModel).FullName);

        private static SoundPlayerViewModel _instance = new SoundPlayerViewModel();

        MediaPlayer _soundPlayer;

        List<string> _files;

        int _index = -1;

        private string _playingMusic = string.Empty;

        #endregion

        #region 属性

        public static SoundPlayerViewModel Instance { get { return _instance; } }

        public bool IsPlaying { get { return null != _soundPlayer; } }

        public bool IsShowNowPlaying { get { return IsPlaying && SettingViewModel.Instance.IsShowNowPlaying; } }

        public string PlayingMusic
        {
            get { return _playingMusic; }
            set { _playingMusic = value; RaisePropertyChanged("PlayingMusic"); }
        }

        #endregion

        #region 构造方法

        private SoundPlayerViewModel()
        {

        }

        ~SoundPlayerViewModel()
        {
            if (null != _soundPlayer)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {

                    try
                    {
                        _soundPlayer.Close();
                        _soundPlayer = null;
                    }
                    catch(Exception ex)
                    {
                        _logger.Error("[~SoundPlayerViewModel] Exception : {0}", ex.Message);
                    }
                }));
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
                        LoadSongProperty(file);
                        try
                        {
                            _soundPlayer.Open(new Uri(file, UriKind.Relative));
                            _soundPlayer.Play();
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("[AutoPlay] Exception : {0}", ex.Message);
                        }
                    }
                    else
                    {
                        AutoPlay();
                    }
                }
            }
        }

        private void LoadSongProperty(string fileName)
        {
            ThreadPool.QueueUserWorkItem((state) => {
                var songInfo = SongHelper.GetMp3Info(fileName);
                var title = string.Format("{0}-{1}", songInfo.Artist, songInfo.Title);
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    PlayingMusic = title;
                }));
            });
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
                        RaisePropertyChanged("IsShowNowPlaying");
                        return;
                    }
                }
                Stop();
            }
            catch (Exception ex)
            {
                _logger.Debug("[Play] Exception : {0}", ex.Message);
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
                RaisePropertyChanged("IsShowNowPlaying");
            }
            catch (Exception ex)
            {
                _logger.Debug("[Stop] Exception : {0}", ex.Message);
            }
        }

        #endregion
    }
}
