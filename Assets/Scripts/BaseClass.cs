using System;
using System.Collections;
using System.Collections.Generic;
using JugendBlitz.scripts.runtime;
using Prefabs.Produce.SO_Data;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Random = UnityEngine.Random;

public class BaseClass : MonoBehaviour
{
    [Space]
    [SerializeField] public TargetBinType targetBinType;

    [Space]
    [SerializeField] protected FixedJoint fixedJoint;
    [SerializeField] public bool isBaseSelected = false;
    [SerializeField] public bool hasAttachment;
    
    [Space]
    [SerializeField] protected XRGrabInteractable xrGrabInteractable;
    
    [Space]
    [SerializeField] protected ProduceData produceData;

    [SerializeField] protected ParticleSystem _particleSystem;
    [SerializeField] protected Transform particleAttachPoint;
    
    protected Transform attachment;
    protected Transform attachmentPlaceholder;
    
    protected GameObject connectedPrefabInstance;
    protected Rigidbody rb;

    private List<AudioSource> _hitSound;
    private float _onTriggerStayDuration;
    private int _audioId;
    private Image iconImageLeft;
    private Image iconImageRight;
    
    private bool _hitGround;
    private bool _hasReachedEnd;

    public bool hasBeenBinnedCorrectly;
    public bool wasBinnedWrong;

    protected virtual void Awake()
    {
        var iconCanvas = this.transform.Find("Icon Canvas");
        iconImageLeft = iconCanvas.transform.GetChild(0).GetComponent<Image>();
        iconImageRight = iconCanvas.transform.GetChild(1).GetComponent<Image>();

        xrGrabInteractable = GetComponent<XRGrabInteractable>();
        rb = this.gameObject.GetComponent<Rigidbody>();
        fixedJoint = GetComponent<FixedJoint>();

        _hitSound = new List<AudioSource>();
        foreach (var audioSource in GetComponents<AudioSource>())
        {
            audioSource.spatialBlend = 1f;
            _hitSound.Add(audioSource);
        }
    }

    private void Start()
    {
        iconImageLeft.color = GameSettings.instance.ColourDictionary[produceData.targetBinType];
        iconImageRight.color = GameSettings.instance.ColourDictionary[produceData.targetBinType];

        GameManager.ItemsInGame++;
        
        // HideUi();
        ShowUi(true);
    }

    protected void ActivateEvents()
    {
        xrGrabInteractable.selectEntered.AddListener(HandleOnSelectEnter);
        xrGrabInteractable.selectExited.AddListener(HandleOnSelectExit);
    }

    protected void DeactivateEvents()
    {
        xrGrabInteractable.selectEntered.RemoveListener(HandleOnSelectEnter);
        xrGrabInteractable.selectExited.RemoveListener(HandleOnSelectExit);
    }

    protected virtual void HandleOnSelectEnter(SelectEnterEventArgs args)
    {
        if (wasBinnedWrong)
        {
            wasBinnedWrong = false;
            DrumBin.HideErrorMessage?.Invoke();
        }
        
        var controller = args.interactorObject as XRBaseControllerInteractor;
        if (controller != null)
        {
            if (controller.name.Contains("Left"))
            {
                ShowUi(false);
            }
            else if (controller.name.Contains("Right"))
            {
                ShowUi(true);
            }
        }
    }

    protected virtual void HandleOnSelectExit(SelectExitEventArgs args)
    {
        if (GetType() == typeof(AttachmentBase)) return;
        if (attachment == null) return;
        
        if (attachment.GetComponent<AttachmentBase>().isAttached)
        {
            attachmentPlaceholder.gameObject.SetActive(true);
            attachment.gameObject.SetActive(false);
        }
        else
        {
            attachmentPlaceholder.gameObject.SetActive(false);
            attachment.gameObject.SetActive(true);
            Destroy(fixedJoint);
        }
    }

    public void RemoveFixedJoint()
    {
        hasAttachment = false;
        Destroy(fixedJoint);
    }

    protected virtual void ShowUi(bool isLeft)
    {
        HideUi();
    }

    protected virtual void HideUi()
    {
        iconImageLeft.enabled = false;
        iconImageRight.enabled = false;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        // if (other.gameObject.name == "Win Hit Box " + produceData.name)
        // {
        //     Destroy(this.gameObject);
        //     JoyScoreIncreased?.Invoke(1);
        // }

        if (other.gameObject.name.Contains("Belt Move Area"))
        {
            _onTriggerStayDuration = 0;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name is "Base")
        {
            // TempGlobalValues.Instance.BeltSpeed = TempGlobalValues.Instance.BeltSpeedMax;
            TempGlobalValues.Instance.isPressurePlateActive = false;
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Base" && !TempGlobalValues.Instance.isPressurePlateActive)
        {
            TempGlobalValues.Instance.isPressurePlateActive = true;
        }

        if (rb == null) return;

        if (other.gameObject.name.Contains("Belt Move Area"))
        {
            _onTriggerStayDuration += Time.deltaTime;

            if (rb.velocity.magnitude < 0.5f)
            {
                if (other.gameObject.name.Contains("Belt Move Area ("))
                {
                    if (other.gameObject.name.Contains("Belt Move Area (3)"))
                    {
                        if (!rb.isKinematic)
                        {
                            rb.velocity = new Vector3(rb.velocity.x + TempGlobalValues.Instance.BeltSpeed * 24f, rb.velocity.y, rb.velocity.z);
                        }
                    }
                    else
                    {
                        if (!rb.isKinematic)
                        {
                            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -TempGlobalValues.Instance.BeltSpeed * 10f);
                        }
                    }
                }
                else
                {
                    if (!rb.isKinematic)
                    {
                        // rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -TempGlobalValues.Instance.BeltSpeed * 10f);
                        rb.AddForce(new Vector3(rb.velocity.x, rb.velocity.y, -TempGlobalValues.Instance.BeltSpeed * 100f));
                    }
                }
            }
        }
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        MakeHitSound(other, true);

        if (other.gameObject.name.Contains("Ground"))
        {
            if (_hitGround) return;
            _hitGround = true;
            
            StartCoroutine(MakeSmoke());
            Destroy(this.gameObject, 2f);
            ConveyorBeltManager.SpawnNextItem?.Invoke(true);
        }
    }

    protected virtual void OnCollisionStay(Collision other)
    {
        if (_hasReachedEnd) return;
        MakeHitSound(other);
    }

    protected virtual void OnDestroy()
    {
        StopAllCoroutines();
        DeactivateEvents();
        GameManager.ItemsInGame--;
    }

    private void MakeHitSound(Collision other, bool alwaysPlay = false)
    {
        var rb = other.rigidbody;
        var currentObjectVelocity = GetComponent<Rigidbody>().velocity;

        if (rb != null)
        {
            var rnd = Random.Range(0, _hitSound.Count);
            rnd = _audioId;
            _audioId++;
            if (_audioId >= _hitSound.Count) _audioId = 0;

            var playRnd = Random.Range(0, 100);

            if (alwaysPlay || playRnd > 85 && currentObjectVelocity.magnitude > 1f && !_hitSound[rnd].isPlaying)
            {
                var volumeScale = currentObjectVelocity.normalized.magnitude / 2f;
                _hitSound[rnd].pitch = 1f - volumeScale;
                _hitSound[rnd].PlayOneShot(_hitSound[rnd].clip, 0.2f + volumeScale);
            }
        }
    }

    private IEnumerator MakeSmoke()
    {
        yield return new WaitForSeconds(1f);

        var ps = Instantiate(_particleSystem, particleAttachPoint.transform.position, Quaternion.Euler(Vector3.up));
        ps.transform.parent = this.transform;
        ps.Play();
    }

    public void DisableSocket()
    {
        // if (socket != null) socket.enabled = false;
        // attachmentSocket.GetComponent<XRSocketInteractor>().socketActive = false;
        // socket.socketActive = false;
        // Destroy(socket);
    }

    public static event Action<int> JoyScoreIncreased;
}