using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class controller_personnage : MonoBehaviour
{
    float vitesse_x;
    public float vitesse_x_max;
    float vitesse_y;
    public float hauteur_Saut;
    bool fin;
    bool gagner = false;
    bool dash_on = false;
    bool attak_animation1 = false;
    bool attak_animation2 = false;
    public TextMeshProUGUI point;
    public TextMeshProUGUI boss;
    int boss_hp = 20;
    int point_total = 1;
    public NewBehaviourScript newBehaviourScript;
    public Camera cam_perso;
    public Camera cam_boss;
    public AudioClip son_slash1;
    public AudioClip son_slash2;
    public AudioClip son_gagner;
    public AudioClip son_mort;
    public AudioClip son_boss_fight;
    bool scene2 = false;
    bool enter_boss_room = false;
        
    void Start()
    {
        cam_perso.enabled = true;
        cam_boss.enabled = false;
    }

    void Update()
    {
        if (!fin || !gagner)
        {
            point.text = "hp :" + point_total;

            if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
            {
                vitesse_x = -vitesse_x_max;
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
            {
                vitesse_x = vitesse_x_max;
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                vitesse_x = GetComponent<Rigidbody2D>().velocity.x;
            }

            if (Mathf.Abs(vitesse_x) > 0.1f)
            {
                GetComponent<Animator>().SetBool("cour", true);
            }
            else
            {
                GetComponent<Animator>().SetBool("cour", false);
            }

            if ((Input.GetKeyDown("w") && Physics2D.OverlapCircle(transform.position, 0.5f)) ||
                (Input.GetKeyDown(KeyCode.UpArrow) && Physics2D.OverlapCircle(transform.position, 0.5f)))
            {
                vitesse_y = hauteur_Saut;
                GetComponent<Animator>().SetBool("saut", true);
            }
            else
            {
                GetComponent<Animator>().SetBool("saut", false);
                vitesse_y = GetComponent<Rigidbody2D>().velocity.y;
            }

            if (Input.GetKeyDown("k") && !dash_on)
            {
                dash_on = true;
                Invoke("stop_dash", 0.5f);
                GetComponent<Animator>().SetTrigger("dash");
                GetComponent<Animator>().SetBool("saut", false);
            }

            if (dash_on && Mathf.Abs(vitesse_x) <= vitesse_x_max)
            {
                vitesse_x *= 4;
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                if (!attak_animation1)
                {
                    GetComponent<Animator>().SetTrigger("attak");
                    attak_animation1 = true;
                    Invoke("arreter_attak", 1f);
                    GetComponent<AudioSource>().PlayOneShot(son_slash1,1f);
                }
                else
                {
                    GetComponent<Animator>().SetTrigger("attak_2");
                    attak_animation2 = true;
                    Invoke("arreter_attak2", 1f);
                    GetComponent<AudioSource>().PlayOneShot(son_slash2, 1f);
                }
            }

            GetComponent<Rigidbody2D>().velocity = new Vector2(vitesse_x, vitesse_y);
        }

        if (point_total >= 20 && !scene2)
        {
            point_total = 20;
            Invoke("niveau_2", 0f);
        }

        if (point_total < 0)
        {
            point_total = 0;
        }
        if (enter_boss_room == true) {
         boss.text = "boss hp :" + boss_hp;
        }
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "enemi")
        {
            if (attak_animation1 || attak_animation2)
            {
                point_total += 1;
                collision.gameObject.GetComponent<Animator>().SetTrigger("mort_enemi");
                collision.gameObject.GetComponent<Collider2D>().enabled = false;
                Destroy(collision.gameObject, 1f);
            }
            else if (point_total > 0)
            {
                GetComponent<Animator>().SetTrigger("prend_degat");
                point_total -= 1;
            }
            else
            {
                fin = true;
                GetComponent<AudioSource>().PlayOneShot(son_mort, 1f);
                GetComponent<Animator>().SetTrigger("mort");
                GetComponent<Collider2D>().enabled = false;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Rigidbody2D>().angularVelocity = 0f;
                GetComponent<Rigidbody2D>().gravityScale = 0f;
                Invoke("recommencer", 3f);
            }
        }

        if (collision.gameObject.name == "mort")
        {
            fin = true; 
            GetComponent<AudioSource>().PlayOneShot(son_mort, 1f);
            GetComponent<Animator>().SetTrigger("mort");
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().angularVelocity = 0f;
            GetComponent<Rigidbody2D>().gravityScale = 0f;
            Invoke("recommencer", 3f);
            collision.gameObject.GetComponent<Collider2D>().enabled = false;
           
        }

        if (collision.gameObject.name == "boss_room")
        {
            cam_perso.enabled = false;
            cam_boss.enabled = true;
            collision.gameObject.GetComponent<Collider2D>().enabled = false;
            GetComponent<AudioSource>().PlayOneShot(son_boss_fight);
            enter_boss_room = true;
        }

        if(collision.gameObject.name == "boss_1")
        {
            if (attak_animation1 || attak_animation2 && (boss_hp > 0))
            {
             if (boss_hp <= 0)
                {
                    gagner = true;
                    Invoke("nouvelle_partie", 0f);
                    collision.gameObject.GetComponent<Animator>().SetTrigger("mort_enemi");
                    collision.gameObject.GetComponent<Collider2D>().enabled = false;
                    Destroy(collision.gameObject, 1f);
                    collision.gameObject.GetComponent<Animator>().SetTrigger("b_mort");

                } else { 
                
                  point_total += 1;
                  boss_hp -= 1; 
                }         
            }

             if (!attak_animation1 || !attak_animation2 )
            {
                if( point_total > 0)
                {
                    point_total -= 2;
                }
                     else if (point_total == 0) 
                {
                    
                    fin = true;
                    GetComponent<AudioSource>().PlayOneShot(son_mort, 1f);
                    GetComponent<Animator>().SetTrigger("mort");
                    GetComponent<Collider2D>().enabled = false;
                    GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    GetComponent<Rigidbody2D>().angularVelocity = 0f;
                    GetComponent<Rigidbody2D>().gravityScale = 0f;
                    Invoke("recommencer", 3f);
                    collision.gameObject.GetComponent<Collider2D>().enabled = false;
                }
           } 
           

        }
        if (Physics2D.OverlapCircle(transform.position, 0.5f))
        {
            GetComponent<Animator>().SetBool("saut", false);

            if (collision.gameObject.tag == "platorm")
            {
                transform.parent = collision.gameObject.transform;
            }
        }

    }

    void OnCollisionExit2D(Collision2D collision)
    {
        transform.parent = null;
    }

    void recommencer()
    {
        SceneManager.LoadScene(1);
    }

    void stop_dash()
    {
        dash_on = false;
    }

    void arreter_attak()
    {
        attak_animation1 = false;
    }

    void arreter_attak2()
    {
        attak_animation2 = false;
    }

    void niveau_2()
    {
        scene2 = true;

        SceneManager.LoadScene(2);
    }
    void nouvelle_partie ()
    {
        SceneManager.LoadScene(0);
    }
}
