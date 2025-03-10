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

    // Create the SwapChainPanel for rendering
    protected override SwapChainPanel CreatePlatformView()
    {
        return new SwapChainPanel();
    }

    // ✅ Correct method for setup
    protected override void ConnectHandler(SwapChainPanel platformView)
    {
        base.ConnectHandler(platformView);
        if (VirtualView != null)
            VirtualView.RedrawRequested += OnRedrawRequested;
    }

    // ✅ Correct method for cleanup
    protected override void DisconnectHandler(SwapChainPanel platformView)
    {
        base.DisconnectHandler(platformView);
        if (VirtualView != null)
            VirtualView.RedrawRequested -= OnRedrawRequested;
    }

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
        GL.ClearColor(1.0f, 0.2f, 0.3f, 1.0f); // Set background color
    }

    private void RenderOpenGL()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        PlatformView?.DispatcherQueue.TryEnqueue(() => PlatformView?.InvalidateMeasure());
    }
}
