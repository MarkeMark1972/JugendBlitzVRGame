using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class AttachmentBase : BaseClass
{
    public static event Action<int> JoyScoreIncreased;
    public static event Action<int> SadnessScoreIncreased;
    
    private Image iconImageDetached;

    private TrashBase _parentTrashBase;
    
    public bool isAttached = true;

    protected void Awake()
    {
        base.Awake();
        
        var iconCanvas = this.transform.Find("Icon Canvas");
        iconImageDetached = iconCanvas.transform.GetChild(2).GetComponent<Image>();
        
        // HideUi();
        ShowUi(true);
        OnDisable();
    }

    private void OnEnable()
    {
        if (!_parentTrashBase.isBaseSelected) return;

        xrGrabInteractable.enabled = true;
        rb.WakeUp();
        ActivateEvents();
    }

    private void OnDisable()
    {
        xrGrabInteractable.enabled = false;
        rb.Sleep();
        DeactivateEvents();
    }

    protected override void HandleOnSelectEnter(SelectEnterEventArgs args)
    {
        // if (!_parentTrashBase.isBaseSelected) return;
        isAttached = false;
        base.HandleOnSelectEnter(args);
        _parentTrashBase.RemoveFixedJoint();
        HideUi();
    }

    protected override void HandleOnSelectExit(SelectExitEventArgs args)
    {
        // if (!_parentTrashBase.isBaseSelected) return;

        HideUi();

        this.transform.parent = GameObject.Find("Produce Spawn Holder").transform;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.WakeUp();

        // _parentTrashBase.DisableSocket();
    }

    protected override void ShowUi(bool isLeft)
    {
        base.ShowUi(isLeft);
        iconImageDetached.enabled = true;
    }

    protected override void HideUi()
    {
        base.HideUi();
        iconImageDetached.enabled = false;
    }

    public void SetParentTrashBase(TrashBase trashBase)
    {
        _parentTrashBase = trashBase;
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}