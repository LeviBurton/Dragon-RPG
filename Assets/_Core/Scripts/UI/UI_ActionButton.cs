using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ActionButton : MonoBehaviour
{
    public ActionSystem actionSystem = null;
    public ActionConfig actionConfig = null;

    public void SetActionConfig(ActionConfig actionConfig)
    {
        this.actionConfig = actionConfig;
    }

    public void SetActionSystem(ActionSystem actionSystem)
    {
        this.actionSystem = actionSystem;
    }

    public void OnButtonPress()
    {
        actionConfig.Execute(actionSystem.gameObject);
    }
}
