using System;
using UnityEngine;

public sealed class TempGlobalValues : MonoBehaviour
{
    public static TempGlobalValues Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        } 
        else 
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        
        // Instance.spawnLocation = GameObject.Find("Produce Spawn Holder").transform;
    }
    
    public float BeltSpeed = 0.8f;
    public float BeltSpeedMax = 0.4f;
    public bool isPressurePlateActive = true;
    public Transform spawnLocation;
    
    [Range(0f, 0.05f)]
    [SerializeField] public float accelerationDelay = 0.002f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPressurePlateActive = !isPressurePlateActive;
            // BeltSpeed = Math.Abs(BeltSpeed - BeltSpeedMax) < 0.01f ? 0 : BeltSpeedMax;
        }
    }
}