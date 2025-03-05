using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using OpenTK.Graphics.OpenGL;
using RevMetrix.BallSpinner.FrontEnd;
using System;

namespace RevMetrix.BallSpinner.FrontEnd;

public class OpenGLViewHandler : ViewHandler<OpenGLView, SwapChainPanel>
{
    private bool _initialized = false;
    private int _vbo; // Vertex Buffer Object

    public OpenGLViewHandler() : base(OpenGLViewMapper) { }

    public static IPropertyMapper<OpenGLView, OpenGLViewHandler> OpenGLViewMapper =
        new PropertyMapper<OpenGLView, OpenGLViewHandler>();

    // Create platform view for Windows
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
        GL.ClearColor(0.1f, 0.2f, 0.3f, 1.0f); // Set clear color to blue

        float[] vertices = {
            0.0f,  0.5f, 0.0f, // Top
            -0.5f, -0.5f, 0.0f, // Bottom left
            0.5f, -0.5f, 0.0f  // Bottom right
        };

        //GL.GenBuffers(1, out _vbo);
        //GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        //GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
    }

    private void RenderOpenGL()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.EnableVertexAttribArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        GL.DisableVertexAttribArray(0);

        // Ensure that SwapChainPanel updates
        PlatformView?.DispatcherQueue.TryEnqueue(() => PlatformView?.InvalidateMeasure());
    }
}
