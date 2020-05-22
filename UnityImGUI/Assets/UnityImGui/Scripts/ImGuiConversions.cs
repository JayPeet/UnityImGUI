using UnityEngine;
using System.Numerics;
public static class ImGuiConversions
{
    public static System.Numerics.Vector2 ToImGui(this UnityEngine.Vector2 v)
    {
        return new System.Numerics.Vector2(v.x, v.y);
    }

    public static UnityEngine.Vector2 ToUnity(this System.Numerics.Vector2 v)
    {
        return new UnityEngine.Vector2(v.X, v.Y);
    }

    public static System.Numerics.Vector3 ToImGui(this UnityEngine.Vector3 v)
    {
        return new System.Numerics.Vector3(v.x, v.y, v.z);
    }

    public static UnityEngine.Vector3 ToUnity(this System.Numerics.Vector3 v)
    {
        return new UnityEngine.Vector3(v.X, v.Y, v.Z);
    }

    public static System.Numerics.Vector4 ToImGui(this UnityEngine.Color c)
    {
        return new System.Numerics.Vector4(c.r, c.g, c.b, c.a);
    }

    public static UnityEngine.Color ToUnity(this System.Numerics.Vector4 c)
    {
        return new UnityEngine.Color(c.X, c.Y, c.Z, c.W);
    }
}
