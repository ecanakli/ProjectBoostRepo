using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    //----------------States----------------
    enum State { Alive, Dying, Transcending}
    State currentstate = State.Alive;

    [Header("Main Settings")]
    [SerializeField] float rocketThrust = 100f;
    [SerializeField] float rotatePower = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [Header("Sound Settings")]
    [SerializeField] AudioClip _thrustSound;
    [SerializeField] AudioClip _deadSound;
    [SerializeField] AudioClip _clearLevelSound;

    [Header("Particle Settings")]
    [SerializeField] ParticleSystem _thrustParticlesLeft;
    [SerializeField] ParticleSystem _thrustParticlesRight;
    [SerializeField] ParticleSystem _successParticles;
    [SerializeField] ParticleSystem _deadParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool collisionDisabled;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //If We Alive
        if (currentstate == State.Alive)
        {
            Thrust();
            Rotate();
        }

        if (Debug.isDebugBuild) //Only Works If Debug Mod On
        {
            //To Facilitate Level Design
            RespondtoDebugKeys();
        }
    }

    private void RespondtoDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        //This Is Toggle Function That Enables And Disables Collision
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisabled = !collisionDisabled;  
        }
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * rocketThrust * Time.deltaTime);

            //Play ThrustParticles
            _thrustParticlesLeft.Play();
            _thrustParticlesRight.Play();
        }
        else
        {
            //Stop ThrustParticles
            _thrustParticlesLeft.Stop();
            _thrustParticlesRight.Stop();
        }

        ThrustSound();
    }

    private void Rotate()
    {
        float zMove = Input.GetAxisRaw("Horizontal");

        rigidBody.angularVelocity = Vector3.zero; //Remove Rotation

        transform.Rotate(0f, 0f , -zMove);
        /*
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotatePower * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotatePower * Time.deltaTime);
        }*/
    }

    void ThrustSound()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            //To Avoid Sound Repetition, Only Play If Is Not Playing
            if (!audioSource.isPlaying) //So It Doesn't Layer
            {
                audioSource.PlayOneShot(_thrustSound);
            }
        }
        else
        {
            audioSource.Stop();
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        //While Dead Ignore Collisions
        if(currentstate != State.Alive || collisionDisabled)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:

                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        currentstate = State.Dying;
        audioSource.Stop();
        _deadParticles.Play();
        audioSource.PlayOneShot(_deadSound);
        StartCoroutine(LoadFirstLevel());
    }

    private void StartSuccessSequence()
    {
        currentstate = State.Transcending;
        audioSource.Stop();
        _successParticles.Play();
        audioSource.PlayOneShot(_clearLevelSound);
        StartCoroutine(LoadNextScene());
    }

    //Loading First Level After Delay
    IEnumerator LoadFirstLevel()
    {
        yield return new WaitForSeconds(levelLoadDelay);
        SceneManager.LoadScene(0);
    }

    //Loading New Level After Delay
    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(levelLoadDelay);

        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;

        //SceneCountInBuildSettings Means Last Scene
        //If Nextscene Is Last Scene Than Load First Scene
        if (nextScene == SceneManager.sceneCountInBuildSettings)  
        {
            nextScene = 0;
        }

        SceneManager.LoadScene(nextScene);
    }
}
