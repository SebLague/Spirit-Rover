using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rover : MonoBehaviour {

	const float marsGrav = 3.7f;
    public float acceleration = 5;
    public float turnSpeed = 10;
    public float brake = 100;
    public float maxWheelAngle = 45;
    public float dstToAngle = 2;
    float speed;
    float wheelAngle;

	public Transform camLook;
	public Transform camFollowPos;
	public Transform bubble;
	public Transform head;

    public Transform[] wheels;
    Wheel[] wheelRefs;

    public static event System.Action<Command> OnCommandRunForFirstTime;
	public static event System.Action OnCommandsFinished;
	public static event System.Action OnStuck;
	public static event System.Action OnTopple;
	public static event System.Action OnBegin;

	[HideInInspector]
	public bool isToppled;
	[HideInInspector]
	public bool isStuck;
	float stuckTestTime;

    Command[] commands;
    Rigidbody rb;

    float startTime;
    float nextCommandTime;
    bool running;
    int commandIndex;

    Command currentCommand;
	Vector3 posOld;

	public static Rover instance;

	void Awake() {
		instance = this;
	}

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheelRefs = new Wheel[wheels.Length];
        for (int i = 0; i < wheels.Length; i++)
        {
            wheelRefs[i] = wheels[i].GetComponent<Wheel>();
        }

		Physics.gravity = Vector3.down * marsGrav;
    }

    private void FixedUpdate()
    {
       // speed = rb.velocity.magnitude;
        if (running)
        {
			
            if (Time.fixedTime >= nextCommandTime)
            {
                if (commandIndex < commands.Length)
                {

                    currentCommand = commands[commandIndex];

                    if (OnCommandRunForFirstTime != null)
                    {
                        OnCommandRunForFirstTime(currentCommand);
                    }

					nextCommandTime = Time.fixedTime + currentCommand.duration;
                    commandIndex++;
                }
                else
                {
                    FinishedRunningCommands();
                    running = false;
                }
            }
            RunCurrentCommand();
        }

		if (!isToppled && transform.up.y < 0) {
			isToppled = true;
			if (OnTopple != null) {
				OnTopple ();
			}
		}

        float targetAngle = transform.eulerAngles.y + wheelAngle;

		float planeDst = speed * Time.fixedDeltaTime;
        float bodyDeltaAngle = Mathf.Min(planeDst * dstToAngle, Mathf.Abs(wheelAngle)) * Mathf.Sign(wheelAngle);

        wheelAngle -= bodyDeltaAngle;
 
        Vector3 wheelForwardSum = Vector3.zero;
        bool isGrounded = false;

        for (int i = 0; i < wheels.Length; i++)
        {
			wheels[i].localEulerAngles = Vector3.forward * wheelAngle;
			wheelForwardSum += -wheels[i].up;
			wheelRefs [i].SetSpeed (speed);
            if (wheelRefs[i].grounded)
            {
                isGrounded = true;
            }
		}

		Vector3 wheelForward = wheelForwardSum / wheels.Length;
		posOld = rb.position;
        if (isGrounded)
        {
            rb.MovePosition(rb.position + wheelForward * Time.fixedDeltaTime * speed);
            rb.MoveRotation(Quaternion.Euler(transform.eulerAngles + transform.up * bodyDeltaAngle));
        }

		float expectedMoveDst = speed * Time.fixedDeltaTime;
		float actualMoveDst = (new Vector2(rb.position.x,rb.position.z)-new Vector2(posOld.x,posOld.z)).magnitude;
		if (actualMoveDst < expectedMoveDst * .1f) {
			stuckTestTime += Time.fixedDeltaTime;
			if (stuckTestTime > 2 && !isStuck && !isToppled) {
				isStuck = true;
				if (OnStuck != null) {
					OnStuck ();
				}
			}
		} else {
			stuckTestTime = 0;
			isStuck = false;
		}
    }

	void OnTriggerEnter(Collider c) {
		if (c.tag == "Finish") {
			print ("FINIS");
		}
	}

    void FinishedRunningCommands()
    {
		if (OnCommandsFinished != null) {
			OnCommandsFinished ();
		}
    }

	void RunCurrentCommand()
	{
        switch (currentCommand.commandType)
        {
            case Command.CommandType.Accelerate:
                speed += acceleration * Time.fixedDeltaTime;
                break;
            case Command.CommandType.Brake:
                speed -= brake * Time.fixedDeltaTime;
                if (speed < 0)
                {
                    speed = 0;
                }
				break;
            case Command.CommandType.Left:
                wheelAngle -= turnSpeed * Time.fixedDeltaTime;
                wheelAngle = Mathf.Clamp(wheelAngle, -maxWheelAngle, maxWheelAngle);
				break;
            case Command.CommandType.Right:
				wheelAngle += turnSpeed * Time.fixedDeltaTime;
                wheelAngle = Mathf.Clamp(wheelAngle, -maxWheelAngle, maxWheelAngle);
				break;

        }
	}

    public void SetCommands(Command[] commands)
    {
        this.commands = commands;

        startTime = Time.fixedTime;
        commandIndex = 0;
        running = true;
		if (OnBegin != null) {
			OnBegin ();
		}
    }

  


}
