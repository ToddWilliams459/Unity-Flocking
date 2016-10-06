using UnityEngine;
using System.Collections;

public class Seeker : Vehicle {

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------
    public GameObject seekerTarget;

    //Seeker's steering force (will be added to acceleration)
    private Vector3 force;

    //WEIGHTING!!!!
    public float seekWeight = 100.0f;

    public float avoidWeight = 100.0f;
	public float cohesionWeight = 50.0f;
	public float alignWeight = 50.0f;
	public float sepWeight = 20.0f;



    //-----------------------------------------------------------------------
    // Start - No Update
    //-----------------------------------------------------------------------
	// Call Inherited Start and then do our own
	override public void Start () {
        //call parent's start
		base.Start();

        //initialize
        force = Vector3.zero;
	}

    //-----------------------------------------------------------------------
    // Class Methods
    //-----------------------------------------------------------------------

    protected override void CalcSteeringForces() {
        //reset value to (0, 0, 0)
        force = Vector3.zero;

        //got a seeking force
        force += Seek(seekerTarget.transform.position) * seekWeight;

        // avoid
		for (int i = 0; i < gm.Obstacles.Length; i++) {
			force += AvoidObstacle(gm.Obstacles[i],safeDistance) * avoidWeight;
		}
		//steering forces stuff goes below
		force += Separation(gm.Flock) * sepWeight;
		force += Cohesion(gm.Centroid) * cohesionWeight;
		force += Alignment(gm.FlockDirection) * alignWeight;

        //limited the seeker's steering force
        force = Vector3.ClampMagnitude(force, maxForce);

        //applied the steering force to this Vehicle's acceleration (ApplyForce)
        ApplyForce(force);

        
    }
	
    protected Vector3 AvoidObstacle(GameObject ob, float safe)
    {
        //reset desired velocity
        desired = Vector3.zero;

        //get radius from obstacles script
        float obRad = ob.GetComponent<ObstacleScript>().Radius;

        //get vector from vehicle to obstacle
        Vector3 vecToCenter = ob.transform.position - transform.position;

        //zero-out y component (only necessary when working on X-Z plane)
        vecToCenter.y = 0;

        //if object is out of my safe zone, ignore it
        if(vecToCenter.magnitude > safe)
        {
            return Vector3.zero;
        }
        //if object is behind me, ignre it
        if(Vector3.Dot(vecToCenter, transform.forward) < 0)
        {
            return Vector3.zero;
        }

        //if object is not in my forward path, ignore it
        if(Mathf.Abs(Vector3.Dot(vecToCenter, transform.right)) > obRad + radius)
        {
            return Vector3.zero;
        }
        //if we get this far, we will collide with an obstacle
        //object on left,steerr right
        if (Vector3.Dot (vecToCenter, transform.right) < 0) {
			desired = transform.right * maxSpeed;
			//debug line to see if the dude is avoiding to the right
			Debug.DrawLine (transform.position, ob.transform.position, Color.red);
		} else {
			desired = transform.right * -maxSpeed;
			//debug line to see if the dude is avoiding to the left
			Debug.DrawLine (transform.position,ob.transform.position,Color.green);
		}
        return desired;
    }
}
