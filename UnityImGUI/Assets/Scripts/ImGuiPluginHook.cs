using ImGuiNET;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ImGuiPluginHook : MonoBehaviour
{
    private delegate void DebugCallback(string message);

    [DllImport("UnityImGuiRenderer")]
    private static extern void RegisterDebugCallback(DebugCallback callback);

    [DllImport("UnityImGuiRenderer")]
    public static extern void ReadImGuiDrawData(ImGuiNET.ImDrawDataPtr ptr);

    private ImGuiController _controller;

    private void Awake()
    {
        _controller = new ImGuiController();
    }

    private void Start()
    {
        RegisterDebugCallback(new DebugCallback(DebugMethod));
    }

    private void Update()
    {
        _controller.Update();
        SubmitUI();
        _controller.Render();
    }

    private static void DebugMethod(string message)
    {
        Debug.Log("UnityImGuiRenderer: " + message);
    }

    private static void SubmitUI()
    {
        ImGui.Text("C# -> C++!");
    }
}
