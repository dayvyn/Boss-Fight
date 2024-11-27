using System.Globalization;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace DavinB
{
    public class CharacterMovement : MonoBehaviour
    {
        IA_PlayerInput input;
        public float speed = 7.0f;
        public float airVelocity = 8f;
        public float gravity = 10.0f;
        public float maxVelocityChange = 10.0f;
        public float jumpHeight = 2.0f;
        public float maxFallSpeed = 20.0f;
        public float rotateSpeed = 25f; //Speed the player rotate
        private Vector3 moveDir;
        public GameObject cam;
        private Rigidbody rb;
        const int maxHealth = 20;
        private int currentHealth;
        public int CurrentHealth { get => currentHealth; private set { currentHealth = value; } }

        private float distToGround;

        private bool canMove = true; //If player is not hitted
        private bool isStuned = false;
        private bool wasStuned = false; //If player was stunned before get stunned another time
        private float pushForce;
        private Vector3 pushDir;

        private int number;
        public float vForce;
        public float hForce;
        private bool slide = false;
        public int place;
        bool jumping;

        public UnityAction PickedUp;
        public UnityAction<int> LoseHealth;
        public bool pickedUpAlready = false;
        Ground ground;

        private void OnEnable()
        {
            input.Enable();
            PickedUp += PickedUpFunction;
            ground = FindAnyObjectByType<Ground>();
            currentHealth = maxHealth;
            LoseHealth += OnHealthLoss;
        }

        private void OnDisable()
        {
            input.Disable();
        }

        void Start()
        {
            // get the distance to ground
            distToGround = GetComponent<Collider>().bounds.extents.y;

            input.Player.Move.performed += ctx =>
            {
                vForce = input.Player.Move.ReadValue<Vector2>().x * speed;
                hForce = input.Player.Move.ReadValue<Vector2>().y * speed;
            };

            input.Player.Move.canceled += ctx => { hForce = 0.0f; vForce = 0.0f; };

            input.Player.Jump.performed += ctx =>
            {
                jumping = true;
            };

            input.Player.Jump.canceled += ctx => { jumping = false; };
        }

        bool IsGrounded()
        {
            return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
        }

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            rb.useGravity = false;
            input = new IA_PlayerInput();

            Cursor.visible = false;
        }

        void FixedUpdate()
        {
            if (canMove)
            {
                if (moveDir.x != 0 || moveDir.z != 0)
                {
                    Vector3 targetDir = moveDir; //Direction of the character

                    targetDir.y = 0;
                    if (targetDir == Vector3.zero)
                        targetDir = transform.forward;
                    Quaternion tr = Quaternion.LookRotation(targetDir); //Rotation of the character to where it moves
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * rotateSpeed); //Rotate the character little by little
                    transform.rotation = targetRotation;
                }

                if (IsGrounded())
                {
                    // Calculate how fast we should be moving
                    Vector3 targetVelocity = moveDir;
                    targetVelocity *= speed;

                    // Apply a force that attempts to reach our target velocity
                    Vector3 velocity = rb.velocity;
                    if (targetVelocity.magnitude < velocity.magnitude) //If I'm slowing down the character
                    {
                        targetVelocity = velocity;
                        rb.velocity /= 1.1f;
                    }
                    Vector3 velocityChange = targetVelocity - velocity;
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                    velocityChange.y = 0;
                    if (!slide)
                    {
                        if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
                            rb.AddForce(velocityChange, ForceMode.VelocityChange);
                    }
                    else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
                    {
                        rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
                        //Debug.Log(rb.velocity.magnitude);
                    }

                    // Jump
                    if (IsGrounded() && jumping)
                    {
                        rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                    }
                }
                else
                {
                    if (!slide)
                    {
                        Vector3 targetVelocity = new Vector3(moveDir.x * airVelocity, rb.velocity.y, moveDir.z * airVelocity);
                        Vector3 velocity = rb.velocity;
                        Vector3 velocityChange = targetVelocity - velocity;
                        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                        rb.AddForce(velocityChange, ForceMode.VelocityChange);
                        if (velocity.y < -maxFallSpeed)
                            rb.velocity = new Vector3(velocity.x, -maxFallSpeed, velocity.z);
                    }
                    else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
                    {
                        rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
                    }
                }
            }
            else
            {
                rb.velocity = pushDir * pushForce;
            }
            // We apply gravity manually for more tuning control
            rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));
        }

        private void Update()
        {
            //float h = Input.GetAxis("Horizontal");
            //float v = Input.GetAxis("Vertical");

            Vector3 v2 = hForce * cam.transform.forward; //Vertical axis to which I want to move with respect to the camera
            Vector3 h2 = vForce * cam.transform.right; //Horizontal axis to which I want to move with respect to the camera
            moveDir = (v2 + h2).normalized; //Global position to which I want to move in magnitude 1

            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f))
            {
                if (hit.transform.tag == "Slide")
                {
                    slide = true;
                }
                else
                {
                    slide = false;
                }
            }

            if (speed > 15.0f)
            {
                StartCoroutine(ResetSpeed());
            }

        }

        float CalculateJumpVerticalSpeed()
        {
            // From the jump height and gravity we deduce the upwards speed 
            // for the character to reach at the apex.
            return Mathf.Sqrt(2 * jumpHeight * gravity);
        }

        public void HitPlayer(Vector3 velocityF, float time)
        {
            rb.velocity = velocityF;

            pushForce = velocityF.magnitude;
            pushDir = Vector3.Normalize(velocityF);
            StartCoroutine(Decrease(velocityF.magnitude, time));
        }

        private IEnumerator Decrease(float value, float duration)
        {
            if (isStuned)
                wasStuned = true;
            isStuned = true;
            canMove = false;

            float delta = 0;
            delta = value / duration;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                yield return null;
                if (!slide) //Reduce the force if the ground isnt slide
                {
                    pushForce = pushForce - Time.deltaTime * delta;
                    pushForce = pushForce < 0 ? 0 : pushForce;
                    //Debug.Log(pushForce);
                }
                rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0)); //Add gravity
            }

            if (wasStuned)
            {
                wasStuned = false;
            }
            else
            {
                isStuned = false;
                canMove = true;
            }
        }

        public void SetSpeed(float x)
        {
            speed = x;
        }

        IEnumerator ResetSpeed()
        {
            WaitForSeconds wait = new WaitForSeconds(3.0f);
            yield return wait;
            speed = 15.0f;
        }

        public void stopPlayer()
        {
            speed = 0;
        }

        void PickedUpFunction()
        {
            pickedUpAlready = true;
            input.Disable();
            float yPos = ground.transform.position.y + 1;
            transform.DOMoveY(50, 2).OnComplete(() => {
                transform.DOMoveY(yPos, 0.2f).SetEase(Ease.Flash).OnComplete(() =>
                {
                    LoseHealth.Invoke(3);
                    transform.DOMoveY(30, .4f).SetEase(Ease.Flash).OnComplete(() =>
                    {
                        transform.DOMoveY(yPos, 0.1f).SetEase(Ease.Flash).OnComplete(() =>
                        {
                            LoseHealth.Invoke(3);
                            transform.DOMoveY(30, .4f).SetEase(Ease.Flash).OnComplete(() =>
                            {
                                transform.DOMoveY(yPos, 0.1f).SetEase(Ease.Flash).OnComplete(() =>
                                {
                                    LoseHealth.Invoke(3);
                                    pickedUpAlready = false;
                                    input.Enable();
                                });
                            });
                        });
                    });
                });
            });
            
        }

        void OnHealthLoss(int dmgAmount)
        {
            currentHealth -= dmgAmount;
            Debug.Log(currentHealth);
        }


    }
}