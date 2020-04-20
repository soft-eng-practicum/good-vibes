using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtBubbleBobber : MonoBehaviour
{
    [SerializeField] bool rise;
    [SerializeField] int change;
    [SerializeField] bool moving;

    // Start is called before the first frame update
    void Start()
    {
        int random = Random.Range(0, 2);
        if (random == 0)
            rise = true;
        else
            rise = false;
        change = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!moving)
            StartCoroutine(Bob());
    }

    IEnumerator Bob()
    {
        moving = true;
        yield return new WaitForSeconds(Random.Range(0f, 0.3f));
        if (rise)
        {
            transform.localPosition += Vector3.up;
            change++;
        }
        else
        {
            transform.localPosition -= Vector3.up;
            change--;
        }
        if (change >= 10)
            rise = false;
        else if (change <= -10)
            rise = true;
        moving = false;
    }

    private void OnEnable()
    {
        moving = false;
        StartCoroutine(Bob());
    }
}
