using System.Drawing.Text;
using System.Windows.Forms;

namespace RealEstateCRMWinForms.Controls
{
    // Label with ClearType smoothing for crisper text
    public class SmoothLabel : Label
    {
        public SmoothLabel()
        {
            // Use GDI+ rendering so TextRenderingHint applies
            UseCompatibleTextRendering = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Apply ClearType smoothing for better readability
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            base.OnPaint(e);
        }
    }
}