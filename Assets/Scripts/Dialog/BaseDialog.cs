using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseDialog : MonoBehaviour {
    public DialogManager dialogManager;
    //public UnityEvent onDialogStart;
    //public UnityEvent onDialogComplete;

    //public void OnDialogStart() {
    //    onDialogStart.Invoke();
    //}
    //public void OnDialogComplete() {
    //    onDialogComplete.Invoke();
    //}

    public abstract void StartDialog();
}
