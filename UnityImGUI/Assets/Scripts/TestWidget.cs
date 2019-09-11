using ImGuiNET;

public class TestWidget
{
    public void Draw(ref float x, ref float y)
    {
        ImGui.SetNextWindowPos(new System.Numerics.Vector2(650, 20), ImGuiCond.FirstUseEver);
        ImGui.Begin("TestWidget");
        ImGui.DragFloat("X:", ref x);
        ImGui.DragFloat("Y:", ref y);
        ImGui.End();
    }
}

public static class Widgets
{
    public static void StopApplicationButton()
    {
        if(ImGui.Button("Stop Application"))
        {
            UnityEngine.Application.Quit();
        }
    }

    public static void DrawGraph()
    {
        float[] vals = new float[] { 0.0f, 0.3f, 1.1f, 0.5f };
        ImGui.PlotHistogram("Test Graph", ref vals[0], vals.Length, 0, "test overlay", 0.0f, 2.0f);
    }

}