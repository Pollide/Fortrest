using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossTelegraph : MonoBehaviour
{
    [SerializeField] private Image telegraphImage;
    [SerializeField] private SlamState state;

    // Start is called before the first frame update
    void Start()
    {
        telegraphImage.rectTransform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        telegraphImage.rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, state.SlamWaitTime / state.SlamDuration);
    }
}
