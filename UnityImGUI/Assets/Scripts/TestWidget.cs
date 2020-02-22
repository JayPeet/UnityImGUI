using ImGuiNET;
using UnityEngine;

public class TestWidget : MonoBehaviour
{
    public float x;
    public float y;
    public System.Numerics.Vector3 backgroundColour;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        backgroundColour = new System.Numerics.Vector3(cam.backgroundColor.r, cam.backgroundColor.g, cam.backgroundColor.b);
    }

    private void Update()
    {
        ImGui.SetNextWindowPos(new System.Numerics.Vector2(650, 20), ImGuiCond.FirstUseEver);
        ImGui.Begin("TestWidget");
        ImGui.DragFloat("X:", ref x);
        ImGui.DragFloat("Y:", ref y);
        ImGui.ColorPicker3("Background Color", ref backgroundColour);
        ImGui.End();

        Color newCol = new Color(backgroundColour.X, backgroundColour.Y, backgroundColour.Z);
        cam.backgroundColor = newCol;
    }
}