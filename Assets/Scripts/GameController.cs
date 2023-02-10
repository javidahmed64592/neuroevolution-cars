using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Population variables
    [SerializeField] private PopulationController population;

    // Genetic Algorithm
    private int count = 0;

    [SerializeField] private int maxCount = 100;

    private int generation = 1;

    // UI
    [SerializeField] private Text generationText;

    [SerializeField] private Text countText;
    [SerializeField] private Text numAliveText;

    private void Awake()
    {
        generationText.text = "Generation: " + generation;
    }

    private void Start()
    {
        StartCoroutine("Running", 1f);
    }

    private void Update()
    {
        numAliveText.text = $"No Alive: {population.numAlive()}" +
            $"\nNo Crashed: {population.numCrashed()}";
        countText.text = "Count: " + count;
    }

    private IEnumerator Running()
    {
        while (true)
        {
            if (population.numAlive() == 0 && count != 0 || count == maxCount)
            {
                ResetAll();
            }
            else
            {
                count++;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private void ResetAll()
    {
        population.Evaluate();
        count = 0;
        generation++;
        generationText.text = "Generation: " + generation;
    }
}