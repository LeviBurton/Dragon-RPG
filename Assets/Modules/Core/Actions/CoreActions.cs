using RPG.Characters;
using System;
using UnityEngine;

public class ActionMethodAttribute : Attribute
{
}

public class ActionMethods
{
    [ActionMethod]
    public static void core_ready(ActionConfig action, GameObject sender)
    {
        //Debug.LogFormat("{0} | {1}", action.actionName, sender.name);
    }

    [ActionMethod]
    public static void core_attack(ActionConfig action, GameObject sender)
    {
        var c = sender.GetComponent<CharacterSystem>();
        Debug.LogFormat("{0} | {1}", action.actionName, sender.name);
    }

    [ActionMethod]
    public static void core_move(ActionConfig action, GameObject sender)
    {
        //Debug.LogFormat("{0} | {1}", action.actionName, sender.name);
    }

    [ActionMethod]
    public static void core_dodge(ActionConfig action, GameObject sender)
    {
        //Debug.LogFormat("{0} | {1}", action.actionName, sender.name);
    }

    [ActionMethod]
    public static void core_dash(ActionConfig action, GameObject sender)
    {
        //Debug.LogFormat("{0} | {1}", action.actionName, sender.name);
    }

    [ActionMethod]
    public static void core_help(ActionConfig action, GameObject sender)
    {
        //Debug.LogFormat("{0} | {1}", action.actionName, sender.name);
    }

    [ActionMethod]
    public static void core_disengage(ActionConfig action, GameObject sender)
    {
        //Debug.LogFormat("{0} | {1}", action.actionName, sender.name);
    }

    [ActionMethod]
    public static void core_cast_spell(ActionConfig action, GameObject sender)
    {
        //Debug.LogFormat("{0} | {1}", action.actionName, sender.name);
    }

    [ActionMethod]
    public static void core_hide(ActionConfig action, GameObject sender)
    {
        //Debug.LogFormat("{0} | {1}", action.actionName, sender.name);
    }

    [ActionMethod]
    public static void core_search(ActionConfig action, GameObject sender)
    {
        //Debug.LogFormat("{0} | {1}", action.actionName, sender.name);
    }

    [ActionMethod]
    public static void core_use_object(ActionConfig action, GameObject sender)
    {
        //Debug.LogFormat("{0} | {1}", action.actionName, sender.name);
    }
}
