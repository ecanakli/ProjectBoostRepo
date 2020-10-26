using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    enum State { Alive, Dying, Transcending}
    State currentstate = State.Alive;

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip thrustSound;
    [SerializeField] AudioClip deadSound;
    [SerializeField] AudioClip clearLevelSound;

    [SerializeField] ParticleSystem thrustParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deadParticles;

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
        if (currentstate == State.Alive)
        {
            Thrust();
            Rotate();
        }

        if (Debug.isDebugBuild) // Only work if Debug mod on
        {
            RespondtoDebugKeys();
        }
    }

    private void RespondtoDebugKeys() //to do
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisabled = !collisionDisabled;  // It will work toggle script like on off when press C
        }
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
            thrustParticles.Play();
        }
        else
        {
            thrustParticles.Stop();
        }

        ThrustSound();
    }

    void ThrustSound()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            if (!audioSource.isPlaying) // so it doesn't layer
            {
                audioSource.PlayOneShot(thrustSound);
            }
        }
        else
        {
            audioSource.Stop();
        }
    }
    private void Rotate()
    {
        rigidBody.freezeRotation = true; //Take manuel controll of rotation

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rcsThrust * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rcsThrust * Time.deltaTime);
        }

        rigidBody.freezeRotation = false; //Resume physics controll of rotation
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(currentstate != State.Alive || collisionDisabled) //ignore collisions when dead
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
        deadParticles.Play();
        audioSource.PlayOneShot(deadSound);
        Invoke("LoadFirstLevel", levelLoadDelay); // do method after levelloaddelay second
    }

    private void StartSuccessSequence()
    {
        currentstate = State.Transcending;
        audioSource.Stop();
        successParticles.Play();
        audioSource.PlayOneShot(clearLevelSound);
        Invoke("LoadNextScene", levelLoadDelay); // do method after levelloaddelay second
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
}
