using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;


public class UseRenderingPlugin : MonoBehaviour
{
	[DllImport("RenderingPlugin")]
	private static extern IntPtr GetRenderEventFunc();

	IEnumerator Start()
	{
		yield return StartCoroutine("CallPluginAtEndOfFrames");
	}

	private IEnumerator CallPluginAtEndOfFrames()
	{
		while (true)
        {
			yield return new WaitForEndOfFrame();
			GL.IssuePluginEvent(GetRenderEventFunc(), 1);
		}
	}
}
