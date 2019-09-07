workspace "UnityImGui"
    configurations {"Debug", "Release"}
    architecture "x64"
project "UnityImGuiRenderer"
    kind "SharedLib"
    language "C++"
    cppdialect "C++17"
    staticruntime "on"

    targetdir "../../UnityImGui/Assets/Plugins"
    objdir "projectfiles/"
    location "."
    files {"**.h", "**.hpp", "**.c","**.cpp"}
    filter "system:windows"
        systemversion "latest"
        links {"opengl32"}
    filter "configurations:Debug"
      symbols "On"
   filter "configurations:Release"
      optimize "On"