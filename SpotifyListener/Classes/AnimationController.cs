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
        private Label TrackLabel { get; }
        private Label SettingsLabel { get; }
        private Label ChangeDeviceLabel { get; }
        private Label CurrentTimeLabel { get; }
        private Label TimeLeftLabel { get; }
        private PathButton BackButton { get; }
        private PathButton PlayButton { get; }
        private PathButton NextButton { get; }
        private PathButton MuteButton { get; }
        private ProgressBar VolumeProgress { get; }
        private ProgressBar PlayProgress { get; }
        private Image AlbumImage { get; }
        private RectangleGeometry AlbumImageRectangle { get; }
        private Button MinimizeButton { get; }
        private Button CloseButton { get; }
    }
    public partial class AnimationController
    {

        public AnimationController(MainWindow mainWnd)
        {
            if (mainWnd is null) throw new NullReferenceException("MainWindow instance must not be null");
            #region bind control references
            TrackLabel = mainWnd.lbl_Track;
            BackButton = mainWnd.BackPath;
            PlayButton = mainWnd.PlayPath;
            NextButton = mainWnd.NextPath;
            MuteButton = mainWnd.VolumePath;
            VolumeProgress = mainWnd.VolumeProgress;
            PlayProgress = mainWnd.PlayProgress;
            SettingsLabel = mainWnd.lbl_settings;
            ChangeDeviceLabel = mainWnd.lbl_change_device;
            CurrentTimeLabel = mainWnd.lbl_CurrentTime;
            TimeLeftLabel = mainWnd.lbl_TimeLeft;
            MinimizeButton = mainWnd.btn_Minimize;
            CloseButton = mainWnd.btn_Close;
            AlbumImage = mainWnd.AlbumImage;
            AlbumImageRectangle = mainWnd.AlbumImageRectangle;
            #endregion
            AlbumImage.BringToFront();
            var baseHeight = AlbumImage.Height;
            var baseWidth = AlbumImage.Width;
            var NormalRect = new Rect { X = 0, Y = 0, Width = 250, Height = 250 };
            var EnlargeRect = new Rect { X = 0, Y = 0, Width = 250 * 1.2, Height = 250 * 1.2 };

            AlbumImage.MouseEnter += delegate
            {
                AlbumImage.Width = baseWidth * 1.2;
                AlbumImage.Height = baseHeight * 1.2;
                AlbumImageRectangle.Rect = EnlargeRect;
            };
            AlbumImage.MouseLeave += delegate
            {
                AlbumImage.Width = baseWidth;
                AlbumImage.Height = baseHeight;
                AlbumImageRectangle.Rect = NormalRect;
            };
        }

        public void TransitionEnable()
        {
            #region exceptional visibility case              
            SettingsLabel.Visibility = Visibility.Visible;
            ChangeDeviceLabel.Visibility = Visibility.Visible;
            CurrentTimeLabel.Visibility = Visibility.Visible;
            TimeLeftLabel.Visibility = Visibility.Visible;
            PlayProgress.Visibility = Visibility.Visible;
            MinimizeButton.Visibility = Visibility.Visible;
            CloseButton.Visibility = Visibility.Visible;
            #endregion

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
            ChangeDeviceLabel.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            CurrentTimeLabel.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            TimeLeftLabel.BeginAnimation(UIElement.OpacityProperty, FadeIn);
            PlayProgress.BeginAnimation(UIElement.OpacityProperty, FadeIn);
        }
        public void TransitionDisable()
        {
            MinimizeButton.Visibility = Visibility.Hidden;
            CloseButton.Visibility = Visibility.Hidden;

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
            ChangeDeviceLabel.BeginAnimation(UIElement.OpacityProperty, FadeOut);
            CurrentTimeLabel.BeginAnimation(UIElement.OpacityProperty, FadeOut);
            TimeLeftLabel.BeginAnimation(UIElement.OpacityProperty, FadeOut);
            PlayProgress.BeginAnimation(UIElement.OpacityProperty, FadeOut);
        }
    }
}
