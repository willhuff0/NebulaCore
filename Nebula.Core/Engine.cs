﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using Nebula.Graphics;

namespace Nebula.Core;

public static unsafe class Engine
{
    //public static Project? ActiveProject { get; set; }
    //public static RuntimeScene? ActiveScene { get; set; }
    
    public static EngineInformation EngineInfo { get; private set; } = null!;
    public static WindowInformation WindowInfo { get; private set; } = null!;
    
    #region Callbacks
    
    private static void GlfwErrorCallback(int errorCode, string description)
    {
        Console.WriteLine($"GLFW error {errorCode}: {description}");
    }
    
    private static void GlfwFramebufferSizeCallback(void* window, int width, int height)
    {
        WindowInfo = new WindowInformation((width, height));
        GL.Viewport(0, 0, width, height);
    }
    
    #endregion

    public static void Initialize()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Glfw.InitHint(Glfw.ANGLE_PLATFORM_TYPE, Glfw.ANGLE_PLATFORM_TYPE_VULKAN);
        } 
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Glfw.InitHint(Glfw.ANGLE_PLATFORM_TYPE, Glfw.ANGLE_PLATFORM_TYPE_METAL);
        }
        
        if (!Glfw.Init())
        {
            Console.WriteLine("Failed to initialize GLFW");
            return;
        }
        
        Glfw.SetErrorCallback(GlfwErrorCallback);
        
        Glfw.WindowHint(Glfw.CLIENT_API, Glfw.OPENGL_ES_API);
        Glfw.WindowHint(Glfw.CONTEXT_CREATION_API, Glfw.EGL_CONTEXT_API);
        Glfw.WindowHint(Glfw.CONTEXT_VERSION_MAJOR, 3);
        Glfw.WindowHint(Glfw.CONTEXT_VERSION_MINOR, 0);
        Glfw.WindowHint(Glfw.SAMPLES, 4);
        var window = Glfw.CreateWindow(1024, 768, "Nebula.Core C#", null, null);
        if (window == null)
        {
            Console.WriteLine("Failed to create GLFW window");
            Glfw.Terminate();
            return;
        }
        
        Glfw.MakeContextCurrent(window);
        
        Glfw.SetFramebufferSizeCallback(window, GlfwFramebufferSizeCallback);

        EngineInfo = EngineInformation.FromCurrent();
        EngineInfo.Print();

        WindowInfo = new WindowInformation((1024, 768));
        GL.Viewport(0, 0, 1024, 768);
        
        GL.Enable(GL.DEPTH_TEST);
        
        GL.Enable(GL.CULL_FACE);
        GL.CullFace(GL.BACK);
        
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Clear(GL.COLOR_BUFFER_BIT | GL.DEPTH_BUFFER_BIT);
        
        Input.Setup(window);
        // Start debugger

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        while (!Glfw.WindowShouldClose(window))
        {
            // Tick pipeline
        }
    }
}

public class EngineInformation
{
    public string NebulaVersion { get; private set; }
    public string GlVersion { get; private set; }
    public string GlRenderer { get; private set; }
    
    public EngineInformation(string nebulaVersion, string glVersion, string glRenderer)
    {
        NebulaVersion = nebulaVersion;
        GlVersion = glVersion;
        GlRenderer = glRenderer;
    }

    public static EngineInformation FromCurrent() => new EngineInformation(
        "0.1.0", 
        GL.GetString(GL.VERSION), 
        GL.GetString(GL.RENDERER)
    );

    public void Print()
    {
        Console.WriteLine($"Nebula version: {NebulaVersion}");
        Console.WriteLine($"GL version: {GlVersion}");
        Console.WriteLine($"GL renderer: {GlRenderer}");
    }
}

public class WindowInformation
{
    public (int width, int height) Size { get; private set; }
    
    public WindowInformation((int width, int height) size)
    {
        Size = size;
    }
}