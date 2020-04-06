using System;
using ImGuiNET;
using UnityEngine;

public class ImGuiController
{
    private ImGuiInput _input = new ImGuiInput();
    private bool _frameBegun;

    public ImGuiController()
    {
        IntPtr context = ImGui.CreateContext();
        ImGui.SetCurrentContext(context);

        ImGui.GetIO().Fonts.AddFontDefault();

        //#TODO : GenerateDeviceResources in Plugin.
        RecreateFontDeviceTexture(false);
        _input.SetKeyMappings();

        SetPerFrameImGuiData(1.0f / 60.0f, Screen.width, Screen.height, new System.Numerics.Vector2(1.0f, 1.0f));

        ImGui.NewFrame();
    }

    ~ImGuiController()
    {
        ImGui.DestroyContext();
    }

    public void RecreateFontDeviceTexture(bool sendToGPU)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        IntPtr pixels;
        int width, height, bytesPerPixel;
        io.Fonts.GetTexDataAsRGBA32(out pixels, out width, out height, out bytesPerPixel);

        if (sendToGPU)
        {
            IntPtr fontTexID = ImGuiPluginHook.GenerateImGuiFontTexture(pixels, width, height, bytesPerPixel);
            io.Fonts.SetTexID(fontTexID);
        }

        io.Fonts.ClearTexData();
    }

    public void SetPerFrameImGuiData(float deltaSeconds, int _windowWidth, int _windowHeight, System.Numerics.Vector2 _scaleFactor)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new System.Numerics.Vector2(
            _windowWidth / _scaleFactor.X,
            _windowHeight / _scaleFactor.Y);
        io.DisplayFramebufferScale = _scaleFactor;
        io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
    }
    public void Render()
    {
        if (_frameBegun)
        {
            _frameBegun = false;

            ImGui.Render();
            //#TODO : Pass GetDrawData to the C++ dll.
            var d = ImGui.GetDrawData();

            ImGuiPluginHook.SendImGuiDrawCommands(d);
        }
    }

    public void Update()
    {
        Debug.Assert(ImGui.GetCurrentContext() != IntPtr.Zero);

        SetPerFrameImGuiData(Time.deltaTime, Screen.width, Screen.height, new System.Numerics.Vector2(1.0f, 1.0f));
        _input.Update();

        _frameBegun = true;
        ImGui.NewFrame();
    }    
}
