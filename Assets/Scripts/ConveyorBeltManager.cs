using System;
using System.Collections;
using System.Collections.Generic;
using JugendBlitz.Game_Data.SO;
using JugendBlitz.scripts.runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ConveyorBeltManager : SerializedMonoBehaviour
{
    [OdinSerialize] private ItemDataSo _itemDataSo;

    // [Header("Original Settings")]
    // [SerializeField] private List<Transform> trashPrefabsEasy;
    // [SerializeField] private int trashPrefabsEasyCounter;
    // [SerializeField] private List<Transform> trashPrefabsMedium;
    // [SerializeField] private int trashPrefabsMediumCounter;
    // [SerializeField] private List<Transform> trashPrefabsHard;
    // [SerializeField] private int trashPrefabsHardCounter;
    //
    // [SerializeField] private Transform trashAudioSpherePrefabs;
    // // [SerializeField] private List<GameObject> trashCollection;
    // [SerializeField] private float spawnInterval = 4.0f;
    // [SerializeField] private Transform itemSpawnPoint;
    // [SerializeField] private List<Transform> audioSphereSpawnPoint;
    // // [SerializeField] private TMP_Text itemsLeftReadout;
    // [SerializeField] private GameObject Blocker;
    // [SerializeField] private Button restartHiddenImage;
    // public float duration = 0.2f;
    
    public static Action OnStartGame;
    public static Action OnEndGame;
    public static Action<bool> SpawnNextItem;
    public static Action OnSpawnWinningEnvelope;
    
    private Animation _flipTableAnimations;

    private int _itemCount = 0;

    private void Awake()
    {
        _flipTableAnimations = GameObject.Find("Flip_Table").GetComponent<Animation>();
        _itemDataSo.restartHiddenImage = GameObject.Find("Restart Hidden Image").GetComponent<Button>();

        // itemDataSo.itemsLeftReadout = GameObject.Find("Readout Text").GetComponent<TMP_Text>();

        _itemDataSo.itemSpawnPoint = GameObject.Find("Item Spawn Point").GetComponent<Transform>();
        _itemDataSo.blocker = GameObject.Find("Blocker").GetComponent<Transform>().gameObject;

        _itemDataSo.audioSphereSpawnPoint = new List<Transform>(3)
        {
            GameObject.Find("Item Spawn Point (1)").GetComponent<Transform>(),
            GameObject.Find("Item Spawn Point (2)").GetComponent<Transform>(),
            GameObject.Find("Item Spawn Point (3)").GetComponent<Transform>()
        };

        _itemDataSo.restartHiddenImage.gameObject.SetActive(false);
        MakeBottleNonClickable();
    }

    private void Start()
    {
        ZeroCounters();
        
        ShuffleList(_itemDataSo.trashPrefabsEasy);
        ShuffleList(_itemDataSo.trashPrefabsMedium);
        ShuffleList(_itemDataSo.trashPrefabsHard);

        _itemDataSo.blocker.SetActive(false);
        UIPanelManager.showRightUI?.Invoke();

        // if (Application.isEditor) OnEnable();

        AudioSphere.SpawnProduce += SpawnTrash;
        OnStartGame += StartGame;
        OnEndGame += EndGame;
        SpawnNextItem += SpawnRandomAudioSphere;
        OnSpawnWinningEnvelope += OnSpawnEnvelope;
        _itemDataSo.restartHiddenImage.onClick.AddListener(OnBottleClicked);
    }

    private void ZeroCounters()
    {
        _itemDataSo.trashPrefabsEasyCounter = 0;
        _itemDataSo.trashPrefabsMediumCounter = 0;
        _itemDataSo.trashPrefabsHardCounter = 0;
    }

    private static void ShuffleList(IList<Transform> transforms)
    {
        for (var i = 0; i < transforms.Count; i++)
        {
            var temp = transforms[i];
            var randomIndex = Random.Range(i, transforms.Count);
            transforms[i] = transforms[randomIndex];
            transforms[randomIndex] = temp;
        }
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UIPanelManager.toggleHideLeftUI?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha2)) UIPanelManager.toggleHideRightUI?.Invoke();
        if (Input.GetKeyDown(KeyCode.Space)) onSpawnWinningEnvelope?.Invoke();

        if (Input.GetKeyDown(KeyCode.T))
        {
            StopAllCoroutines();
            StartCoroutine(ChangeTimeScale(0f));
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            StopAllCoroutines();
            StartCoroutine(ChangeTimeScale(1f));
        }
    }*/

    // private void UIPanelManagerOnShowHideRightUI() { }

    /*private void OnEnable()
    {
        AudioSphere.SpawnProduce += SpawnTrash;
        startGame += StartGame;
        endGame += EndGame;
        spawnNextItem += SpawnRandomAudioSphere;
        onSpawnWinningEnvelope += OnSpawnEnvelope;
        GotoTimeSpeed += SetTimeTo;
        itemDataSo.restartHiddenImage.onClick.AddListener(OnBottleClicked);
    }

    private void OnDisable()
    {
        AudioSphere.SpawnProduce -= SpawnTrash;
        startGame -= StartGame;
        endGame -= EndGame;
        spawnNextItem -= SpawnRandomAudioSphere;
        onSpawnWinningEnvelope -= OnSpawnEnvelope;
        GotoTimeSpeed -= SetTimeTo;
        itemDataSo.restartHiddenImage.onClick.RemoveListener(OnBottleClicked);
    }*/

    private void OnBottleClicked()
    {
        ShowStartMenu();
    }

    private void OnDestroy()
    {
        ZeroCounters();
        AudioSphere.SpawnProduce -= SpawnTrash;
        OnStartGame -= StartGame;
        OnEndGame -= EndGame;
        SpawnNextItem -= SpawnRandomAudioSphere;
        OnSpawnWinningEnvelope -= OnSpawnEnvelope;
        StopAllCoroutines();
        TempGlobalValues.Instance.isPressurePlateActive = false;
    }

    private void SetTimeTo(float target)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeTimeScale(target));
    }

    private IEnumerator ChangeTimeScale(float target)
    {
        var start = Time.time;
        var initialTimeScale = Time.timeScale;

        while (Time.time - start < _itemDataSo.duration)
        {
            Time.timeScale = Mathf.Lerp(initialTimeScale, target, (Time.time - start) / _itemDataSo.duration);
            yield return null;
        }

        Time.timeScale = target;
    }

    private void OnSpawnEnvelope()
    {
        // TODO: Spawn envelope.
        Debug.Log("Spawn envelope");
        // UIPanelManager.showLeftUI?.Invoke();

        _flipTableAnimations.Play("Flip Table Animations");
        OnEndGame?.Invoke();

        UIPanelManager.showRightUI?.Invoke();

        // StartCoroutine(ShowStartMenuWithDelay());

        MakeBottleClickable();
    }

    private void MakeBottleClickable() => _itemDataSo.restartHiddenImage.gameObject.SetActive(true);

    private void MakeBottleNonClickable() => _itemDataSo.restartHiddenImage.gameObject.SetActive(false);

    private void ShowStartMenu()
    {
        UIPanelManager.showRightUI?.Invoke();
        _flipTableAnimations.Play("Flip Table Back Again");
        MakeBottleNonClickable();
        InstructionsImageSlider.ShowStartMenuClicked?.Invoke();
    }

    private void StartGame()
    {
        ZeroCounters();
        MakeBottleNonClickable();

        _itemDataSo.blocker.SetActive(true);
        _itemCount = 0;
        if (GameManager.DidWinGame)
        {
            _flipTableAnimations.Play("Flip Table Back Again");
        }

        SpawnRandomAudioSphere();

        StartCoroutine(DelayedSpawn());
    }

    private IEnumerator DelayedSpawn()
    {
        while (GameManager.GameInProgress)
        {
            yield return new WaitForSeconds(4);
            SpawnRandomAudioSphere();
        }
    }

    private void EndGame()
    {
        _itemCount = 0;
        _itemDataSo.blocker.SetActive(false);
        _itemDataSo.restartHiddenImage.gameObject.SetActive(true);
    }

    private void SpawnTrash()
    {
        if (!GameManager.GameInProgress) return;

        _itemCount--;
        var spawnedTrash = Instantiate(GetRandomTrashItem(), 
            _itemDataSo.itemSpawnPoint.position, 
            Quaternion.identity,
            TempGlobalValues.Instance.spawnLocation);
    }

    private IEnumerator SpawnAudioSphereLoop()
    {
        yield return new WaitForEndOfFrame();

        while (true)
        {
            if (TempGlobalValues.Instance.BeltSpeed > 0.1f) SpawnRandomAudioSphere();

            yield return new WaitForSeconds(_itemDataSo.spawnInterval);
        }
    }

    private void SpawnAudioSphere()
    {
        // if (TempGlobalValues.Instance.BeltSpeed > 0.1f) SpawnRandomAudioSphere();
        SpawnRandomAudioSphere();
    }

    private void SpawnRandomAudioSphere(bool hitGround = false)
    {
        if (!GameManager.GameInProgress) return;

        if (_itemCount > 4 || GameManager.ItemsInGame > 4) return;
        _itemCount++;

        var rnd = Random.Range(0, _itemDataSo.audioSphereSpawnPoint.Count);
        var spawnedTrash = Instantiate(_itemDataSo.trashAudioSpherePrefabs,
            _itemDataSo.audioSphereSpawnPoint[rnd].position,
            _itemDataSo.audioSphereSpawnPoint[rnd].rotation,
            TempGlobalValues.Instance.spawnLocation);
        spawnedTrash.GetComponent<Rigidbody>().AddForce(Vector3.down * 2500f, ForceMode.Acceleration);
    }

    private Transform GetRandomTrashItem()
    {
        Transform item;
        int rnd;

        switch (GameSettings.instance.CorrectItemCounter)
        {
            case < 10:
            {
                item = _itemDataSo.trashPrefabsEasy[_itemDataSo.trashPrefabsEasyCounter];
                _itemDataSo.trashPrefabsEasyCounter++;
                if (_itemDataSo.trashPrefabsEasyCounter >= _itemDataSo.trashPrefabsEasy.Count)
                    _itemDataSo.trashPrefabsEasyCounter = 0;
                break;
            }
            case < 15:
            {
                item = _itemDataSo.trashPrefabsMedium[_itemDataSo.trashPrefabsMediumCounter];
                _itemDataSo.trashPrefabsMediumCounter++;
                if (_itemDataSo.trashPrefabsMediumCounter >= _itemDataSo.trashPrefabsMedium.Count)
                    _itemDataSo.trashPrefabsMediumCounter = 0;
                break;
            }
            default:
            {
                item = _itemDataSo.trashPrefabsHard[_itemDataSo.trashPrefabsHardCounter];
                _itemDataSo.trashPrefabsHardCounter++;
                if (_itemDataSo.trashPrefabsHardCounter >= _itemDataSo.trashPrefabsHard.Count)
                    _itemDataSo.trashPrefabsHardCounter = 0;
                break;
            }
        }

        return item;
    }

    private void SetReadout(int val)
    {
        // itemDataSo.itemsLeftReadout.text = val.ToString("00");
    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     trashCollection.Add(collision.gameObject);
    // }

    // void OnCollisionExit(Collision collision)
    // {
    //     trashCollection.Remove(collision.gameObject);
    // }

    // public void StartMoving()
    // {
    //     for (int i = 0; i < trashCollection.Count; i++)
    //     {
    //         Debug.Log("Touching object: " + trashCollection[i].name);
    //         trashCollection[i].GetComponent<Rigidbody>().AddForce(0, 0, -0.5f * Time.deltaTime, ForceMode.Impulse);
    //     }
    // }
}