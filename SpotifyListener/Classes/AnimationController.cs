using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SpotifyListener.Classes
{
    public partial class AnimationController
    {
        private readonly DoubleAnimation BorderIn = new DoubleAnimation()
        {
            From = 0.97,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(300)
        };
        private readonly DoubleAnimation BorderOut = new DoubleAnimation()
        {
            From = 1,
            To = 0.97,
            Duration = TimeSpan.FromMilliseconds(300)
        };
        private readonly DoubleAnimation Slide_Enter = new DoubleAnimation()
        {
            From = 0,
            To = -200,
            Duration = TimeSpan.FromMilliseconds(500)
        };
        private readonly DoubleAnimation Slide_Leave = new DoubleAnimation()
        {
            From = -200,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(500),
            //                AutoReverse = true
        };
        private readonly DoubleAnimation FadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(500),
        };
        private readonly DoubleAnimation FadeOut = new DoubleAnimation()
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(200)
        };
        private readonly DoubleAnimation MouseX_Enter = new DoubleAnimation()
        {
            From = 0,
            To = 150,
            Duration = TimeSpan.FromMilliseconds(0)
        };
        private readonly DoubleAnimation MouseY_Enter = new DoubleAnimation()
        {
            From = 0,
            To = -220,
            Duration = TimeSpan.FromMilliseconds(0)
        };
        private readonly DoubleAnimation MouseX_Leave = new DoubleAnimation()
        {
            From = 150,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(0)
        };
        private readonly DoubleAnimation MouseY_Leave = new DoubleAnimation()
        {
            From = -220,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(0)
        };
        private readonly Border Border;
        private readonly Label TrackLabel;
        private readonly Button SettingsLabel;
        private readonly Button ChangeDeviceButton;
        private readonly Label CurrentTimeLabel;
        private readonly Label TimeLeftLabel;
        private readonly Button BackButton;
        private readonly Button PlayButton;
        private readonly Button NextButton;
        private readonly Button MuteButton;
        private readonly ProgressBar VolumeProgress;
        private readonly ProgressBar PlayProgress;
        private readonly Image AlbumImage;
        private readonly Button MinimizeButton;
        private readonly Button CloseButton;
        private readonly Button SearchButton;
    }
    public partial class AnimationController
    {

        public AnimationController(MainWindow mainWnd)
        {
            if (mainWnd is null) throw new NullReferenceException("MainWindow instance must not be null");
            #region bind control references
            Border = mainWnd.border_Form;
            TrackLabel = mainWnd.lbl_Track;
            BackButton = mainWnd.BackPath;
            PlayButton = mainWnd.PlayPath;
            NextButton = mainWnd.NextPath;
            MuteButton = mainWnd.VolumePath;
            VolumeProgress = mainWnd.VolumeProgress;
            PlayProgress = mainWnd.PlayProgress;
            SettingsLabel = mainWnd.btn_settings;
            ChangeDeviceButton = mainWnd.btn_device;
            CurrentTimeLabel = mainWnd.lbl_CurrentTime;
            TimeLeftLabel = mainWnd.lbl_TimeLeft;
            MinimizeButton = mainWnd.btn_Minimize;
            CloseButton = mainWnd.btn_Close;
            AlbumImage = mainWnd.AlbumImage;
            SearchButton = mainWnd.btn_search;
            #endregion
            AlbumImage.BringToFront();
            var baseHeight = AlbumImage.Height;
            var baseWidth = AlbumImage.Width;

            AlbumImage.MouseEnter += delegate
            {
                AlbumImage.Width = baseWidth * 1.2;
                AlbumImage.Height = baseHeight * 1.2;
            };
            AlbumImage.MouseLeave += delegate
            {
                AlbumImage.Width = baseWidth;
                AlbumImage.Height = baseHeight;
            };
        }

        public void TransitionEnable()
        {
            var albTrf = new TranslateTransform();
            var trkTrf = new TranslateTransform();

            AlbumImage.RenderTransform = albTrf;
            TrackLabel.RenderTransform = trkTrf;

            albTrf.BeginAnimation(TranslateTransform.XProperty, Slide_Enter);
            trkTrf.BeginAnimation(TranslateTransform.XProperty, MouseX_Enter);
            trkTrf.BeginAnimation(TranslateTransform.YProperty, MouseY_Enter);

            TrackLabel.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            BackButton.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            PlayButton.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            NextButton.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            MuteButton.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            VolumeProgress.BeginAnimation(UIElement.OpacityProperty, FadeIn);

            SettingsLabel.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            ChangeDeviceButton.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            CurrentTimeLabel.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            TimeLeftLabel.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            PlayProgress.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            SearchButton.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            MinimizeButton.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            CloseButton.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            Border.BeginAnimation(UIElement.OpacityProperty, BorderIn);
        }
        public void TransitionDisable()
        {
            var albTrf = new TranslateTransform();
            var trkTrf = new TranslateTransform();

            AlbumImage.RenderTransform = albTrf;
            TrackLabel.RenderTransform = trkTrf;

            albTrf.BeginAnimation(TranslateTransform.XProperty, Slide_Leave);
            trkTrf.BeginAnimation(TranslateTransform.XProperty, MouseX_Leave);
            trkTrf.BeginAnimation(TranslateTransform.YProperty, MouseY_Leave);

            //TrackLabel.BeginAnimation(UIElement.OpacityProperty, FadeOut);
            BackButton.BeginAnimation(UIElement.OpacityProperty, FadeOut);
            PlayButton.BeginAnimation(UIElement.OpacityProperty, FadeOut);
            NextButton.BeginAnimation(UIElement.OpacityProperty, FadeOut);

            MuteButton.BeginAnimation(UIElement.OpacityProperty, FadeOut);
            VolumeProgress.BeginAnimation(UIElement.OpacityProperty, FadeOut);

            SettingsLabel.BeginAnimation(UIElement.OpacityProperty, FadeOut);
            ChangeDeviceButton.BeginAnimation(UIElement.OpacityProperty, FadeOut);
            CurrentTimeLabel.BeginAnimation(UIElement.OpacityProperty, FadeOut);
            TimeLeftLabel.BeginAnimation(UIElement.OpacityProperty, FadeOut);
            PlayProgress.BeginAnimation(UIElement.OpacityProperty, FadeOut);
            SearchButton.BeginAnimation(UIElement.OpacityProperty, FadeOut);
            MinimizeButton.BeginAnimation(UIElement.OpacityProperty, FadeOut);
            CloseButton.BeginAnimation(UIElement.OpacityProperty, FadeOut);
            Border.BeginAnimation(UIElement.OpacityProperty, BorderOut);

        }
    }
}
