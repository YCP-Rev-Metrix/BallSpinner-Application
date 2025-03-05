using Microsoft.Maui.Controls;
using static RevMetrix.BallSpinner.FrontEnd.OpenGLViewHandler;

namespace RevMetrix.BallSpinner.FrontEnd;

public class OpenGLView : View
{
    public event EventHandler? RedrawRequested;

    public void RequestRedraw()
    {
        RedrawRequested?.Invoke(this, EventArgs.Empty);
    }
}
