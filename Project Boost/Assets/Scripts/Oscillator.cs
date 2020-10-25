using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent] // It's allows you to drag one same script(You cant attach more than one same script on spesific object);
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector;
    [SerializeField] float period = 2f;

    [SerializeField] [Range(-1, 1)]
    float movementFactor;

    Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        //startingPos = new Vector3(transform.position.x - 21.69f, transform.position.y, transform.position.z);
        startingPos = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        if(period <= Mathf.Epsilon) { return; } // protect against period is zero -----------// float numbers 
        float cycles = Time.time / period; // grows continually from 0

        const float tau = Mathf.PI * 2f; // about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau); //goes from -1 to 1

        //movementFactor = rawSinWave / 2f + 0.5f; // this range is set (-1, 1) in normal, so to calculate this in (0,1) area, we are divided by 2f which is new value = -0,5,0.5 then add 0.5 than the final value is in that range = (0 to 1)
        movementFactor = rawSinWave;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
