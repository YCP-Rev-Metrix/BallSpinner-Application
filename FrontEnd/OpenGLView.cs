using Microsoft.Maui.Controls;
using System;

namespace RevMetrix.BallSpinner.FrontEnd;

/// <summary>
/// A custom MAUI view for OpenGL rendering.
/// </summary>
public class OpenGLView : View
{
    public event EventHandler? RedrawRequested;

    /// <summary>
    /// Triggers a redraw of the OpenGL context.
    /// </summary>
    public void RequestRedraw()
    {
        RedrawRequested?.Invoke(this, EventArgs.Empty);
    }
}
