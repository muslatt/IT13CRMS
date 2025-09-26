using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Utils
{
    public static class ControlExtensions
    {
        // Enables DoubleBuffered on controls like FlowLayoutPanel where the property is protected
        public static void EnableDoubleBuffering(Control control)
        {
            try
            {
                var doubleBufferedProp = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                doubleBufferedProp?.SetValue(control, true, null);
            }
            catch { /* best effort */ }
        }

        // Optional: suspend/resume drawing via WM_SETREDRAW to reduce flicker during bulk updates
        private const int WM_SETREDRAW = 0x000B;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public static void SuspendDrawing(this Control control)
        {
            if (control.IsHandleCreated)
            {
                SendMessage(control.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
            }
        }

        public static void ResumeDrawing(this Control control, bool refresh = true)
        {
            if (control.IsHandleCreated)
            {
                SendMessage(control.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
                if (refresh)
                {
                    control.Invalidate();
                    control.Update();
                }
            }
        }
    }
}

