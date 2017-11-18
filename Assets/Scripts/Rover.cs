using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rover : MonoBehaviour {

    public float acceleration = 5;
    public float turnSpeed = 10;
    public float brake = 100;
    public float maxWheelAngle = 45;
    public float dstToAngle = 2;
    public float speed;
    float wheelAngle;

    public Transform[] wheels;

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
    }

    private void FixedUpdate()
    {
        speed = rb.velocity.magnitude;
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
        Vector3 wheelForward = new Vector3(Mathf.Sin(targetAngle * Mathf.Deg2Rad), 0, Mathf.Cos(targetAngle * Mathf.Deg2Rad));

        Debug.DrawRay(transform.position, wheelForward * 10, Color.red);

        float planeDst = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
        float bodyDeltaAngle = Mathf.Min(planeDst * dstToAngle, Mathf.Abs(wheelAngle)) * Mathf.Sign(wheelAngle);
        rb.MoveRotation(Quaternion.Euler(transform.eulerAngles + transform.up * bodyDeltaAngle));
        wheelAngle -= bodyDeltaAngle;
        Vector2 planeDir = new Vector2(transform.forward.x, transform.forward.z).normalized;
        rb.velocity = new Vector3(wheelForward.x * planeDst, rb.velocity.y, wheelForward.z * planeDst);
        //rb.MoveRotation(Quaternion.Euler(Quaternion.rot
        foreach (Transform t in wheels)
        {
            t.localEulerAngles = Vector3.forward * wheelAngle;
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
                rb.AddForce(transform.forward * acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);
                break;
            case Command.CommandType.Brake:
                Vector3 planeVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                if (planeVelocity.magnitude > 0.0001f)
                {
                    Vector3 brakeDir = -planeVelocity.normalized;
                    float moveAngle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                    rb.AddForce(brakeDir * brake * Time.fixedDeltaTime, ForceMode.Acceleration);
                    float moveAngleAfterBraking = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                    // dont allow deceleration to begin accelerating rover in opp direction
                    if (Mathf.DeltaAngle(moveAngle, moveAngleAfterBraking) > 90)
                    {
                        rb.velocity = new Vector3(0, rb.velocity.y, 0);
                    }
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
