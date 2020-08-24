using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
    public GameObject container;
    public GameObject[] foodsAvailable;
    public float timeLowerBound;
    public float timeHigherBound;

    // Start is called before the first frame update
    void Start()
    {
        Spawner();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Spawner()
    {
        int index = Random.Range(0, foodsAvailable.Length - 1);

        GameObject food = Instantiate(foodsAvailable[index]);
        food.transform.position = new Vector3(
            Random.Range(-100, 100),
            Random.Range(0, 10),
            Random.Range(-100, 100)
        );
        food.transform.parent = container.transform;

        Invoke("Spawner", Random.Range(timeLowerBound, timeHigherBound));
    }
}
