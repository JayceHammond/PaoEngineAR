using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Popup : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private TextMeshProUGUI textMeshPro;
    private bool isEnabled = false;
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        textMeshPro = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        textMeshPro.SetText("");

        LeanTween.alphaCanvas(canvasGroup, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPopup(string text){
        textMeshPro.SetText(text);
        LeanTween.alphaCanvas(canvasGroup, 0.8f, 1f);
        StartCoroutine(DissolvePopup());
    }

    private IEnumerator DissolvePopup(){
        yield return new WaitForSeconds(3);
        LeanTween.alphaCanvas(canvasGroup, 0f, 1f);
    }
}
