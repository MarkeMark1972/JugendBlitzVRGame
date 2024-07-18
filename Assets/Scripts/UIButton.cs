using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static XRUIInputModule inputModule => EventSystem.current.currentInputModule as XRUIInputModule;
 
    public void OnPointerEnter(PointerEventData eventData)
    {
        var interactor = inputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
        if (!interactor) { return; }
        interactor.xrController.SendHapticImpulse(.1f, .1f);
 
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        var interactor = inputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
        if (!interactor) { return; }
        interactor.xrController.SendHapticImpulse(.1f, .1f);
 
    }
}