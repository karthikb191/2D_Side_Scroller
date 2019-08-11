using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAudios
{
    public AudioClip run;
    public AudioClip jump;
    public AudioClip slide;
    public AudioClip death;
}

public class PlayerMovement : MonoBehaviour {

    //private CharacterController playerController;
    [SerializeField]
    private float maxSpeed = 20.0f;
    [SerializeField]
    private float jumpForce = 20.0f;
    [SerializeField]
    private float gravity = -9.8f;
    public LayerMask whatArePlatforms;
    [SerializeField]
    private PlayerAudios playerAudios;

    private Vector3 initialPosition;
    private Vector3 moveVector = Vector3.zero;
    private float verticalSpeed = 0;
    private float horizontalSpeed = 0;

    private CircleCollider2D circleCollider;
    private Animator animator;
    private AudioSource audioSource;
    


    private bool grounded = true;
    private bool sliding = false;
    private bool dead = false;
    public bool Dead
    {
        get
        {
            return dead;
        }
    }

    private int health;
    public int Health
    {
        get
        {
            return health;
        }
    }
    

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        health = 100;
        initialPosition = gameObject.transform.position;
        GameManager.Instance.ResetGameEvent += ResetPlayer;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.transform.parent.tag == "Hazard")
        {
            health -= 20;
            audioSource.PlayOneShot(playerAudios.death);
            //ChangeAudioClip(playerAudios.death, false);
        }
    }

    // Update is called once per frame
    void Update() {
        //Do nothing until the game starts
        if (!GameManager.Instance.GameStarted || dead)
            return;
        
        gameObject.transform.position += Move();
        SetAnimations();
        CheckIfBelowGround();
        dead = IsPlayerDead();
	}

    void SetAnimations()
    {
        animator.speed = DifficultyManager.Instance.DifficultyParameter;
        animator.SetFloat("HSpeed", horizontalSpeed);
        animator.SetBool("Grounded", grounded);
        animator.SetBool("Slide", sliding);
        
    }

    Vector3 Move()
    {
        //Vector3 result = Vector3.zero;
        Vector3 targetPosition = Vector3.zero;

        //TODO: change this to a much smoother value
        horizontalSpeed = maxSpeed;


        ReadInputsAndSetValues();

        //Debug.Log(DifficultyManager.Instance.DifficultyParameter);
        targetPosition = (transform.right * horizontalSpeed * DifficultyManager.Instance.DifficultyParameter
                            + new Vector3(0, verticalSpeed, 0))* Time.deltaTime * Time.deltaTime;

        targetPosition.y = Mathf.Clamp(targetPosition.y, -1, 1);

        return targetPosition;
    }

    void ReadInputsAndSetValues()
    {
        Vector3 result = Vector3.zero;
        
        if (IsGrounded())
        {
            grounded = true;

            //Default running audio
            if(!sliding)
                ChangeAudioClip(playerAudios.run, false);


            //Debug.Log(verticalSpeed);
            if (verticalSpeed < 10.0f)
                verticalSpeed = -50f;


            if (VirtualControls.Instance.ShortSwipeUp)
            {
                verticalSpeed = jumpForce * 0.6f;

                //setting jump audio
                ChangeAudioClip(playerAudios.jump, false);
            }
            if (Input.GetKeyDown(KeyCode.Space) || VirtualControls.Instance.LongSwipeUp)
            {
                verticalSpeed = jumpForce;

                //Setting jump audio
                ChangeAudioClip(playerAudios.jump, false);
            }
            if (VirtualControls.Instance.SwipeDown)
            {
                if (!sliding)
                {
                    sliding = true;
                    circleCollider.offset = new Vector2(0, -0.5f);
                    circleCollider.radius = circleCollider.radius / 2;
                    StartCoroutine(StopSlide());

                    //setting slide audio
                    ChangeAudioClip(playerAudios.slide, false);
                }
            }
        }
        else
        {
            grounded = false;
            verticalSpeed += gravity;

            if (Input.GetKeyDown(KeyCode.Space) || VirtualControls.Instance.SwipeDown)
                verticalSpeed = -jumpForce;
        }
    }

    IEnumerator StopSlide()
    {
        yield return new WaitForSeconds(1.0f);
        sliding = false;
        circleCollider.offset = new Vector2(0, 0);
        circleCollider.radius = circleCollider.radius * 2;

    }


    bool IsGrounded()
    {
        //distanceToGround = 0;
        //cast a circle with a certain radius
        Vector3 startPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - circleCollider.bounds.extents.y, gameObject.transform.position.z);
        Vector3 dir = -gameObject.transform.up * -Mathf.Clamp(verticalSpeed, -1, 0);

        Ray2D r = new Ray2D(startPos, dir);
        Debug.DrawRay(startPos, dir, Color.red);
        RaycastHit2D hit = Physics2D.CircleCast(startPos, 0.5f, dir, 0.0f, whatArePlatforms);

        //Orienting the player
        if (hit.collider != null)
        {
            
            //distanceToGround = Mathf.Abs(hit.point.y - (gameObject.transform.position.y - circleCollider.bounds.extents.y));
            //OrientPlayer();
            float angle = Vector3.Angle(gameObject.transform.up, hit.normal);
            if (angle < 65)
            {
                //Debug.Log(angle);
                if (angle > 0.01f)
                    dir = Vector3.Lerp(gameObject.transform.up, hit.normal, 0.1f);
                else
                    dir = hit.normal;

                gameObject.transform.up = dir;
            }
        }
        else
        {
            // Debug.Log("Else block executing");
            dir = Vector3.Lerp(gameObject.transform.up, Vector3.up, 0.1f);
            gameObject.transform.up = dir;

        }
        
        return hit;
        //return false;
    }
    
    void CheckIfBelowGround()
    {
        if (gameObject.transform.position.y < -10)
            health -= 100;
        health = Mathf.Clamp(health, 0, 100);
    }

    void ChangeAudioClip(AudioClip clip, bool loop)
    {

        audioSource.pitch = DifficultyManager.Instance.DifficultyParameter;
        audioSource.loop = loop;
        audioSource.clip = clip;
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    bool IsPlayerDead()
    {
        if (health <= 0)
        {
            ChangeAudioClip(playerAudios.death, false);
            gameObject.transform.GetChild(1).gameObject.SetActive(true);    //enable particle system
            gameObject.transform.GetChild(0).gameObject.SetActive(false);   //disable sprite
            return true;
        }
        return false;
    }

    void ResetPlayer()
    {
        health = 100;
        gameObject.transform.position = initialPosition;
        dead = false;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);    //enable the sprite
        gameObject.transform.GetChild(1).gameObject.SetActive(false);   //disable the particle effect
    }
}
