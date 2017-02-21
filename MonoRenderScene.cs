using MoleMole;
using System;
using UnityEngine;

public class MonoRenderScene : MonoBehaviour
{
    private Camera _camera;
    private GUIStyle _style;
    private bool _toggled;
    public RenderGroup[] renderGroups;

    private void OnEnable()
    {
        this._style = new GUIStyle();
        Texture2D textured = new Texture2D(0x10, 0x10);
        Color[] colors = new Color[0x100];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.gray;
        }
        textured.SetPixels(colors);
        this._style.normal.background = textured;
        Application.targetFrameRate = 60;
        if (Singleton<MainUIManager>.Instance == null)
        {
            Singleton<MainUIManager>.Create();
        }
        if (Singleton<MiHoYoGameData>.Instance == null)
        {
            Singleton<MiHoYoGameData>.Create();
        }
        this._camera = Camera.main;
        MonoWindZone zone = UnityEngine.Object.FindObjectOfType<MonoWindZone>();
        if (zone != null)
        {
            zone.Init();
        }
        base.gameObject.AddComponent<MonoBenchmarkSwitches>();
    }

    private void OnGUI()
    {
        if (this._toggled)
        {
            GUI.color = Color.white;
            GUI.backgroundColor = Color.gray;
            GUILayout.BeginArea(new Rect(10f, 10f, 250f, (float) (Screen.height - 20)), this._style);
            for (int i = 0; i < this.renderGroups.Length; i++)
            {
                RenderGroup group = this.renderGroups[i];
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Label(group.name, new GUILayoutOption[0]);
                GUILayoutOption[] optionArray1 = new GUILayoutOption[] { GUILayout.Width(100f), GUILayout.Height(40f) };
                if (GUILayout.Button("Toggle", optionArray1))
                {
                    for (int j = 0; j < group.gameObjects.Length; j++)
                    {
                        group.gameObjects[j].SetActive(!group.gameObjects[j].activeSelf);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(50f) };
            this._toggled = !GUILayout.Button("Close", options);
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(260f, 10f, 250f, (float) (Screen.height - 20)), this._style);
            GUILayout.Label(string.Format("FPS: {0}", 1f / Time.smoothDeltaTime), new GUILayoutOption[0]);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Height(50f) };
            if (GUILayout.Button("PostFX Toggle", optionArray3))
            {
                PostFXWithResScale scale = UnityEngine.Object.FindObjectOfType<PostFXWithResScale>();
                if (scale != null)
                {
                    scale.enabled = !scale.enabled;
                }
            }
            GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.Height(50f) };
            if (GUILayout.Button("HDR & HDR Buffer", optionArray4))
            {
                PostFXWithResScale scale2 = UnityEngine.Object.FindObjectOfType<PostFXWithResScale>();
                if (scale2 != null)
                {
                    bool hdr = Camera.main.hdr;
                    Camera.main.hdr = !hdr;
                    scale2.HDRBuffer = !hdr;
                }
            }
            GUILayoutOption[] optionArray5 = new GUILayoutOption[] { GUILayout.Height(50f) };
            if (GUILayout.Button("FXAA", optionArray5))
            {
                PostFXWithResScale scale3 = UnityEngine.Object.FindObjectOfType<PostFXWithResScale>();
                if (scale3 != null)
                {
                    scale3.FXAA = !scale3.FXAA;
                }
            }
            GUILayoutOption[] optionArray6 = new GUILayoutOption[] { GUILayout.Height(50f) };
            if (GUILayout.Button("Distortion Map", optionArray6))
            {
                PostFXWithResScale scale4 = UnityEngine.Object.FindObjectOfType<PostFXWithResScale>();
                if (scale4 != null)
                {
                    scale4.UseDistortion = !scale4.UseDistortion;
                }
            }
            GUILayoutOption[] optionArray7 = new GUILayoutOption[] { GUILayout.Height(50f) };
            if (GUILayout.Button("Distortion Apply", optionArray7))
            {
                PostFXWithResScale scale5 = UnityEngine.Object.FindObjectOfType<PostFXWithResScale>();
                if (scale5 != null)
                {
                    scale5.UseDistortion = !scale5.UseDistortion;
                }
            }
            GUILayoutOption[] optionArray8 = new GUILayoutOption[] { GUILayout.Height(50f) };
            if (GUILayout.Button("Use Distortion Depth Test", optionArray8))
            {
                PostFXWithResScale scale6 = UnityEngine.Object.FindObjectOfType<PostFXWithResScale>();
                if (scale6 != null)
                {
                    scale6.UseDepthTest = !scale6.UseDepthTest;
                }
            }
            GUILayout.EndArea();
        }
        else
        {
            this._toggled = GUI.Button(new Rect(10f, 10f, 150f, 50f), "Render Scene");
        }
    }

    private void Update()
    {
        Shader.SetGlobalVector("_miHoYo_CameraRight", this._camera.transform.right);
    }
}

