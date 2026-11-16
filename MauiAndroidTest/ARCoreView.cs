#if ANDROID
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Opengl;
using Android.Views;
using Google.AR.Core;
using Google.AR.Core.Exceptions;
using Java.Nio;
using Microsoft.Maui.ApplicationModel;
using AColor = Android.Graphics.Color;
using Javax.Microedition.Khronos.Egl;
using Javax.Microedition.Khronos.Opengles;

namespace MauiAndroidTest;

/// <summary>
/// Native Android ARCore view.
/// ARCore owns the camera, motion tracking and plane detection.
/// A tap is ray-cast with Frame.HitTest and the resulting Anchor is rendered
/// as a real 3D object in ARCore world coordinates.
/// </summary>
public sealed class ARCoreView : GLSurfaceView
{
    private readonly ARRenderer renderer;
    private Session? session;
    private bool resumed;
    private string selectedObject = "Chair";

    public event EventHandler<string>? StatusChanged;

    public ARCoreView() : this(Platform.AppContext)
    {
    }

    public ARCoreView(Context context) : base(context)
    {
        SetEGLContextClientVersion(2);

        renderer = new ARRenderer(this);
        SetRenderer(renderer);
        RenderMode = Rendermode.Continuously;

        Touch += OnTouch;
    }

    public void ResumeAR()
    {
        if (resumed)
            return;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                var permission = await Permissions.RequestAsync<Permissions.Camera>();
                if (permission != PermissionStatus.Granted)
                {
                    Report("Kaamera luba puudub.");
                    return;
                }

                if (session == null)
                    CreateSession();

                session?.Resume();
                renderer.Session = session;
                resumed = true;
                RequestRender();
                Report("ARCore aktiivne. Liiguta telefoni, kuni pind leitakse.");
            }
            catch (UnavailableArcoreNotInstalledException)
            {
                Report("Google Play services for AR ei ole seadmes paigaldatud.");
            }
            catch (UnavailableApkTooOldException)
            {
                Report("Google Play services for AR vajab uuendamist.");
            }
            catch (UnavailableSdkTooOldException)
            {
                Report("Seadme ARCore SDK on liiga vana.");
            }
            catch (Exception ex)
            {
                Report($"ARCore käivitamine ebaõnnestus: {ex.Message}");
            }
        });
    }

    public void PauseAR()
    {
        if (!resumed)
            return;

        try
        {
            session?.Pause();
        }
        catch
        {
        }

        resumed = false;
    }

    public void SetSelectedObject(string name)
    {
        selectedObject = name;
        renderer.SelectedObject = name;
        Report($"Valitud: {name}");
    }

    public void PlaceSelectedObject(string name)
    {
        selectedObject = name;
        renderer.SelectedObject = name;
        renderer.RequestPlacement();
    }

    public void RemoveSelectedObject()
    {
        renderer.RemoveAnchor();
        Report("Objekt eemaldatud.");
    }

    private Session CreateSession()
{
    var session = new Session(Platform.AppContext);

    var config = new Config(session);
    session.GetConfig(config);

    config.SetPlaneFindingMode(
        Config.PlaneFindingMode.HorizontalAndVertical);

    session.Configure(config);

    return session;
}

    private void OnTouch(object? sender, TouchEventArgs e)
    {
        if (e.Event == null || e.Event.Action != MotionEventActions.Up)
            return;

        if (!resumed || session == null)
            return;

        renderer.SetTap(e.Event.GetX(), e.Event.GetY());
        RequestRender();
        e.Handled = true;
    }

    internal void Report(string message)
    {
        StatusChanged?.Invoke(this, message);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                session?.Pause();
                session?.Close();
            }
            catch
            {
            }
        }

        base.Dispose(disposing);
    }

    private sealed class ARRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
    {
        private const float Near = 0.1f;
        private const float Far = 100f;

        private readonly ARCoreView owner;
        private int cameraTextureId;
        private int cameraProgram;
        private int cubeProgram;

        private int cameraPosition;
        private int cameraTexCoord;
        private int cameraSampler;

        private int cubePosition;
        private int cubeColor;
        private int cubeMvp;

        private readonly float[] viewMatrix = new float[16];
        private readonly float[] projectionMatrix = new float[16];
        private readonly float[] modelMatrix = new float[16];
        private readonly float[] modelViewProjection = new float[16];

        private readonly float[]?[] cubeColors = new float[]?[3];

        private float tapX = -1;
        private float tapY = -1;
        private bool pendingPlacement;
        private Anchor? anchor;
        private string selectedObject = "Chair";
        private int viewportWidth;
        private int viewportHeight;

        internal Session? Session { get; set; }

        internal string SelectedObject
        {
            get => selectedObject;
            set => selectedObject = value;
        }

        internal ARRenderer(ARCoreView owner)
        {
            this.owner = owner;
            Android.Opengl.Matrix.SetIdentityM(modelMatrix, 0);
        }

        public void OnSurfaceCreated(IGL10? gl, Javax.Microedition.Khronos.Egl.EGLConfig? config)
        {
            GLES20.GlClearColor(0f, 0f, 0f, 1f);

            cameraTextureId = CreateExternalTexture();
            cameraProgram = CreateProgram(CameraVertexShader, CameraFragmentShader);
            cubeProgram = CreateProgram(CubeVertexShader, CubeFragmentShader);

            cameraPosition = GLES20.GlGetAttribLocation(cameraProgram, "aPosition");
            cameraTexCoord = GLES20.GlGetAttribLocation(cameraProgram, "aTexCoord");
            cameraSampler = GLES20.GlGetUniformLocation(cameraProgram, "uTexture");

            cubePosition = GLES20.GlGetAttribLocation(cubeProgram, "aPosition");
            cubeColor = GLES20.GlGetUniformLocation(cubeProgram, "uColor");
            cubeMvp = GLES20.GlGetUniformLocation(cubeProgram, "uMvp");

            Session?.SetCameraTextureName(cameraTextureId);
        }

        public void OnSurfaceChanged(IGL10? gl, int width, int height)
        {
            viewportWidth = width;
            viewportHeight = height;
            GLES20.GlViewport(0, 0, width, height);

            try
            {
                Session?.SetDisplayGeometry((int)DisplayRotation.Rotation0, width, height);
            }
            catch
            {
            }
        }

        public void OnDrawFrame(IGL10? gl)
        {
            var currentSession = Session;
            if (currentSession == null)
                return;

            try
            {
                var frame = currentSession.Update();
                var camera = frame.Camera;

                GLES20.GlClear(GLES20.GlColorBufferBit | GLES20.GlDepthBufferBit);

                DrawCameraBackground();

                if (pendingPlacement && camera.TrackingState == TrackingState.Tracking)
                {
                    PlaceFromTap(frame);
                    pendingPlacement = false;
                }

                if (anchor != null && anchor.TrackingState == TrackingState.Tracking)
                    DrawAnchor(camera);
            }
            catch (Exception ex)
            {
                owner.Report($"ARCore frame viga: {ex.Message}");
            }
        }

        internal void SetTap(float x, float y)
        {
            tapX = x;
            tapY = y;
            pendingPlacement = true;
        }

        internal void RequestPlacement()
        {
            // "Place" uses the screen center. A normal tap can be used as well.
            tapX = viewportWidth * 0.5f;
            tapY = viewportHeight * 0.5f;
            pendingPlacement = true;
        }

        internal void RemoveAnchor()
        {
            try
            {
                anchor?.Detach();
                anchor?.Dispose();
            }
            catch
            {
            }

            anchor = null;
        }

        private void PlaceFromTap(Google.AR.Core.Frame frame)
        {
            try
            {
                var hits = frame.HitTest(tapX, tapY);

                foreach (var hit in hits)
                {
                    var trackable = hit.Trackable;

                    if (trackable is Plane plane &&
                        plane.IsPoseInPolygon(hit.HitPose))
                    {
                        RemoveAnchor();
                        anchor = hit.CreateAnchor();

                        owner.Report(
                            $"{selectedObject} paigutatud tuvastatud tasapinnale.");
                        return;
                    }
                }

                owner.Report("Tasapinda ei leitud. Suuna telefon põrandale või lauale.");
            }
            catch (Exception ex)
            {
                owner.Report($"Hit-test ebaõnnestus: {ex.Message}");
            }
        }

        private void DrawCameraBackground()
        {
            GLES20.GlDisable(GLES20.GlDepthTest);

            GLES20.GlUseProgram(cameraProgram);

            float[] positions =
            {
                -1f, -1f,  1f, -1f, -1f, 1f, 1f, 1f
            };

            float[] texCoords =
            {
                0f, 1f, 1f, 1f, 0f, 0f, 1f, 0f
            };

            using var positionBuffer = CreateFloatBuffer(positions);
            using var texBuffer = CreateFloatBuffer(texCoords);

            GLES20.GlEnableVertexAttribArray(cameraPosition);
            GLES20.GlVertexAttribPointer(
                cameraPosition, 2, GLES20.GlFloat, false, 0, positionBuffer);

            GLES20.GlEnableVertexAttribArray(cameraTexCoord);
            GLES20.GlVertexAttribPointer(
                cameraTexCoord, 2, GLES20.GlFloat, false, 0, texBuffer);

            GLES20.GlActiveTexture(GLES20.GlTexture0);
            GLES20.GlBindTexture(GLES11Ext.GlTextureExternalOes, cameraTextureId);
            GLES20.GlUniform1i(cameraSampler, 0);

            GLES20.GlDrawArrays(GLES20.GlTriangleStrip, 0, 4);

            GLES20.GlDisableVertexAttribArray(cameraPosition);
            GLES20.GlDisableVertexAttribArray(cameraTexCoord);
        }

        private void DrawAnchor(Google.AR.Core.Camera camera)
        {
            anchor!.Pose.ToMatrix(modelMatrix, 0);

            camera.GetViewMatrix(viewMatrix, 0);
            camera.GetProjectionMatrix(projectionMatrix, 0, Near, Far);

            Android.Opengl.Matrix.MultiplyMM(
                modelViewProjection, 0, viewMatrix, 0, modelMatrix, 0);

            Android.Opengl.Matrix.MultiplyMM(
                modelViewProjection, 0, projectionMatrix, 0, modelViewProjection, 0);

            GLES20.GlEnable(GLES20.GlDepthTest);
            GLES20.GlUseProgram(cubeProgram);

            using var vertices = CreateFloatBuffer(CubeVertices);

            GLES20.GlEnableVertexAttribArray(cubePosition);
            GLES20.GlVertexAttribPointer(
                cubePosition, 3, GLES20.GlFloat, false, 0, vertices);

            var color = GetObjectColor(selectedObject);
            GLES20.GlUniform4f(cubeColor, color[0], color[1], color[2], 1f);
            GLES20.GlUniformMatrix4fv(
                cubeMvp, 1, false, modelViewProjection, 0);

            GLES20.GlDrawArrays(GLES20.GlTriangles, 0, CubeVertices.Length / 3);

            GLES20.GlDisableVertexAttribArray(cubePosition);
        }

        private static float[] GetObjectColor(string name) =>
            name switch
            {
                "Table" => new[] { 0.55f, 0.30f, 0.12f },
                "Lamp" => new[] { 0.95f, 0.75f, 0.20f },
                _ => new[] { 0.25f, 0.55f, 0.95f }
            };

        private static int CreateExternalTexture()
        {
            var textures = new int[1];
            GLES20.GlGenTextures(1, textures, 0);

            GLES20.GlBindTexture(GLES11Ext.GlTextureExternalOes, textures[0]);
            GLES20.GlTexParameteri(
                GLES11Ext.GlTextureExternalOes,
                GLES20.GlTextureMinFilter,
                GLES20.GlLinear);
            GLES20.GlTexParameteri(
                GLES11Ext.GlTextureExternalOes,
                GLES20.GlTextureMagFilter,
                GLES20.GlLinear);
            GLES20.GlTexParameteri(
                GLES11Ext.GlTextureExternalOes,
                GLES20.GlTextureWrapS,
                GLES20.GlClampToEdge);
            GLES20.GlTexParameteri(
                GLES11Ext.GlTextureExternalOes,
                GLES20.GlTextureWrapT,
                GLES20.GlClampToEdge);

            return textures[0];
        }

        private static int CreateProgram(string vertexSource, string fragmentSource)
        {
            var vertex = CompileShader(GLES20.GlVertexShader, vertexSource);
            var fragment = CompileShader(GLES20.GlFragmentShader, fragmentSource);

            var program = GLES20.GlCreateProgram();
            GLES20.GlAttachShader(program, vertex);
            GLES20.GlAttachShader(program, fragment);
            GLES20.GlLinkProgram(program);

            var status = new int[1];
            GLES20.GlGetProgramiv(program, GLES20.GlLinkStatus, status, 0);

            if (status[0] == 0)
            {
                var log = GLES20.GlGetProgramInfoLog(program);
                GLES20.GlDeleteProgram(program);
                throw new InvalidOperationException($"OpenGL link error: {log}");
            }

            return program;
        }

        private static int CompileShader(int type, string source)
        {
            var shader = GLES20.GlCreateShader(type);
            GLES20.GlShaderSource(shader, source);
            GLES20.GlCompileShader(shader);

            var status = new int[1];
            GLES20.GlGetShaderiv(shader, GLES20.GlCompileStatus, status, 0);

            if (status[0] == 0)
            {
                var log = GLES20.GlGetShaderInfoLog(shader);
                GLES20.GlDeleteShader(shader);
                throw new InvalidOperationException($"OpenGL shader error: {log}");
            }

            return shader;
        }

        private static FloatBuffer CreateFloatBuffer(float[] values)
        {
            var buffer = ByteBuffer
                .AllocateDirect(values.Length * sizeof(float))
                .Order(ByteOrder.NativeOrder())
                .AsFloatBuffer();

            buffer.Put(values);
            buffer.Position(0);
            return buffer;
        }

        private const string CameraVertexShader = @"
        attribute vec2 aPosition;
        attribute vec2 aTexCoord;
        varying vec2 vTexCoord;

        void main() {
            gl_Position = vec4(aPosition, 0.0, 1.0);
            vTexCoord = aTexCoord;
        }";

        private const string CameraFragmentShader =
        " " +
        "#extension GL_OES_EGL_image_external : require\n" +
        "precision mediump float;\n" +
        "varying vec2 vTexCoord;\n" +
        "uniform samplerExternalOES uTexture;\n" +
        "\n" +
        "void main() {\n" +
        "    gl_FragColor = texture2D(uTexture, vTexCoord);\n" +
        "}";

        private const string CubeVertexShader = @"
        uniform mat4 uMvp;
        attribute vec3 aPosition;

        void main() {
            gl_Position = uMvp * vec4(aPosition, 1.0);
        }";

        private const string CubeFragmentShader =
        "precision mediump float;\n" +
        "uniform vec4 uColor;\n" +
        "\n" +
        "void main() {\n" +
        "    gl_FragColor = uColor;\n" +
        "}";

        private static readonly float[] CubeVertices =
        {
            // Front
            -0.08f, -0.08f,  0.08f,   0.08f, -0.08f,  0.08f,   0.08f,  0.08f,  0.08f,
            -0.08f, -0.08f,  0.08f,   0.08f,  0.08f,  0.08f,  -0.08f,  0.08f,  0.08f,

            // Back
            -0.08f, -0.08f, -0.08f,  -0.08f,  0.08f, -0.08f,   0.08f,  0.08f, -0.08f,
            -0.08f, -0.08f, -0.08f,   0.08f,  0.08f, -0.08f,   0.08f, -0.08f, -0.08f,

            // Left
            -0.08f, -0.08f, -0.08f,  -0.08f, -0.08f,  0.08f,  -0.08f,  0.08f,  0.08f,
            -0.08f, -0.08f, -0.08f,  -0.08f,  0.08f,  0.08f,  -0.08f,  0.08f, -0.08f,

            // Right
             0.08f, -0.08f, -0.08f,   0.08f,  0.08f, -0.08f,   0.08f,  0.08f,  0.08f,
             0.08f, -0.08f, -0.08f,   0.08f,  0.08f,  0.08f,   0.08f, -0.08f,  0.08f,

            // Top
            -0.08f,  0.08f, -0.08f,  -0.08f,  0.08f,  0.08f,   0.08f,  0.08f,  0.08f,
            -0.08f,  0.08f, -0.08f,   0.08f,  0.08f,  0.08f,   0.08f,  0.08f, -0.08f,

            // Bottom
            -0.08f, -0.08f, -0.08f,   0.08f, -0.08f,  0.08f,  -0.08f, -0.08f,  0.08f,
            -0.08f, -0.08f, -0.08f,   0.08f, -0.08f, -0.08f,   0.08f, -0.08f,  0.08f
        };
    }
}
#endif
