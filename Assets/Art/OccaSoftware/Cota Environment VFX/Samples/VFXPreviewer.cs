using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.VFX;

namespace OccaSoftware.Cota.Demo
{
    public class VFXPreviewer : MonoBehaviour
    {
        List<Transform> children = new List<Transform>();

        int index = 0;

        void Start()
        {
            foreach (Transform t in transform)
            {
                children.Add(t);
            }
            children[0].gameObject.SetActive(true);
        }

        Texture2D normalBackground = null;
        Texture2D hoverBackground = null;
        GUIStyle buttonStyle = null;

        private void SetupStyle()
        {
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontSize = 16;
                buttonStyle.normal.textColor = Color.black;
                buttonStyle.hover.textColor = Color.black;
                if (normalBackground == null)
                {
                    normalBackground = MakeTex(1, 1, new Color(1f, 1f, 1f, 1f));
                }
                buttonStyle.normal.background = normalBackground;
                if (hoverBackground == null)
                {
                    hoverBackground = MakeTex(1, 1, new Color(0.8f, 0.8f, 0.8f, 1f));
                }
                buttonStyle.hover.background = hoverBackground;
            }
        }

        private void OnGUI()
        {
            SetupStyle();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.Space(20);
            if (
                GUILayout.Button(
                    "<- Previous",
                    buttonStyle,
                    GUILayout.Width(100),
                    GUILayout.Height(30)
                )
            )
            {
                SwitchChild(false);
            }

            GUILayout.Space(20);

            if (
                GUILayout.Button("Next ->", buttonStyle, GUILayout.Width(100), GUILayout.Height(30))
            )
            {
                SwitchChild(true);
            }
            GUILayout.Space(20);
            foreach (Transform t in transform)
            {
                if (t.gameObject.activeSelf)
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.fontSize = 24; // Set the font size to your desired value

                    GUILayout.Label(t.gameObject.name, style);
                }
            }
            GUILayout.Space(20);
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }

        private void SwitchChild(bool next)
        {
            children[index].gameObject.SetActive(false);

            if (next)
            {
                index = (index + 1) % children.Count;
            }
            else
            {
                index = (index - 1 + children.Count) % children.Count;
            }

            children[index].gameObject.SetActive(true);
        }

        private Texture2D MakeTex(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = color;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
