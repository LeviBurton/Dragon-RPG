using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ActionBar : MonoBehaviour
{
    public ActionSystem actionSystem = null;
    public GameObject actionButtonPrefab;
    
    void Start()
    {
        for (int i = 0; i < actionSystem.actions.Count; i++)
        {
            var action = actionSystem.actions[i];
            var parent = transform.GetChild(i);
            var actionButton = Instantiate(actionButtonPrefab, parent);
            var uiActionButton = actionButton.GetComponent<UI_ActionButton>();
            var actionButtonImage = actionButton.GetComponent<Image>();

            uiActionButton.SetActionConfig(action);
            uiActionButton.SetActionSystem(actionSystem);
            actionButtonImage.sprite = action.spriteIcon;
        }
    }
}
