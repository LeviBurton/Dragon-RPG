using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Character_Creation : MonoBehaviour {

    public RectTransform[] contentPages;
    public Button nextButton;
    public Button prevButton;
    public TextMeshProUGUI contentTitleText;
    public TextMeshProUGUI descriptionText;
    private int currentContentIndex;

    void Start()
    {
        contentTitleText.text = "Gender, Race and Class";
        descriptionText.text = string.Empty;
        currentContentIndex = 0;

        for (int i = 0; i < contentPages.Length; i++)
        {
            contentPages[i].gameObject.SetActive(false);
        }
      
        contentPages[currentContentIndex].gameObject.SetActive(true);
    }

    public void NextButtonClicked()
    {
        var previousContentIndex = currentContentIndex;
        contentPages[previousContentIndex].gameObject.SetActive(false);

        currentContentIndex++;

        if (currentContentIndex >= contentPages.Length)
        {
            currentContentIndex = contentPages.Length - 1;
            //nextButton.enabled = false;
            //prevButton.enabled = true;
        }
        else
        {
            //nextButton.enabled = true;
            //prevButton.enabled = true;
        }

        contentPages[currentContentIndex].gameObject.SetActive(true);

        descriptionText.text += "<size=100%><color=#00FFFF>Next Button Clicked\n</color></size>";
    }

    public void PrevButtonClicked()
    {
        var previousContentIndex = currentContentIndex;
        contentPages[previousContentIndex].gameObject.SetActive(false);

        currentContentIndex--;

        if (currentContentIndex <= 0)
        {
            currentContentIndex = 0;
            //prevButton.enabled = false;
        }
        else
        {
            //prevButton.enabled = true;
            //nextButton.enabled = true;
        }

        contentPages[currentContentIndex].gameObject.SetActive(true);

        descriptionText.text += "<color=#FFFF00>Previous Button Clicked\n</color>";
    }

}
