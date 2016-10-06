using UnityEngine;
using System.Collections;

//use the Generic system here to make use of a Flocker list later on
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

abstract public class Vehicle : MonoBehaviour {

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------

    //Access to GameManager Script
    protected GameManager gm;

    //movement
    protected Vector3 acceleration;
    protected Vector3 velocity;
    protected Vector3 desired;

    public Vector3 Velocity {
        get { return velocity; }
    }

    //public for changing in Inspector
    //define movement behaviors
    public float maxSpeed = 6.0f;
    public float maxForce = 12.0f;
    public float mass = 1.0f;
    public float radius = 1.0f;

	//steering behavior variables
	public float safeDistance = 10.0f;
	public float steerSpeed = 5.0f;

    //access to Character Controller component
    CharacterController charControl;
    

    abstract protected void CalcSteeringForces();


    //-----------------------------------------------------------------------
    // Start and Update
    //-----------------------------------------------------------------------
	virtual public void Start(){
        gm = GameObject.Find("GameManagerGO").GetComponent<GameManager>();
        //acceleration = new Vector3 (0, 0, 0);     
        acceleration = Vector3.zero;
        velocity = transform.forward;
        charControl = GetComponent<CharacterController>();
	}

	
	// Update is called once per frame
	protected void Update () {
        //calculate all necessary steering forces
        CalcSteeringForces();

        //add accel to vel
        velocity += acceleration * Time.deltaTime;
        velocity.y = 0;

        //limit vel to max speed
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.forward = velocity.normalized;

        //move the character based on velocity
        charControl.Move(velocity * Time.deltaTime);

        //reset acceleration to 0
        acceleration = Vector3.zero;
	}


    //-----------------------------------------------------------------------
    // Class Methods
    //-----------------------------------------------------------------------

    protected void ApplyForce(Vector3 steeringForce) {
        acceleration += steeringForce / mass;
    }

    protected Vector3 Seek(Vector3 targetPos) {
        desired = targetPos - transform.position;
        desired = desired.normalized * maxSpeed;
        desired -= velocity;
        desired.y = 0;
        return desired;
    }

	public Vector3 Separation(List<GameObject> flock){
		//List<Vector3>temp;
		Vector3 tempFl;
		Vector3 tempDes;
		Vector3 total = Vector3.zero;
		foreach(GameObject f in flock){
			tempFl = this.transform.position - f.transform.position;
			if(tempFl.magnitude < safeDistance){
				//temp.Add(tempFl);
				if(Vector3.Dot(tempFl,transform.right) < 0){
					tempDes = transform.right * steerSpeed;
				}
				else{
					tempDes = transform.right * -steerSpeed;
				}
				total += tempDes.normalized * tempFl.magnitude;
			}

		}
		if(total.magnitude == 0.0f){
			return total;
		}
		else{
			return (total.normalized * maxSpeed) - velocity;
		}
	}

	public Vector3 Alignment(Vector3 dof){
		Vector3 desVel = dof.normalized *maxSpeed;
		desVel -= velocity;
		desVel.y = 0;
		return desVel;

	}

	public Vector3 Cohesion(Vector3 cof){
		return Seek (cof);
	}

}
