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
    [HideInInspector] private float[] input;
    [SerializeField] private float sightDistance = 5f;

    // Genetic Algorithm
    [HideInInspector] public bool isAlive = true;

    [HideInInspector] public bool atEnd = false;

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

        obstaclesLayer = LayerMask.GetMask("Obstacle");
    }

    private void FixedUpdate()
    {
        if (isAlive && !atEnd)
        {
            checkSurroundings();
            Move();
        }
    }

    private void checkSurroundings()
    {
        for (int i = 0; i < inputNodes; i++)
        {
            //Color col;
            float angle = ((i * 2 * Mathf.PI) / inputNodes) + (car.eulerAngles.y * Mathf.PI / 180);
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

    private void Move()
    {
        float[] output = nn.feedforward(input);
        float newSpeedPercent = output[0];
        float turning = output[1];

        //speedPercent = Mathf.Sign(newSpeedPercent) * Mathf.Min(1f, Mathf.Abs(Mathf.SmoothStep(speedPercent, newSpeedPercent, Time.deltaTime / timeMaxSpeed)));
        speedPercent = Mathf.SmoothStep(speedPercent, newSpeedPercent, Time.deltaTime / timeMaxSpeed);
        speedPercent = speedPercent < 0 ? Mathf.Max(-0.2f, speedPercent) : Mathf.Min(1f, speedPercent);

        Vector3 newPos = Vector3.Lerp(car.position, car.position + (car.forward * maxSpeed * speedPercent), posSmoothStep);
        Quaternion newRot = car.rotation * Quaternion.AngleAxis(turning * turningSpeed * Time.deltaTime, Vector3.up);
        car.SetPositionAndRotation(newPos, newRot);

        speedPercent = Mathf.SmoothStep(speedPercent, 0f, Time.deltaTime / timeSlowDown);
    }

    private void SetAlive(bool alive)
    {
        isAlive = alive;
        skin.enabled = alive;
        speedPercent = 0f;
        atEnd = !alive;
    }

    public void ResetPosition(Vector3 startPosition)
    {
        // Set attributes
        SetAlive(true);

        // Move to spawn position
        car.position = startPosition;
        car.rotation = Quaternion.identity;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Collision with death object? You die!
        if (other.gameObject.CompareTag("DEATH POW")) SetAlive(false);
        if (other.gameObject.CompareTag("Stop"))
        {
            atEnd = true;
            isAlive = false;
            speedPercent = 0f;
        }
    }

    public float fitness()
    {
        return car.position.z > 0 ? Mathf.Pow(car.position.z, 2) : 0f;
    }
}