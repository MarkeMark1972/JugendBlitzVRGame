using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TrashBase : BaseClass
{
    public static event Action<int> JoyScoreIncreased;
    public static event Action<int> SadnessScoreIncreased;


    protected virtual void Awake()
    {
        base.Awake();
        
        var capSocket = this.transform.Find("Cap Socket");
        var socketBase = capSocket.transform.Find("Base");
        var attachmentBase = capSocket.transform.Find("Attachment");

        if (socketBase != null && socketBase.childCount > 0) attachment = socketBase.GetChild(0);
        if (attachmentBase != null && attachmentBase.childCount > 0) attachmentPlaceholder = attachmentBase.GetChild(0);

        if (attachment != null)
        {
            attachment.GetComponent<AttachmentBase>().SetParentTrashBase(this);
            attachment.gameObject.SetActive(false);
        }
        if (attachmentPlaceholder != null) attachmentPlaceholder.gameObject.SetActive(true);

        HideUi();
        ActivateEvents();
    }

    private void Update()
    {
        if (!TempGlobalValues.Instance.isPressurePlateActive)
        {
            if (Mathf.Abs(rb.velocity.z) < TempGlobalValues.Instance.BeltSpeedMax)
            {
                TempGlobalValues.Instance.BeltSpeed =
                    Mathf.Lerp(TempGlobalValues.Instance.BeltSpeed, TempGlobalValues.Instance.BeltSpeedMax, TempGlobalValues.Instance.accelerationDelay);
            }
        }
        else
        {
            TempGlobalValues.Instance.BeltSpeed = Mathf.Lerp(TempGlobalValues.Instance.BeltSpeed, 0, TempGlobalValues.Instance.accelerationDelay);
            if (TempGlobalValues.Instance.BeltSpeed < 0.001f) TempGlobalValues.Instance.BeltSpeed = 0;
        }
    }

    protected override void HandleOnSelectEnter(SelectEnterEventArgs args)
    {
        if (isBaseSelected) return;
        isBaseSelected = true;

        if (attachment != null && attachment.GetComponent<AttachmentBase>().isAttached)
        {
            attachmentPlaceholder.gameObject.SetActive(false);
            attachment.gameObject.SetActive(true);
        }

        base.HandleOnSelectEnter(args);
    }

    protected override void HandleOnSelectExit(SelectExitEventArgs args)
    {
        isBaseSelected = false;

        base.HandleOnSelectExit(args);

        HideUi();
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}