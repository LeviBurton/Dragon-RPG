using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Action")]
public class ActionConfig : ScriptableObject
{
    //public delegate void OnActionComplete(ActionConfig actionConfig, GameObject executer);
    //public event OnActionComplete onActionComplete;

    public string actionName;

    [TextArea(3, 30)]
    public string Description;

    public Sprite spriteIcon;

    // We bind the functionality of the action to a static method.  This allows us to 
    // define the behavior in code then bind that to this action asset.
    public string executeMethodName = string.Empty;
    public MethodInfo executeMethod = null;
    private Action<ActionConfig, GameObject> actionExecuteDelegate = null;

    private void OnEnable()
    {
        BindExecuteMethod();
    }

    public void BindExecuteMethod()
    {
        if (executeMethod != null)
        {
            return;
        }

        executeMethod = typeof(ActionConfig).Assembly
            .GetTypes()
            .SelectMany(x => x.GetMethods())
            .Where(x => x.GetCustomAttributes(true).OfType<ActionMethodAttribute>().Any())
            .Where(x => x.Name == executeMethodName).SingleOrDefault();

        if (executeMethod == null)
            return;

        actionExecuteDelegate = (Action<ActionConfig, GameObject>)Delegate.CreateDelegate(typeof(Action<ActionConfig, GameObject>), executeMethod);
    }

    public void BindExecuteMethod(string methodName)
    {
        if (executeMethodName == methodName)
            return;

        executeMethodName = methodName;

        executeMethod = typeof(ActionConfig).Assembly
            .GetTypes()
            .SelectMany(x => x.GetMethods())
            .Where(x => x.GetCustomAttributes(true).OfType<ActionMethodAttribute>().Any())
            .Where(x => x.Name == methodName).SingleOrDefault();

        if (executeMethod == null)
            return;

        actionExecuteDelegate = (Action<ActionConfig, GameObject>)Delegate.CreateDelegate(typeof(Action<ActionConfig, GameObject>), executeMethod);
    }

    // This method is what we will call when the user wants to execute one of their actions.  
    // It will call the bound delegate we selected in the inspector.
    public void Execute(GameObject executer)
    {
        if (actionExecuteDelegate == null)
            return;

        actionExecuteDelegate(this, executer);
    }
}


