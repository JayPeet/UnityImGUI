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
    public static extern System.IntPtr GenerateImGuiFontTexture(System.IntPtr pixels, int width, int height, int bytesPerPixel);

    [DllImport("UnityImGuiRenderer")]
    public static extern void SendImGuiDrawCommands(ImGuiNET.ImDrawDataPtr ptr);

    private ImGuiController _controller;

    public float x, y;
    private TestWidget widget;

    private void Awake()
    {
        _controller = new ImGuiController();
        widget = new TestWidget();
    }

    private void Start()
    {
        RegisterDebugCallback(new DebugCallback(DebugMethod));
        StartCoroutine("CallPluginAtEndOfFrames");
    }

    private WaitForEndOfFrame frameWait = new WaitForEndOfFrame();
    private IEnumerator CallPluginAtEndOfFrames()
    {

        yield return frameWait;
        _controller.RecreateFontDeviceTexture(true);

        while (true)
        {
            //At the end of the frame, have ImGui render before invoking the draw on the GPU.
            yield return frameWait;
            _controller.Render();
            GL.IssuePluginEvent(GetRenderEventFunc(), 1);
        }
    }

    private void Update()
    {
        _controller.Update();
        SubmitUI();
        //Widgets.DrawGraph();
    }

    private static void DebugMethod(string message)
    {
        Debug.Log("UnityImGuiRenderer: " + message);
    }


    private static void SubmitUI()
    {
        ImGui.SetNextWindowPos(new System.Numerics.Vector2(650, 20), ImGuiCond.FirstUseEver);
        ImGui.ShowDemoWindow();
    }
}
