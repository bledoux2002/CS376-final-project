using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Status
{
    COASTING,
    BRAKING,
    BOOSTING,
    RECOVERING
}

public class PlayerController : MonoBehaviour
{
    private GameManager gm;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;
    public float VehicleShift { get; set; }
    private float boostFuel;
    [SerializeField] private float boostDrain;
    [SerializeField] private float boostRecover;
    [SerializeField] private float minBoost;
    [SerializeField] private TMP_Text boostText;
    public Status Status { get; set; }

    private InputAction _moveAction;
    private InputAction _boostAction;
    private InputAction _brakeAction;
    private InputAction _pauseAction;
    
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private AudioSource crashSound;

    private bool _crashed;
    
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        boostFuel = 100f;
        Status = Status.COASTING;
        
        _moveAction = InputSystem.actions.FindAction("Move");
        _boostAction = InputSystem.actions.FindAction("Boost");
        _brakeAction = InputSystem.actions.FindAction("Brake");
        _pauseAction = InputSystem.actions.FindAction("Pause");

        _crashed = false;
    }

    void Update()
    {
        if (_pauseAction.WasPressedThisDynamicUpdate())
        {
            gm.PauseGame();
        }
        Move();
        CheckBounds();
    }

    private void Move()
    {
        Vector2 input = _moveAction.ReadValue<Vector2>();
        VehicleShift = input.y * speed;
        Vector3 movement = new Vector3(input.x * speed, 0, 0);
        rb.AddForce(movement);

        bool boostPressed = _boostAction.IsPressed();
        if (boostPressed && boostFuel > 0f)
        {
            if (Status != Status.RECOVERING && Status != Status.BRAKING)
            {
                Status = Status.BOOSTING;
                boostFuel -= boostDrain * Time.deltaTime;
                if (engineSound.pitch < 1f) engineSound.pitch += Time.deltaTime;
                if (engineSound.pitch > 1f) engineSound.pitch = 1f;
                if (boostFuel < 0f)
                {
                    boostFuel = 0f;
                    Status = Status.RECOVERING;
                }
            }
        }
        else
        {
            bool brakePressed = _brakeAction.IsPressed();
            if (boostFuel < 100f) boostFuel += boostRecover * Time.deltaTime;
            if (boostFuel > 100f) boostFuel = 100f;
            if (brakePressed)
            {
                Status = Status.BRAKING;
            }
            else if (boostFuel < minBoost)
            {
                Status = Status.RECOVERING;
            }
            else
            {
                Status = Status.COASTING;
            }
            if (engineSound.pitch > 0.75f) engineSound.pitch -= Time.deltaTime;
            if (engineSound.pitch < 0.75f) engineSound.pitch = 0.75f;
        }
        boostText.text = $"{(int)boostFuel}";
    }

    private void CheckBounds()
    {
        if (!_crashed && (transform.position.x > 5f || transform.position.x < -5f))
        {
            Crash();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Vehicle"))
        {
            Crash();
        }
    }

    private void Crash()
    {
        _crashed = true;
        crashSound.Play();
        engineSound.Stop();
        gm.GameOver();
    }

    public void Reset()
    {
        rb.linearVelocity = Vector3.zero;
        rb.Sleep();
        rb.WakeUp();
        transform.position = new Vector3(2.5f, 0f, -25f);
        boostFuel = 100f;
        Status = Status.COASTING;
        _crashed = false;
        engineSound.pitch = 0.75f;
        engineSound.Play();
    }
}
