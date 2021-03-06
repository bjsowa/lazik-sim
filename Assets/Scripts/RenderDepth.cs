﻿using UnityEngine;

public class RenderDepth : MonoBehaviour
{
    private Shader m_Shader;
    private Material m_Material;

    private void Awake()
    {
        m_Shader = Shader.Find("Custom/RenderDepth");
        if (m_Shader == null)
            Debug.LogError("shader null");
        m_Material = new Material(m_Shader);
        m_Material.hideFlags = HideFlags.HideAndDontSave;
    }
    
    private void Start()
    {
        if (m_Shader == null || !m_Shader.isSupported)
        {
            Debug.LogError("Shader " + m_Shader.name + " is not supported");
            enabled = false;
            return;
        }

        // turn on depth rendering for the camera so that the shader can access it via _CameraDepthTexture
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (m_Shader != null)
            Graphics.Blit(src, dest, m_Material);
        else
            Graphics.Blit(src, dest);
    }
}