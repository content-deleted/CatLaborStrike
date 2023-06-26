using System;
using UnityEngine;

namespace Nova.Attributes {

public enum ActionOnConditionFail {
    // If condition(s) are false, don't draw the field at all.
    DontDraw,
    // If condition(s) are false, just set the field as disabled.
    JustDisable,
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ShowIfAttribute : PropertyAttribute {
    public ActionOnConditionFail Action { get; private set; }
    public string Condition { get; private set; }

     public ShowIfAttribute(string condition, ActionOnConditionFail action = ActionOnConditionFail.DontDraw) {
        Action = action;
        Condition = condition;
    }
}

}  // namespace Nova.Attributes