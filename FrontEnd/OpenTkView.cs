using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;

namespace RevMetrix.BallSpinner.FrontEnd;

public class Game : GameWindow
{
    private int VertexBufferObject;
    private int VertexArrayObject;
    private int IndexBufferObject;
    private int ShaderProgram;

    private List<float> vertices = new();
    private List<int> indices = new();

    private Matrix4 model;
    private Matrix4 view;
    private Matrix4 projection;

    private Vector3 cameraPosition = new(0, 0, 3);
    private Vector3 cameraFront = new(0, 0, -1);
    private Vector3 cameraUp = new(0, 1, 0);

    private Vector3 lightPosition = new(2.0f, 2.0f, 2.0f);  // Light source position

    private float cameraSpeed = 0.05f; // Camera speed for movement


  
    // Rotation Variables
    private float rotationDirectionX = 1.0f;
    private float rotationDirectionY = 1.0f;
    private float rotationDirectionZ = 1.0f;

    private float rotationAngleX = 0.0f;
    private float rotationAngleY = 0.0f;
    private float rotationAngleZ = 0.0f;

    private float rotationSpeedX = 20.0f;
    private float rotationSpeedY = 15.0f;
    private float rotationSpeedZ = 10.0f;

    public Game(int width, int height, string title)
        : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title })
    { }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Enable(EnableCap.DepthTest);

        GenerateSphere(0.5f, 30, 30); // Create the sphere

        string vertexShaderSource = @"#version 330 core
                    layout (location = 0) in vec3 aPos;
                    layout (location = 1) in vec3 aColor;  // New color attribute

                    out vec3 FragPos;
                    out vec3 Normal;
                    out vec3 VertexColor;  // Passing color to the fragment shader

                    uniform mat4 model;
                    uniform mat4 view;
                    uniform mat4 projection;

                    void main()
                    {
                        FragPos = vec3(model * vec4(aPos, 1.0));
                        Normal = normalize(mat3(transpose(inverse(model))) * aPos);
                        VertexColor = aColor;  // Pass color to the fragment shader
                        gl_Position = projection * view * vec4(FragPos, 1.0);
                    }
                ";

        string fragmentShaderSource = @"#version 330 core
                    in vec3 FragPos;
                    in vec3 Normal;
                    in vec3 VertexColor;  // Received from vertex shader

                    out vec4 FragColor;

                    uniform vec3 lightPos;
                    uniform vec3 viewPos;
                    uniform vec3 lightColor;

                    void main()
                    {
                        // Ambient
                        float ambientStrength = 0.1;
                        vec3 ambient = ambientStrength * lightColor;

                        // Diffuse
                        vec3 norm = normalize(Normal);
                        vec3 lightDir = normalize(lightPos - FragPos);
                        float diff = max(dot(norm, lightDir), 0.0);
                        vec3 diffuse = diff * lightColor;

                        // Specular
                        float specularStrength = 0.5;
                        vec3 viewDir = normalize(viewPos - FragPos);
                        vec3 reflectDir = reflect(-lightDir, norm);
                        float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
                        vec3 specular = specularStrength * spec * lightColor;

                        // Final color using per-vertex color
                        vec3 result = (ambient + diffuse + specular) * VertexColor;
                        FragColor = vec4(result, 1.0);
                    }
                ";

        ShaderProgram = CreateShader(vertexShaderSource, fragmentShaderSource);
        GL.UseProgram(ShaderProgram);

        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);

        VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

        IndexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        GL.BindVertexArray(0);

        model = Matrix4.Identity;
        view = Matrix4.LookAt(cameraPosition, cameraPosition + cameraFront, cameraUp);
        projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), Size.X / (float)Size.Y, 0.1f, 100.0f);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Update rotation angles based on elapsed time
        rotationAngleX += rotationSpeedX * rotationDirectionX * (float)e.Time;
        rotationAngleY += rotationSpeedY * rotationDirectionY * (float)e.Time;
        rotationAngleZ += rotationSpeedZ * rotationDirectionZ * (float)e.Time;

        // Keep angles within 0-360 degrees
        rotationAngleX %= 360.0f;
        rotationAngleY %= 360.0f;
        rotationAngleZ %= 360.0f;

        // Create a combined rotation matrix for all three axes
        model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotationAngleX)) *
                Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotationAngleY)) *
                Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotationAngleZ));

        GL.UseProgram(ShaderProgram);

        int modelLoc = GL.GetUniformLocation(ShaderProgram, "model");
        int viewLoc = GL.GetUniformLocation(ShaderProgram, "view");
        int projLoc = GL.GetUniformLocation(ShaderProgram, "projection");
        int lightPosLoc = GL.GetUniformLocation(ShaderProgram, "lightPos");
        int viewPosLoc = GL.GetUniformLocation(ShaderProgram, "viewPos");
        int objectColorLoc = GL.GetUniformLocation(ShaderProgram, "objectColor");
        int lightColorLoc = GL.GetUniformLocation(ShaderProgram, "lightColor");

        GL.UniformMatrix4(modelLoc, false, ref model);
        GL.UniformMatrix4(viewLoc, false, ref view);
        GL.UniformMatrix4(projLoc, false, ref projection);
        GL.Uniform3(lightPosLoc, lightPosition);
        GL.Uniform3(viewPosLoc, cameraPosition);
        GL.Uniform3(objectColorLoc, new Vector3(0.8f, 0.2f, 0.2f));
        GL.Uniform3(lightColorLoc, new Vector3(1.0f, 1.0f, 1.0f));

        GL.BindVertexArray(VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();

        float deltaTime = (float)e.Time;
        if (KeyboardState.IsKeyDown(Keys.W)) cameraPosition += cameraSpeed * cameraFront;
        if (KeyboardState.IsKeyDown(Keys.S)) cameraPosition -= cameraSpeed * cameraFront;
        if (KeyboardState.IsKeyDown(Keys.A)) cameraPosition -= Vector3.Normalize(Vector3.Cross(cameraFront, cameraUp)) * cameraSpeed;
        if (KeyboardState.IsKeyDown(Keys.D)) cameraPosition += Vector3.Normalize(Vector3.Cross(cameraFront, cameraUp)) * cameraSpeed;

        // Keep view updated
        view = Matrix4.LookAt(cameraPosition, cameraPosition + cameraFront, cameraUp);
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
        projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), e.Width / (float)e.Height, 0.1f, 100.0f);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        GL.DeleteBuffer(VertexBufferObject);
        GL.DeleteBuffer(IndexBufferObject);
        GL.DeleteVertexArray(VertexArrayObject);
        GL.DeleteProgram(ShaderProgram);
    }

    private static int CreateShader(string vertexSource, string fragmentSource)
    {
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexSource);
        GL.CompileShader(vertexShader);
        CheckShaderCompilation(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentSource);
        GL.CompileShader(fragmentShader);
        CheckShaderCompilation(fragmentShader);

        int shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        return shaderProgram;
    }

    private static void CheckShaderCompilation(int shader)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Shader Compilation Error: {infoLog}");
        }
    }

    private void GenerateSphere(float radius, int latSegments, int lonSegments)
    {
        Random rand = new Random(); // For random colors

        for (int lat = 0; lat <= latSegments; lat++)
        {
            float theta = MathF.PI * lat / latSegments;
            float sinTheta = MathF.Sin(theta);
            float cosTheta = MathF.Cos(theta);

            for (int lon = 0; lon <= lonSegments; lon++)
            {
                float phi = 2.0f * MathF.PI * lon / lonSegments;
                float x = MathF.Cos(phi) * sinTheta;
                float y = cosTheta;
                float z = MathF.Sin(phi) * sinTheta;

                // Add vertex position
                vertices.Add(radius * x);
                vertices.Add(radius * y);
                vertices.Add(radius * z);

                // Assign random colors for a multicolored effect
                vertices.Add((float)rand.NextDouble()); // R
                vertices.Add((float)rand.NextDouble()); // G
                vertices.Add((float)rand.NextDouble()); // B
            }
        }

        for (int lat = 0; lat < latSegments; lat++)
        {
            for (int lon = 0; lon < lonSegments; lon++)
            {
                int first = (lat * (lonSegments + 1)) + lon;
                int second = first + lonSegments + 1;

                indices.Add(first);
                indices.Add(second);
                indices.Add(first + 1);

                indices.Add(second);
                indices.Add(second + 1);
                indices.Add(first + 1);
            }
        }
    }
}
