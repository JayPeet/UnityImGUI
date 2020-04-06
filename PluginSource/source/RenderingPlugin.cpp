// Example low level rendering Unity plugin

#include "PlatformBase.h"
#include "RenderAPI.h"

#include "cimgui.h"

#include <assert.h>
#include <math.h>
#include <vector>
#include <string>


// --------------------------------------------------------------------------
// UnitySetInterfaces

static void UNITY_INTERFACE_API OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType);

static IUnityInterfaces* s_UnityInterfaces = NULL;
static IUnityGraphics* s_Graphics = NULL;

static ImDrawData* s_drawData = NULL;

void CleanupDrawLists(ImDrawData* drawData)
{
	// D11.JN: CmdLists are normally managed internally by imgui, but we're cloning them so we need to take responsibility for cleanup
	if (drawData->CmdLists)
	{
		for (int i = 0; i < drawData->CmdListsCount; ++i)
		{
			delete drawData->CmdLists[i];
		}
		delete[] drawData->CmdLists;
	}
}


extern "C" void	UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces)
{
	s_UnityInterfaces = unityInterfaces;
	s_Graphics = s_UnityInterfaces->Get<IUnityGraphics>();
	s_Graphics->RegisterDeviceEventCallback(OnGraphicsDeviceEvent);

	s_drawData = new ImDrawData();
	
#if SUPPORT_VULKAN
	if (s_Graphics->GetRenderer() == kUnityGfxRendererNull)
	{
		extern void RenderAPI_Vulkan_OnPluginLoad(IUnityInterfaces*);
		RenderAPI_Vulkan_OnPluginLoad(unityInterfaces);
	}
#endif // SUPPORT_VULKAN

	// Run OnGraphicsDeviceEvent(initialize) manually on plugin load
	OnGraphicsDeviceEvent(kUnityGfxDeviceEventInitialize);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
{
	CleanupDrawLists(s_drawData);
	ImDrawData_destroy(s_drawData);

	s_Graphics->UnregisterDeviceEventCallback(OnGraphicsDeviceEvent);
}

#if UNITY_WEBGL
typedef void	(UNITY_INTERFACE_API * PluginLoadFunc)(IUnityInterfaces* unityInterfaces);
typedef void	(UNITY_INTERFACE_API * PluginUnloadFunc)();

extern "C" void	UnityRegisterRenderingPlugin(PluginLoadFunc loadPlugin, PluginUnloadFunc unloadPlugin);

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API RegisterPlugin()
{
	UnityRegisterRenderingPlugin(UnityPluginLoad, UnityPluginUnload);
}
#endif

// --------------------------------------------------------------------------
// GraphicsDeviceEvent


static RenderAPI* s_CurrentAPI = NULL;
static UnityGfxRenderer s_DeviceType = kUnityGfxRendererNull;


static void UNITY_INTERFACE_API OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType)
{
	// Create graphics API implementation upon initialization
	if (eventType == kUnityGfxDeviceEventInitialize)
	{
		assert(s_CurrentAPI == NULL);
		s_DeviceType = s_Graphics->GetRenderer();
		s_CurrentAPI = CreateRenderAPI(s_DeviceType);
	}

	// Let the implementation process the device related events
	if (s_CurrentAPI)
	{
		s_CurrentAPI->ProcessDeviceEvent(eventType, s_UnityInterfaces);
	}

	// Cleanup graphics API implementation upon shutdown
	if (eventType == kUnityGfxDeviceEventShutdown)
	{
		delete s_CurrentAPI;
		s_CurrentAPI = NULL;
		s_DeviceType = kUnityGfxRendererNull;
	}
}



// --------------------------------------------------------------------------
// OnRenderEvent
// This will be called for GL.IssuePluginEvent script calls; eventID will
// be the integer passed to IssuePluginEvent. In this example, we just ignore
// that value.
static void UNITY_INTERFACE_API OnRenderEvent(int eventID)
{
	// Unknown / unsupported graphics device type? Do nothing
	if (s_CurrentAPI == NULL)
		return;

	if (s_drawData)
	{
		s_CurrentAPI->ProcessImGuiCommandList(s_drawData);
	}
}


// --------------------------------------------------------------------------
// GetRenderEventFunc, an example function we export which is used to get a rendering event callback function.

extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetRenderEventFunc()
{
	return OnRenderEvent;
}


//
typedef void(__stdcall* DebugCallback) (const char* str);
DebugCallback gDebugCallback;

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API RegisterDebugCallback(DebugCallback callback)
{
    if (callback)
    {
        gDebugCallback = callback;
    }
}

static void DebugInUnity(std::string message)
{
    if (gDebugCallback)
    {
        gDebugCallback(message.c_str());
    }
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SendImGuiDrawCommands(ImDrawData* inData)
{
	CleanupDrawLists(s_drawData);
	ImDrawData_Clear(s_drawData);

	if (inData)
	{
		s_drawData->Valid = inData->Valid;
		s_drawData->CmdListsCount = inData->CmdListsCount;
		s_drawData->TotalIdxCount = inData->TotalIdxCount;
		s_drawData->TotalVtxCount = inData->TotalVtxCount;
		s_drawData->DisplayPos = inData->DisplayPos;
		s_drawData->DisplaySize = inData->DisplaySize;
		s_drawData->FramebufferScale = inData->FramebufferScale;

		s_drawData->CmdLists = inData->CmdListsCount ? new ImDrawList*[inData->CmdListsCount] : NULL;
	
		for (int i = 0; i < inData->CmdListsCount; ++i)
		{
			s_drawData->CmdLists[i] = ImDrawList_CloneOutput(inData->CmdLists[i]);
		}
	}
}

extern "C" ImTextureID UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GenerateImGuiFontTexture(void* pixels, int width, int height, int bytesPerPixel)
{
	return s_CurrentAPI->CreateImGuiFontsTexture(pixels, width, height, bytesPerPixel);
}

