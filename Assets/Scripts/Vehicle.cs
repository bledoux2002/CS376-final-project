using System;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public GameManager GameManager { get; set; }
    public bool Forward { get; set; }
    public float Speed { get; set; }
    public bool Passed { get; set; }

    private float _playerZ;

    private void Start()
    {
        _playerZ = FindFirstObjectByType<PlayerController>().transform.position.z;
    }

    void Update()
    {
        if (!Forward)
        {
            transform.Translate(Time.deltaTime * Speed * Vector3.forward);
        }
        else if ((!Passed && transform.position.z < _playerZ) || (Passed && transform.position.z > _playerZ))
        {
                Passed = !Passed;
                GameManager.ChangeScore(Passed ? 1 : -1);
        }
        if (transform.position.z < -GameManager.spawnBackwardMax)
        {
            GameManager.Despawn(Forward);
        }
    }
}
