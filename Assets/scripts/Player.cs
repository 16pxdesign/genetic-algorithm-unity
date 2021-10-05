using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// The class represents a single player and his behaviors
/// </summary>
public class Player : MonoBehaviour
{
    //variables
    private static float speed = 45f;
    public Vector3 spawn = new Vector3(45, 0.5f, 85);
    public bool dead = false;

    //GA
    public Brain brain;
    public bool finish = false;
    public float fitness = 0;

    public int lifespan = 0;

    //prefabs
    public Material bestMaterial;
    public Material dieMaterial;
    public Shader alwaysOnTop;


    //Initialize point
    void Start()
    {
        if (brain == null)
        {
            brain = new Brain();
        }
    }

    //Run for each frame
    void FixedUpdate()
    {
        if (OutBounds())
            Die();
        Move();
    }

    /// <summary>
    /// Checks whether the object has left the map
    /// </summary>
    /// <returns></returns>
    private bool OutBounds()
    {
        return transform.position.y < 0.5f;
    }

    /// <summary>
    /// Kills the player, stops him and changes color
    /// </summary>
    public void Die()
    {
        dead = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Renderer>().material = dieMaterial;
    }

    /// <summary>
    /// Responsible for moving the player around the map
    /// </summary>
    void Move()
    {
        if (!dead)
        {
            //check for object speed
            if (GetComponent<Rigidbody>().velocity.magnitude < speed)
            {
                //Force object with next vector or die
                if (brain.Vectors.Length > brain.Step)
                {
                    GetComponent<Rigidbody>().AddForce(brain.Vectors[brain.Step] * speed);
                    brain.Step++;
                }
                else
                {
                    Die();
                }
            }
        }
    }

    /// <summary>
    /// Calculates the result obtained by the player based rules
    /// </summary>
    /// <param name="goal">Reference to goal prefab</param>
    public void CalculateFitness(GameObject goal)
    {
        //check for type of fitness
        if (finish)
        {
            float distance = Vector3.Distance(spawn, goal.transform.position);
            fitness = 10000000.0f / (brain.Step * brain.Step);
            print("fitness2 " + fitness.ToString("0.0000000000000000000000"));
        }
        else
        {
            float distance = Vector3.Distance(transform.position, goal.transform.position);
            //fitness = 10.0f / (distance * distance * distance * distance);
            fitness = 10.0f / (distance * distance * distance * distance * distance * distance * distance * distance *
                               distance);
            print("Fitness " + fitness.ToString("0.0000000000000000000000") + " " + distance);
        }
    }

    /// <summary>
    /// Listener for a collision with another object
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "target":
                finish = true;
                Die();
                break;
            case "wall":
                Die();
                break;
        }
    }

    /// <summary>
    /// Destroy current object
    /// </summary>
    public void DestroyIt()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    ///Marks the player as the best
    /// </summary>
    public void MarkBest()
    {
        GetComponent<Renderer>().material = bestMaterial;
        GetComponent<Renderer>().material.shader = alwaysOnTop;
    }
    
    /// <summary>
    /// Returns a new object with the genome copied
    /// </summary>
    /// <param name="best"></param>
    public void CreateBaby(GameObject best, GameObject best2)
    {
        //var clone = best.GetComponent<Player>().brain.Clone();

        Brain clone = new Brain();
        for (int i = 0; i < best.GetComponent<Player>().brain.Vectors.Length; i++)
        {
            double coin = Random.Range(0, 1);
            clone.Vectors[i] = coin < 0.5
                ? best.GetComponent<Player>().brain.Vectors[i]
                : best2.GetComponent<Player>().brain.Vectors[i];
        }


        brain = clone;
    }


}