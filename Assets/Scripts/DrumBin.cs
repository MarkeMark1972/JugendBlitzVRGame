using System;
using System.Collections;
using System.Collections.Generic;
using JugendBlitz.scripts.runtime;
using UnityEngine;
using UnityEngine.UI;

public class DrumBin : MonoBehaviour
{
    [SerializeField] private ParticleSystem winParticleSystem;
    [SerializeField] private ParticleSystem loseParticleSystem;
    [SerializeField] private TargetBinType binType;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private Color binColour;
    [SerializeField] private MeshRenderer binMeshRenderer;
    [SerializeField] private Image labelImage;

    private Image _carefulErrorImage;

    private Image _wrongBinErrorImage;
    // private TMP_Text _leftErrorTitle;
    // private TMP_Text _leftErrorDescription;
    // private TMP_Text _rightErrorTitle;
    // private TMP_Text _rightErrorDescription;

    private AudioSource _winAudio;
    private AudioSource _loseAudio;

    private bool _isPlayingAudio;

    public static Action HideErrorMessage;

    private void Awake()
    {
        _winAudio = GetComponents<AudioSource>()[0];
        _loseAudio = GetComponents<AudioSource>()[1];

        _carefulErrorImage = GameObject.Find("Error UI Panel Careful Image").GetComponent<Image>();
        _wrongBinErrorImage = GameObject.Find("Error UI Panel Wrong Bin Image").GetComponent<Image>();
        // _leftErrorTitle = GameObject.Find("Left Error UI Panel Title Text").GetComponent<TMP_Text>();
        // _leftErrorDescription = GameObject.Find("Left Error UI Panel Description Text").GetComponent<TMP_Text>();
        // _rightErrorTitle = GameObject.Find("Right Error UI Panel Title Text").GetComponent<TMP_Text>();
        // _rightErrorDescription = GameObject.Find("Right Error UI Panel Description Text").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        HideAllText();

        // labelImage.sprite = sprites[(int)binType];

        binColour = GameSettings.instance.ColourDictionary[binType];
        // labelImage.color = binColour;
        binMeshRenderer.material.color = binColour;
    }

    private void OnEnable()
    {
        HideErrorMessage += OnHideErrorMessage;
    }

    private void OnDisable()
    {
        HideErrorMessage -= OnHideErrorMessage;
    }

    private void OnHideErrorMessage()
    {
        _carefulErrorImage.enabled = false;
        _wrongBinErrorImage.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var isAHand = other.name.Contains("Left") || other.name.Contains("Right");
        if (isAHand) return;

        if (_isPlayingAudio) return;

        var baseScript = other.gameObject.GetComponent<BaseClass>();
        if (baseScript != null)
        {
            if (baseScript is TrashBase)
            {
                if (baseScript.targetBinType == binType && !baseScript.hasAttachment)
                {
                    if (!baseScript.hasBeenBinnedCorrectly)
                    {
                        PlayerWin();
                        Destroy(other.gameObject, 2f);
                        baseScript.hasBeenBinnedCorrectly = true;
                    }
                }
                else
                {
                    PlayerLose();
                    StartCoroutine(TryAgain(other, "Try Again Spawn Point Left", baseScript));
                }
            }
            else if (baseScript is AttachmentBase)
            {
                if (baseScript.targetBinType == binType)
                {
                    if (!baseScript.hasBeenBinnedCorrectly)
                    {
                        PlayerWin();
                        Destroy(other.gameObject, 2f);
                        baseScript.hasBeenBinnedCorrectly = true;
                    }
                }
                else
                {
                    PlayerLose();
                    StartCoroutine(TryAgain(other, "Try Again Spawn Point Right", baseScript));
                }
            }

            // TODO: If both base and attachment are in correct bins then spawn next item.
            // ConveyorBeltManager.spawnNextItem?.Invoke();
        }
    }

    private IEnumerator TryAgain(Collider other, string spawnerName, BaseClass baseScript)
    {
        yield return new WaitForSeconds(2f);

        var pos = GameObject.Find(spawnerName);
        other.transform.rotation = Quaternion.identity;
        other.transform.position = pos.transform.position;

        baseScript.wasBinnedWrong = true;

        // if (spawnerName.Contains("Left"))
        // {
        var trashBase = other.GetComponent<TrashBase>();
        // _leftErrorTitle.text = "Wrong Bin";
        // _leftErrorDescription.text = "WRONG -> " + trashBase.name;
        ShowLeftText();
        // StartCoroutine(HideLeftTextDelayed());

        if (baseScript.hasAttachment)
        {
            _carefulErrorImage.enabled = true;
            _wrongBinErrorImage.enabled = false;
        }
        else
        {
            _carefulErrorImage.enabled = false;
            _wrongBinErrorImage.enabled = true;
        }

        // ConveyorBeltManager.GotoTimeSpeed(0);
        // }
        // else
        // {
        //     var attachmentBase = other.GetComponent<AttachmentBase>();
        //     // _rightErrorTitle.text = "Wrong Bin";
        //     // _rightErrorDescription.text = "WRONG -> " + attachmentBase.name;
        //     ShowRightText();
        //     StartCoroutine(HideRightTextDelayed());
        //     // ConveyorBeltManager.GotoTimeSpeed(0);
        // }
    }

    private void PlayerWin()
    {
        // ConveyorBeltManager.GotoTimeSpeed(1);
        winParticleSystem.Play();
        _winAudio.PlayOneShot(_winAudio.clip);
        _isPlayingAudio = true;
        // StartCoroutine(PlayDelay());
        _isPlayingAudio = false;
        ConveyorBeltManager.SpawnNextItem?.Invoke(false);
    }

    private void PlayerLose()
    {
        loseParticleSystem.Play();
        _loseAudio.PlayOneShot(_loseAudio.clip);
        _isPlayingAudio = true;
        // StartCoroutine(PlayDelay());
        _isPlayingAudio = false;
    }

    private IEnumerator PlayDelay()
    {
        yield return new WaitForSeconds(2f);
        _isPlayingAudio = false;
    }

    private void HideAllText()
    {
        HideLeftText();
        HideRightText();
    }

    private void ShowLeftText()
    {
        // _leftErrorTitle.gameObject.SetActive(true);
        // _leftErrorDescription.gameObject.SetActive(true);
        _carefulErrorImage.enabled = true;
        _wrongBinErrorImage.enabled = false;
    }

    private void HideLeftText()
    {
        // _leftErrorTitle.gameObject.SetActive(false);
        // _leftErrorDescription.gameObject.SetActive(false);
        _carefulErrorImage.enabled = false;
        _wrongBinErrorImage.enabled = false;
    }

    private void ShowRightText()
    {
        // _rightErrorTitle.gameObject.SetActive(true);
        // _rightErrorDescription.gameObject.SetActive(true);
    }

    private void HideRightText()
    {
        // _rightErrorTitle.gameObject.SetActive(false);
        // _rightErrorDescription.gameObject.SetActive(false);
    }

    private IEnumerator HideLeftTextDelayed()
    {
        yield return new WaitForSeconds(2f);
        HideLeftText();
    }

    private IEnumerator HideRightTextDelayed()
    {
        yield return new WaitForSeconds(2f);
        HideRightText();
    }
}