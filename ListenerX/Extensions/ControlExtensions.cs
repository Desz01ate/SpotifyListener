using Listener.Core.Framework.Players;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ListenerX.Extensions
{
    public static class ControlExtensions
    {
        public static double CalculateRelativeValue(this ProgressBar control)
        {
            double absolutePosition = Mouse.GetPosition(control).X / control.ActualWidth;
            double relativePosition = absolutePosition * control.Maximum;
            return relativePosition;
        }

        public static void BringToFront(this FrameworkElement element)
        {
            if (element == null) return;

            if (!(element.Parent is Panel parent)) return;

            var maxZ = parent.Children.OfType<UIElement>()
              .Where(x => x != element)
              .Select(x => Panel.GetZIndex(x))
              .Max();
            Panel.SetZIndex(element, maxZ + 1);
        }
    }
}
