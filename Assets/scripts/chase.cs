using UnityEngine;
using System.Collections;

public class chase : MonoBehaviour
{

    public Transform player;
    public Transform head;
    Animator anim;

    string state = "patrol";
    public GameObject[] waypoints;
    int currentWP = 0;
    public float rotSpeed = 0.2f;
    public float speed = 1.5f;
    float accuracyWP = 2.0f;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = player.position - this.transform.position; //get the vector between player and enemy
       // direction.y = 0; //to prevent vertical rotation-> tipping over
        float angle = Vector3.Angle(direction, head.up);

        if (state == "patrol" && waypoints.Length > 0)
        {
            anim.SetBool("isIdle", false);
            anim.SetBool("isWalk", true);
            if (Vector3.Distance(waypoints[currentWP].transform.position, transform.position) < accuracyWP)
            {

               // currentWP = Random.Range(0, waypoints.Length); // this is to randomize the current position waypoint
                currentWP++;
                if(currentWP >= waypoints.Length)
                {
                	currentWP = 0;
                }	
            }

            //rotate towards waypoint
            direction = waypoints[currentWP].transform.position - transform.position;
            this.transform.rotation = Quaternion.Slerp(transform.rotation,
                                 Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);
            this.transform.Translate(0, 0, Time.deltaTime * speed);
        }

        if (Vector3.Distance(player.position, this.transform.position) < 10 && (angle < 60 || state == "pursuing"))
        {

            state = "pursuing";
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                        Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);

            if (direction.magnitude > 3)
            {
                this.transform.Translate(0, -0.005f, Time.deltaTime * speed);
                anim.SetBool("isWalk", true);
                anim.SetBool("isAttack", false);
            }
            else
            {
                anim.SetBool("isWalk", false);
                anim.SetBool("isAttack", true);
                
            }

        }
        else
        {
            anim.SetBool("isWalk", true);
            anim.SetBool("isAttack", false);
            state = "patrol";
        }

    }
}
