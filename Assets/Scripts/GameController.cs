using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Population variables
    [SerializeField] private PopulationController population;

    // Laser
    [SerializeField] private Transform laser;

    [SerializeField] private float laserSpeed = 0.1f;
    private Vector3 laserStartPos;

    // Genetic Algorithm
    private int count = 0;

    private int generation = 1;

    // UI
    [SerializeField] private Text generationText;

    [SerializeField] private Text numAliveText;

    private void Awake()
    {
        generationText.text = "Generation: " + generation;

        laserStartPos = laser.position;
    }

    private void Start()
    {
        StartCoroutine("Running", 1f);
    }

    private void Update()
    {
        numAliveText.text = "Active cars: " + population.numAlive()
            + "\nReached end: " + population.numAtEnd();
    }

    private IEnumerator Running()
    {
        while (true)
        {
            if (population.numAlive() == 0 && count != 0)
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

    private void FixedUpdate()
    {
        laser.position = Vector3.Lerp(laser.position, laser.position + new Vector3(0, 0, laserSpeed * Mathf.Pow(count, 0.5f)), 0.15f);
    }

    private void ResetAll()
    {
        population.Evaluate();
        count = 0;
        generation++;
        generationText.text = "Generation: " + generation;
        laser.position = laserStartPos;
    }
}