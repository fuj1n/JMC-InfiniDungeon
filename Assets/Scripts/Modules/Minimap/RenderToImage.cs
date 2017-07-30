using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderToImage : MonoBehaviour {

	public RawImage target;
	public int width, height;

	private RenderTexture tex;

	private void Start () {
		tex = new RenderTexture(Mathf.RoundToInt(width), Mathf.RoundToInt(height), 24);
		GetComponent<Camera>().targetTexture = tex;
		target.texture = tex;
	}

	private void OnDestroy()
	{
		tex.Release();
		tex = null;
	}
}
