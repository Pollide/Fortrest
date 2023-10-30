using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TMPWave : MonoBehaviour
{
    public TMP_Text textMesh;
    public float amplitude = 0.1f;
    public float speed = 5f;

    private string originalText;

    private void Start()
    {
        textMesh = GetComponent<TMP_Text>();
        originalText = textMesh.text;
    }

    private void Update()
    {
        string wavedText = "";

        for (int i = 0; i < originalText.Length; i++)
        {
            float offset = Mathf.Sin((Time.time * speed) - (i * 0.1f)) * amplitude;
            wavedText += "<voffset=" + offset.ToString("F2") + ">" + originalText[i] + "</voffset>";
        }

        textMesh.text = wavedText;
    }
}
