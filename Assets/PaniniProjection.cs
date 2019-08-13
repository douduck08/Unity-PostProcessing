using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaniniProjection : MonoBehaviour {

    [SerializeField] Shader paniniProjectionShader;
    [SerializeField, Range (0.5f, 1.5f)] float scale = 1f;

    Camera mainCamera;
    Material material;

    void Start () {
        mainCamera = Camera.main;
        material = new Material (paniniProjectionShader);
    }

    void OnRenderImage (RenderTexture src, RenderTexture dest) {
        var projectionMatrix = GL.GetGPUProjectionMatrix (mainCamera.projectionMatrix, false);
        material.SetMatrix ("_InverseProjectionMatrix", Matrix4x4.Inverse (projectionMatrix));
        material.SetFloat ("_Scale", scale);
        Graphics.Blit (src, dest, material, 0);
    }
}