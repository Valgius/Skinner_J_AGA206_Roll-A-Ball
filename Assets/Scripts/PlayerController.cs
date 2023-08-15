using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    [HideInInspector]
    public float baseSpeed;
    private Rigidbody rb;
    private int pickupCount;
    GameObject resetPoint;
    bool resetting = false;
    bool grounded = true;
    bool activePowerup = false;
    Color originalColour;
    private Timer timer;
    private bool gameOver;

    //Controllers
    CameraController cameraController;

    [Header("UI")]
    public GameObject inGamePanel;
    public GameObject gameOverPanel;
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text winTimeText;

    // Start is called before the first frame update
    void Start()
    {
        baseSpeed = speed;
        rb = GetComponent<Rigidbody>();

        //Get the number of pickups in our scene
        pickupCount = GameObject.FindGameObjectsWithTag("Pick Up").Length;
        //Run the check pickips function
        SetCountText();
        //Get the timer object ans start the timer
        timer = FindObjectOfType<Timer>();
        timer.StartTimer();

        //Turn on our In Game Panel
        inGamePanel.SetActive(true);
        //Turn off our Game Over Panel
        gameOverPanel.SetActive(false);

        //Enables reset point
        resetPoint = GameObject.Find("Reset Point");
        originalColour = GetComponent<Renderer>().material.color;

        cameraController = FindObjectOfType<CameraController>();

        Time.timeScale = 1;
    }

    private void Update()
    {
        timerText.text = "Time: " + timer.GetTime().ToString("F2");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameOver == true)
                return;

        if (resetting)
            return;

        if (grounded)
        {
            // Character Movement
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

            if (cameraController.cameraStyle == CameraStyle.Free)
            {
                //rotates the player to the direction of the camera
                transform.eulerAngles = Camera.main.transform.eulerAngles;
                //translates the input vectors into coordinates
                movement = transform.TransformDirection(movement);
            }

            rb.AddForce(movement * speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pick Up")
        {
            Destroy(other.gameObject);
            //Decrement the pickup count
            pickupCount -= 1;
            //Run the check pickips function
            SetCountText();
        }

        if(other.gameObject.CompareTag("Powerup"))
        {
            activePowerup = true;
            other.GetComponent<Powerup>().UsePowerup();
        }
    }

    void SetCountText()
    {
        //Display the ammount of pickups left in out scene
        scoreText.text = "Pickups Left: " + pickupCount;

        // Win condition 
        if (pickupCount == 0)
        {
            WinGame();

        }
    }

    void WinGame()
    {
        //Set the game over to true
        gameOver = true;
        //Stop the timer
        timer.StopTimer();
        //Turn on our Win Panel    
        gameOverPanel.SetActive(true);
        //Turn off our In Game Panel
        inGamePanel.SetActive(false);
        //Display the timer on the win time text
        winTimeText.text = "Your time was: " + timer.GetTime().ToString("F2");

        //Set the Velocity of the rigidbody to zero
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
        {
            StartCoroutine(ResetPlayer());
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (activePowerup == false)
            {
                StartCoroutine(ResetPlayer());
            }
            else
            {
                //Logic for killing enemy
                Destroy(collision.gameObject);
            }
        } 
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            grounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            grounded = false;
    }

    public IEnumerator ResetPlayer()
    {
        resetting = true;
        GetComponent<Renderer>().material.color = Color.white;
        rb.velocity = Vector3.zero;
        Vector3 startPos = transform.position;
        float resetSpeed = 2f;
        var i = 0.0f;
        var rate = 1.0f / resetSpeed;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            transform.position = Vector3.Lerp(startPos, resetPoint.transform.position, i);
            yield return null;
        }
        GetComponent<Renderer>().material.color = originalColour;
        resetting = false;

    }
}
