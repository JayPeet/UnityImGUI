using ImGuiNET;
using UnityEngine;

public class DemoWindow : MonoBehaviour
{
    private System.Numerics.Vector2 screenWindowPos = new System.Numerics.Vector2(650, 20);
    private void Update()
    {
        ImGui.SetNextWindowPos(screenWindowPos, ImGuiCond.FirstUseEver);
        ImGui.ShowDemoWindow();
    }
}
