using System;
using ImGuiNET;
using UnityEngine;

public class ImGuiController
{
    private bool _frameBegun;
    private IntPtr _fontAtlasID;

    public ImGuiController()
    {
        IntPtr context = ImGui.CreateContext();
        ImGui.SetCurrentContext(context);

        ImGui.GetIO().Fonts.AddFontDefault();

        //#TODO : GenerateDeviceResources in Plugin.
        RecreateFontDeviceTexture(false);
        SetKeyMappings();

        SetPerFrameImGuiData(1.0f / 60.0f, Screen.width, Screen.height, new System.Numerics.Vector2(1.0f, 1.0f));

        ImGui.NewFrame();
        _frameBegun = true;
    }

    ~ImGuiController()
    {
        ImGui.DestroyContext();
        Debug.Log("Kill Context");
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

    private static void SetKeyMappings()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.KeyMap[(int)ImGuiKey.Tab] = (int)KeyCode.Tab;
        io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)KeyCode.LeftArrow;
        io.KeyMap[(int)ImGuiKey.RightArrow] = (int)KeyCode.RightArrow;
        io.KeyMap[(int)ImGuiKey.UpArrow] = (int)KeyCode.UpArrow;
        io.KeyMap[(int)ImGuiKey.DownArrow] = (int)KeyCode.DownArrow;
        io.KeyMap[(int)ImGuiKey.PageUp] = (int)KeyCode.PageUp;
        io.KeyMap[(int)ImGuiKey.PageDown] = (int)KeyCode.PageDown;
        io.KeyMap[(int)ImGuiKey.Home] = (int)KeyCode.Home;
        io.KeyMap[(int)ImGuiKey.End] = (int)KeyCode.End;
        io.KeyMap[(int)ImGuiKey.Delete] = (int)KeyCode.Delete;
        io.KeyMap[(int)ImGuiKey.Backspace] = (int)KeyCode.Backspace;
        io.KeyMap[(int)ImGuiKey.Enter] = (int)KeyCode.Return;
        io.KeyMap[(int)ImGuiKey.Escape] = (int)KeyCode.Escape;
        io.KeyMap[(int)ImGuiKey.A] = (int)KeyCode.A;
        io.KeyMap[(int)ImGuiKey.C] = (int)KeyCode.C;
        io.KeyMap[(int)ImGuiKey.V] = (int)KeyCode.V;
        io.KeyMap[(int)ImGuiKey.X] = (int)KeyCode.X;
        io.KeyMap[(int)ImGuiKey.Y] = (int)KeyCode.Y;
        io.KeyMap[(int)ImGuiKey.Z] = (int)KeyCode.Z;
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
        if (_frameBegun)
        {
            ImGui.Render();
        }

        SetPerFrameImGuiData(Time.deltaTime, Screen.width, Screen.height, new System.Numerics.Vector2(1.0f, 1.0f));
        UpdateImGuiInput();

        _frameBegun = true;
        ImGui.NewFrame();
    }

    private void UpdateImGuiInput()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        System.Numerics.Vector2 mousePosition = new System.Numerics.Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

        bool leftPressed = Input.GetMouseButton(0);
        bool middlePressed = Input.GetMouseButton(1);
        bool rightPressed = Input.GetMouseButton(2);

        io.MouseDown[0] = leftPressed || Input.GetMouseButtonDown(0);
        io.MouseDown[1] = rightPressed || Input.GetMouseButtonDown(1);
        io.MouseDown[2] = middlePressed || Input.GetMouseButtonDown(2);

        io.MousePos = mousePosition;
        io.MouseWheel = Input.mouseScrollDelta.y;

        // Call to io.AddInputCharacter is required to make text wiget working
        
        io.AddInputCharactersUTF8(Input.inputString);

        // Check all keys. We can rely on Input.inputString with this, because we miss arrows and few other keys.
        if(Input.anyKey)
        {
            for (int key = 0; key < 512; ++key)
            {
                io.KeysDown[key] = Input.GetKey((KeyCode)key);
            }

            io.KeyCtrl = Input.GetKey(KeyCode.LeftControl);
            io.KeyAlt = Input.GetKey(KeyCode.LeftAlt);
            io.KeyShift = Input.GetKey(KeyCode.LeftShift);
            io.KeySuper = Input.GetKey(KeyCode.LeftWindows);
        }
    }
}
