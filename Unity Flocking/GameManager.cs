
using UnityEngine;
using System.Collections;

//add using System.Collections.Generic; to use the generic list format
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------
    public GameObject dude;
    public GameObject target;

    public GameObject dudePrefab;
    public GameObject targetPrefab;
    public GameObject obstaclePrefab;

	private GameObject[] obstacles;

    public GameObject[] Obstacles {
        get { return obstacles; }
    }

	private Vector3 centroid;

	public Vector3 Centroid{
		get{ return centroid;}
	}

	//average flock direction
	private Vector3 flockDirection;

	public Vector3 FlockDirection{
		get{ return flockDirection;}
	}

	//list of flockers
	private List<GameObject>flock;

	public List<GameObject>Flock{
		get{ return flock;}
	}

	//how many flockers are there
	public int numberFlockers;

    //-----------------------------------------------------------------------
    // Start and Update
    //-----------------------------------------------------------------------
	void Start () {
		numberFlockers = 5;
		//create the list of flockers
		flock = new List<GameObject> ();

        //Create the target (noodle)
        Vector3 pos = new Vector3(0, 4.0f, 0);
        target = (GameObject)Instantiate(targetPrefab, pos, Quaternion.identity);
        //target = Instantiate(targetPrefab, pos, Quaternion.identity) as GameObject;

        //Create the GooglyEye Guy at (10, 1, 10)
		for (int i = 0; i < numberFlockers; i++) {
			pos = new Vector3 (Random.Range (-10, 10), 1, 10);
			dude = (GameObject)Instantiate (dudePrefab, pos, Quaternion.identity);
			dude.GetComponent<Seeker>().seekerTarget = target;

			flock.Add (dude);
		}
        //set the camera's target 
        Camera.main.GetComponent<SmoothFollow>().target = dude.transform;

        //set the Googly Eye Guys Target

		//Create multiple flockers here, set their targets and add them to the list of flockers!

        obstacles = new GameObject[20];

        //create obstacles and place them in the obstacles array
        for(int i =0; i < 20; i++)
        {
            pos = new Vector3(Random.Range(-30,30),1.1f,Random.Range(-30,30));
            Quaternion rot = Quaternion.Euler(0,Random.Range(0,180),0);
            GameObject temp = Instantiate(obstaclePrefab, pos, rot) as GameObject;
            obstacles[i] = temp;

        }
	}
	

	void Update () {
        //compare the distance between the guy and noodle
        //move the noodle if it's close
		foreach (GameObject dud in flock) {
			float dist = Vector3.Distance(target.transform.position, dud.transform.position);
			
			//randomize the target's position
			if(dist < 3f)
			{
				do
				{
					target.transform.position = new Vector3(Random.Range(-30, 30), 4f, Random.Range(-30, 30));
				}
				while (NearAnObstacle());
				
			}
		}
		calcCentroid();
		CalcFlockDirection();
	}

    //-----------------------------------------------------------------------
    // Flocking Methods
    //-----------------------------------------------------------------------
    bool NearAnObstacle()
    {
        //iterate through all obstacles and compare the distance between each obstacle and the noodle 
        //if the noodle is within a 4 unit distance of the noodle, return true
        for(int i = 0; i < obstacles.Length; i++)
        {
            if(Vector3.Distance(target.transform.position, obstacles[i].transform.position) < 5.0f)
            {
                return true;
            }
        }
        //otherwise, the noodle is not near an obstacle
        return false;
    }

		void calcCentroid(){
		centroid = Vector3.zero;
		//average of the positions
		foreach(GameObject f in flock){
			centroid += f.transform.position;
		}
		centroid /= flock.Count;
		gameObject.transform.position = centroid;
	}

		void CalcFlockDirection(){
		flockDirection = Vector3.zero;
		foreach(GameObject f in flock){
			centroid += f.transform.forward;
		}
		flockDirection /= flock.Count;
		gameObject.transform.forward = flockDirection;
	}


}
