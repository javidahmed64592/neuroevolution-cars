using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulationController : MonoBehaviour
{
    // Camera variables
    [SerializeField] private Transform camTransform;

    [SerializeField] private Vector3 offset;
    private float smoothStep = 75f;

    // Population variables
    [SerializeField] private GameObject CarPrefab;

    [SerializeField] private int populationSize;
    [SerializeField] private float mutationRate;

    private List<Car> population = new List<Car>();

    private float maxFitness = 0;

    public void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        for (int _ = 0; _ < populationSize; _++)
        {
            Vector3 pos = new Vector3(0f, 0f, transform.position.z) + transform.position;
            GameObject Car = Instantiate(CarPrefab, pos, Quaternion.identity);
            Car.transform.parent = transform;
            population.Add(Car.GetComponent<Car>());
        }
    }

    public int bestAgent()
    {
        maxFitness = 0;
        int bestAgent = 0;

        for (int i = 0; i < population.Count; i++)
        {
            float fitness = population[i].fitness();
            bool isAlive = population[i].isAlive;
            bool atEnd = population[i].atEnd;

            if (fitness > maxFitness && isAlive)
            {
                maxFitness = fitness;
                bestAgent = i;
            }
        }

        return bestAgent;
    }

    public int numAlive()
    {
        int num = 0;
        foreach (Car Car in population)
        {
            num += System.Convert.ToInt32(Car.isAlive);
        }

        return num;
    }

    public void Evaluate()
    {
        for (int i = 0; i < populationSize; i++)
        {
            int indexA = parentIndex();
            int indexB = parentIndex();

            while (indexB == indexA) indexB = parentIndex();

            population[i].nn.Crossover(population[indexA].nn, population[indexB].nn, mutationRate);
            population[i].ResetPosition(new Vector3(0f, 0f, transform.position.z) + transform.position);
        }

        foreach (Car Car in population)
        {
            Car.nn.ApplyMatrices();
        }

        maxFitness = 0;
    }

    private int parentIndex()
    {
        int index = Random.Range(0, population.Count);
        while (Random.Range(0f, 1f) > population[index].fitness() / maxFitness)
        {
            index = Random.Range(0, population.Count);
        }

        return index;
    }

    private void LateUpdate()
    {
        // Target
        Transform target = population[bestAgent()].transform;

        // Update position and rotation
        Vector3 targetPosition = target.position + offset;
        camTransform.SetPositionAndRotation(Vector3.Lerp(camTransform.position, targetPosition, 1 / smoothStep),
            Quaternion.Slerp(camTransform.rotation, Quaternion.LookRotation(target.position - camTransform.position), 1 / smoothStep));
    }
}