using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioSphere : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    public static event Action SpawnProduce;

    private List<AudioSource> _hitSound;
    private bool _hasReachedEnd;
    private int _audioId;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        label.enabled = false;

        _hitSound = new List<AudioSource>();
        foreach (var audioSource in GetComponents<AudioSource>())
        {
            audioSource.spatialBlend = 1f;
            _hitSound.Add(audioSource);
        }
    }

    private void Update()
    {
        this.transform.LookAt(Camera.main.transform, Vector3.up);

        if (_rb.velocity.magnitude < 0.02)
        {
            _rb.AddForce(new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)), ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        MakeHitSound(other, true);
    }

    private void OnCollisionStay(Collision other)
    {
        if (_hasReachedEnd) return;

        MakeHitSound(other);
    }

    private void MakeHitSound(Collision other, bool alwaysPlay = false)
    {
        var rb = other.rigidbody;
        var currentObjectVelocity = _rb.velocity;
        label.text = currentObjectVelocity.magnitude.ToString();

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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name.Contains("Belt Move Area (3)"))
        {
            if (_hasReachedEnd) return;
            _hasReachedEnd = true;
            SpawnProduce?.Invoke();
            _rb.Sleep();
            Destroy(this.gameObject, 2f);
        }
    }
}