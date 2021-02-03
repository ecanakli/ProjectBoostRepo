using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//It's Allows You To Drag One Same Script(You Can't Attach More Than One Same Script On Spesific Object);
[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movVector; //To Determine The Range Of Object Position
    [SerializeField] float period = 2f; //Period Time

    //To Show Object Movement Value In Editor
    [SerializeField] [Range(-1, 1)]
    float movScale; //Moving Pointer Between Left = (-1) Not Move = (0) Right Move = (1)

    Vector3 startingPos;

    private void Start()
    {
        SettingStartPos();
    }

    private void SettingStartPos()
    {
        startingPos = transform.position;
    }

    private void Update()
    {
        if(period <= Mathf.Epsilon) { return; } //We Can't Equal Two Float Number, To Protect  Against Zero Error, Instead Of Using Zero, We Are Using Epsilon, Which Is The Closest Value To It
        float cycles = Time.time / period; //Grows Continually From 0

        const float tau = Mathf.PI * 2f; //Cons Tau Value About 6.28f
        float rawSinWave = Mathf.Sin(cycles * tau); //Goes From (-1 to 1)

        movScale = rawSinWave;
        //To Be Able To Adjust Value Later We Setted movScale To rawSinWave
        /*-------------- To Move Object Between (0,1) Area --------------------
        movScale = rawSinWave / 2f + 0.5f;
        //This Range Is Set (-1 , 1) In Normal, If We Divided rawSinWave Value By 2f, Which Is New Value = - 0.5f, 0.5f
        //Then Adding 0.5f, The Final Value Will Be In That Range (0 to 1)*/

        Vector3 offset = movVector * movScale; //Creating Offset
        transform.position = startingPos + offset; //Movement
    }
}
