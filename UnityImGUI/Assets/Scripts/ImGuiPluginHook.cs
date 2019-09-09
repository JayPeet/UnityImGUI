using ImGuiNET;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ImGuiPluginHook : MonoBehaviour
{
    [DllImport("UnityImGuiRenderer")]
    private static extern System.IntPtr GetRenderEventFunc();

    private delegate void DebugCallback(string message);

    [DllImport("UnityImGuiRenderer")]
    private static extern void RegisterDebugCallback(DebugCallback callback);

    [DllImport("UnityImGuiRenderer")]
    public static extern void SendImGuiDrawCommands(ImGuiNET.ImDrawDataPtr ptr);

    private ImGuiController _controller;

    private void Awake()
    {
        _controller = new ImGuiController();
    }

    private void Start()
    {
        RegisterDebugCallback(new DebugCallback(DebugMethod));
        StartCoroutine("CallPluginAtEndOfFrames");
    }

    private IEnumerator CallPluginAtEndOfFrames()
    {
        while (true)
        {
            //At the end of the frame, have ImGui render before invoking the draw on the GPU.
            yield return new WaitForEndOfFrame();
            _controller.Render();
            GL.IssuePluginEvent(GetRenderEventFunc(), 1);
        }
    }

     private void Update()
    {
        _controller.Update();
        SubmitUI();
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
