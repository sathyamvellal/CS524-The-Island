using UnityEngine;
using System.Collections;

public class chase2 : MonoBehaviour
{

    public Transform player;
    Animator anim;
    public float speed = 3.5f;
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = player.position - this.transform.position;
        float angle = Vector3.Angle(direction, this.transform.forward);
        if (Vector3.Distance(player.position, this.transform.position) < 100 && angle < 300)
        {

            //direction.y = 0;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                        Quaternion.LookRotation(direction), 0.1f);

            anim.SetBool("isIdle", false);
            if (direction.magnitude > 2)
            {
                this.transform.Translate(0, -0.005f, Time.deltaTime * speed);
                anim.SetBool("isWalk", true);
                anim.SetBool("isAttack", false);
            }
            else
            {
                anim.SetBool("isAttack", true);
                anim.SetBool("isWalk", false);
            }

        }
        else
        {
            anim.SetBool("isIdle", true);
            anim.SetBool("isWalk", false);
            anim.SetBool("isAttack", false);
        }

    }
}
