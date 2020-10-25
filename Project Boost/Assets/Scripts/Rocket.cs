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
    [SerializeField] AudioClip thrustSound;
    [SerializeField] AudioClip deadSound;
    [SerializeField] AudioClip clearLevelSound;

    Rigidbody rigidBody;
    AudioSource audioSource;

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
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
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
            transform.Rotate(Vector3.forward * rcsThrust);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rcsThrust);
        }

        rigidBody.freezeRotation = false; //Resume physics controll of rotation
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(currentstate != State.Alive) //ignore collisions when dead
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
        audioSource.PlayOneShot(deadSound);
        Invoke("LoadFirstLevel", 1f); //do method after 1 sec
    }

    private void StartSuccessSequence()
    {
        currentstate = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(clearLevelSound);
        Invoke("LoadNextScene", 1f); //do method after 1 sec
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
