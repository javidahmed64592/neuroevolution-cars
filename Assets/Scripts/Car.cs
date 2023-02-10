using UnityEngine;

public class Car : MonoBehaviour
{
    // Car Attributes
    private float speedPercent = 0f;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float turningSpeed;
    [SerializeField] private float timeMaxSpeed;
    [SerializeField] private float timeSlowDown;
    [SerializeField] private float posSmoothStep = 0.05f;
    private Transform car;

    // Car Material
    private Material mat;

    private MeshRenderer skin;
    [SerializeField] private Texture[] carColours;

    // Layer Mask for Obstacles
    private LayerMask obstaclesLayer;

    // Neural Network
    [HideInInspector] public NeuralNetwork nn;

    private int inputNodes;
    private int extraInputNodes = 0;
    private float[] input;
    private float[] output;
    [SerializeField] private float sightDistance = 5f;
    [SerializeField] private float stoppingFloat = 1f;

    // Distance and angle to nearest target
    private GameObject[] targets;

    private Transform nearestTarget;

    private float distanceToTarget;
    [SerializeField] private float targetDistanceThreshold;
    private float alignmentAngle;

    // Genetic Algorithm
    [HideInInspector] public bool isAlive = true;

    [SerializeField] private float startHealth = 500f;
    private float currentHealth;
    [SerializeField] private float healthRegen = 50f;
    [SerializeField] private float healthDrain = 10f;

    [SerializeField] public float baseFitness = 0.5f;

    public bool crashed = false;
    [SerializeField] private float crashedPenalty = 0.1f;

    private void Awake()
    {
        car = GetComponent<Transform>();

        // Select random colour for car
        mat = GetComponent<Renderer>().material;
        skin = GetComponent<MeshRenderer>();
        mat.SetTexture("_MainTex", carColours[Random.Range(0, carColours.Length)]);

        // Configuring neural network
        nn = GetComponent<NeuralNetwork>();
        inputNodes = nn.layerNodes[0];
        input = new float[inputNodes];
        targets = GameObject.FindGameObjectsWithTag("Target");

        obstaclesLayer = LayerMask.GetMask("Obstacle");

        currentHealth = startHealth;
    }

    private void FixedUpdate()
    {
        if (isAlive)
        {
            checkSurroundings();
            //findTarget();
            Move();

            currentHealth = Mathf.Max(0f, currentHealth + (healthRegen * speedPercent - healthDrain) * Time.deltaTime);
            if (currentHealth == 0f)
            {
                SetAlive(false);
            }
        }
    }

    private void checkSurroundings()
    {
        for (int i = 0; i < inputNodes - extraInputNodes; i++)
        {
            //Color col;
            float angle = ((i * 2 * Mathf.PI) / (inputNodes - extraInputNodes)) + (car.eulerAngles.y * Mathf.PI / 180);
            Vector3 dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

            RaycastHit hit;
            if (Physics.Raycast(car.position + new Vector3(0f, 1f, 0f), dir, out hit, sightDistance, obstaclesLayer) && hit.transform.CompareTag("DEATH POW"))
            {
                input[i] = 1 - (hit.distance / sightDistance);
                //col = Color.red;
            }
            else
            {
                input[i] = 0;
                //col = Color.green;
            }

            //Debug.DrawRay(transform.position + new Vector3(0, 1, 0), dir * sightDistance, col);
        }
    }

    public Transform NearestTarget()
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = car.position;
        foreach (GameObject target in targets)
        {
            Vector3 diff = target.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = target;
                distance = curDistance;
            }
        }
        return closest.transform;
    }

    private void findTarget()
    {
        nearestTarget = NearestTarget();

        distanceToTarget = Vector3.Distance(car.position, nearestTarget.position);
        alignmentAngle = Mathf.Abs(Mathf.Cos(Vector3.Angle(car.forward, nearestTarget.forward) * Mathf.PI / 180));

        input[inputNodes - 3] = distanceToTarget < targetDistanceThreshold ?
           1 - (distanceToTarget / targetDistanceThreshold) : 0f;
        input[inputNodes - 2] = Mathf.Cos(Vector3.Angle(car.forward, nearestTarget.position) * Mathf.PI / 180);
        input[inputNodes - 1] = alignmentAngle;
    }

    private void Move()
    {
        output = nn.feedforward(input);

        if (nn.sigmoid(output[2]) > stoppingFloat) SetAlive(false);

        float newSpeedPercent = output[0];
        float turning = output[1];

        speedPercent = Mathf.Sign(newSpeedPercent) * Mathf.Min(1f, Mathf.Abs(Mathf.SmoothStep(speedPercent, newSpeedPercent, Time.deltaTime / timeMaxSpeed)));

        Vector3 newPos = Vector3.Lerp(car.position, car.position + (car.forward * maxSpeed * speedPercent), posSmoothStep);
        Quaternion newRot = car.rotation * Quaternion.AngleAxis(turning * turningSpeed * Time.deltaTime, Vector3.up);
        car.SetPositionAndRotation(newPos, newRot);

        speedPercent = Mathf.SmoothStep(speedPercent, 0f, Time.deltaTime / timeSlowDown);
    }

    private void SetAlive(bool alive)
    {
        isAlive = alive;
        speedPercent = 0f;
    }

    public void ResetPosition(Vector3 startPosition)
    {
        // Set attributes
        SetAlive(true);
        crashed = false;
        skin.enabled = true;
        currentHealth = isAlive ? startHealth : 0f;

        // Move to spawn position
        car.position = startPosition;
        car.rotation = Quaternion.identity;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Collision with death object? You die!
        if (other.gameObject.CompareTag("DEATH POW"))
        {
            skin.enabled = false;
            crashed = true;
            SetAlive(false);
        }
    }

    public float fitness()
    {
        float score = targetDistanceThreshold * (baseFitness + ((1 - baseFitness) * alignmentAngle)) / distanceToTarget;
        return Mathf.Pow(score * (crashed ? crashedPenalty : 1f), 2);
    }
}