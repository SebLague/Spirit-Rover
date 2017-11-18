using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rover : MonoBehaviour {


    public float acceleration = 5;
    public float turnSpeed = 10;
    public float brake = 100;
    public float maxWheelAngle = 45;
    public float dstToAngle = 2;
    float speed;
    float wheelAngle;

	public Transform camLook;
	public Transform camFollowPos;

    public Transform[] wheels;
    Wheel[] wheelRefs;

    public event System.Action<Command> OnCommandRunForFirstTime;

    Command[] commands;
    Rigidbody rb;

    float startTime;
    float nextCommandTime;
    bool running;
    int commandIndex;

    Command currentCommand;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheelRefs = new Wheel[wheels.Length];
        for (int i = 0; i < wheels.Length; i++)
        {
            wheelRefs[i] = wheels[i].GetComponent<Wheel>();
        }
    }

    private void FixedUpdate()
    {
       // speed = rb.velocity.magnitude;
        if (running)
        {
			
            if (Time.time >= nextCommandTime)
            {
                if (commandIndex < commands.Length)
                {

                    currentCommand = commands[commandIndex];

                    if (OnCommandRunForFirstTime != null)
                    {
                        OnCommandRunForFirstTime(currentCommand);
                    }

                    nextCommandTime = Time.time + currentCommand.duration;
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

        float targetAngle = transform.eulerAngles.y + wheelAngle;

        float planeDst = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
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

        if (isGrounded)
        {
            rb.MovePosition(rb.position + wheelForward * Time.fixedDeltaTime * speed);
            rb.MoveRotation(Quaternion.Euler(transform.eulerAngles + transform.up * bodyDeltaAngle));
        }
    }

    void FinishedRunningCommands()
    {

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

        startTime = Time.time;
        commandIndex = 0;
        running = true;
    }

  


}
