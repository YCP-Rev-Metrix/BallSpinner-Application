using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using OpenTK.Graphics.OpenGL;
using System;

namespace RevMetrix.BallSpinner.FrontEnd;

public class OpenGLViewHandler : ViewHandler<OpenGLView, SwapChainPanel>
{
    private bool _initialized = false;

    public OpenGLViewHandler() : base(OpenGLViewMapper) { }

    public static IPropertyMapper<OpenGLView, OpenGLViewHandler> OpenGLViewMapper =
        new PropertyMapper<OpenGLView, OpenGLViewHandler>();

    protected override SwapChainPanel CreatePlatformView()
    {
        return new SwapChainPanel();
    }

    //protected override void ConnectHandler(OpenGLView view)
    //{
    //    base.ConnectHandler(view);
    //    view.RedrawRequested += OnRedrawRequested;
    //}

    //protected override void DisconnectHandler(OpenGLView view)
    //{
    //    base.DisconnectHandler(view);
    //    view.RedrawRequested -= OnRedrawRequested;
    //}

    private void OnRedrawRequested(object? sender, EventArgs e)
    {
        if (!_initialized)
        {
            InitializeOpenGL();
            _initialized = true;
        }

        RenderOpenGL();
    }

    private void InitializeOpenGL()
    {
        GL.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);
    }

    private void RenderOpenGL()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        PlatformView?.DispatcherQueue.TryEnqueue(() => PlatformView?.InvalidateMeasure());
    }
}
