using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameObject player; //player prefab
    public GameObject goal; //goal prefab

    public GameObject[] population; //population list
    private int _populationSize = 100; //size of population list
    private float _fitnessSum = 0; //hold fitness sum value
    public int minStep = 300; //store minimum step achieved 
    private bool _paused = false; //is game paused
    public int generation = 0; //current generation
    private int lifespan = 10; // amount of steps for first generation
    private bool firstGoal = false; //test value, true after reach the goal
    
    //Initialize point
    void Start()
    {
        population = new GameObject[_populationSize];
        for (int i = 0; i < population.Length; i++)
        {
            population[i] = Instantiate(player, player.GetComponent<Player>().spawn, Quaternion.identity);
            population[i].name = i.ToString();
        }
    }

    /// <summary>
    ///Stop interface for x seconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator Delay()
    {
        _paused = true;
        yield return new WaitForSeconds(4);
        _paused = false;
    }

    //Run for each update
    void Update()
    {
        if (_paused)
        {
            return;
        }
        
        //check for finish players
        for (int i = 0; i < population.Length; i++)
        {
            //check for finish players
            if (population[i].GetComponent<Player>().finish)
            {
                //temp message for testing after reach the goal
                if (!firstGoal)
                {
                    firstGoal = true;
                    print("Fist goal reached at " + generation.ToString());
                    GetComponent<AudioSource>().Play();
                }
                
                //die all players in population
                foreach (var player in population)
                {
                    player.GetComponent<Player>().Die();
                }
            }
            
            //kill players with higher steps
            if (population[i].GetComponent<Player>().brain.Step > minStep)
            {
                population[i].GetComponent<Player>().Die();
            }
        }

        //GA
        if (IsAllDead())
        {
            //GeneticAlgorithm
            CalculateFitnessSum();
            NaturalSelection();
            Mutation();
            IncreaseLifeSpan();
            generation++;
        }

        //StartCoroutine(Delay());


        
    }

    private void IncreaseLifeSpan()
    {
        if (population[0].GetComponent<Player>().brain.Step < lifespan)
        {
            lifespan = lifespan + 1;
            foreach (var player in population)
            {
                player.GetComponent<Player>().lifespan = lifespan;
            }
        }
    }

    /// <summary>
    /// Method responsible for execute mutations for each object in population
    /// </summary>
    private void Mutation()
    {
        for (int i = 1; i < population.Length; i++)
        {
            population[i].GetComponent<Player>().brain.Mutate();
        }
    }

    /// <summary>
    /// Method creates a new population based on the old one and replaces it
    /// </summary>
    private void NaturalSelection()
    {
        //create list for new population
        GameObject[] newPopulation = new GameObject[population.Length];

        //find the best one and move it to the new list without any changes
        var best = GetBestPlayer();
        var best2 = GetBestPlayer();
        newPopulation[0] = Instantiate(player, player.GetComponent<Player>().spawn, Quaternion.identity);
        newPopulation[0].GetComponent<Player>().CreateBaby(best,best2);
        newPopulation[0].GetComponent<Player>().MarkBest();

        //crossover
        for (int i = 1; i < newPopulation.Length; i++)
        {
            GameObject parent = SelectParent();
            GameObject parent2 = SelectParent();
            newPopulation[i] = Instantiate(player, player.GetComponent<Player>().spawn, Quaternion.identity);
            newPopulation[i].GetComponent<Player>().CreateBaby(parent,parent2);
        }

        //Destroy old population and replace with a new one
        DestroyPopulation();
        population = newPopulation;

    }

    /// <summary>
    /// Destroy all population objects prefabs in game
    /// </summary>
    private void DestroyPopulation()
    {
        for (int i = 0; i < population.Length; i++)
        {
            population[i].GetComponent<Player>().DestroyIt();
        }
    }

    /// <summary>
    ///Selecting parent based on random number and fitness sum
    /// </summary>
    /// <returns></returns>
    private GameObject SelectParent()
    {
        float random = Random.Range(0.0f, _fitnessSum);
        //float random = Random.Range((int)(_fitnessSum*0.8), _fitnessSum);
        float sum = 0;

        for (int i = 0; i < population.Length; i++)
        {
            sum += population[i].GetComponent<Player>().fitness;
            if (sum >= random)
            {
                return population[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Check if the entire population is dead
    /// </summary>
    /// <returns>Population status</returns>
    private bool IsAllDead()
    {
        for (int i = 0; i < population.Length; i++)
        {
            if (!population[i].GetComponent<Player>().dead)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Sum all fitness from each population object
    /// </summary>
    private void CalculateFitnessSum()
    {
        _fitnessSum = 0;
        for (int i = 0; i < population.Length; i++)
        {
            var playerReference = population[i].GetComponent<Player>();
            playerReference.CalculateFitness(goal);
            _fitnessSum += playerReference.fitness;
        }
    }

    /// <summary>
    /// Return the best object in population
    /// </summary>
    /// <returns></returns>
    private GameObject GetBestPlayer()
    {
        float max = 0;
        int indexOfBest = 0;
        
        //population = population.OrderByDescending(x => x.GetComponent<Player>().fitness).ToArray();
        //print("max"+population[0].GetComponent<Player>().fitness.ToString("0.0000000000000000000000") );
        //print("max"+population[599].GetComponent<Player>().fitness.ToString("0.0000000000000000000000") );
        
        population = population.OrderByDescending(x => x.GetComponent<Player>().fitness).ToArray();
        for (int i = 0; i < population.Length; i++)
        {
            if (population[i].GetComponent<Player>().fitness > max)
            {
                max = population[i].GetComponent<Player>().fitness;
                indexOfBest = i;
            }
        }


        if (population[indexOfBest].GetComponent<Player>().finish)
        {
            minStep = population[indexOfBest].GetComponent<Player>().brain.Step;
        }
        
        //print("fitness best " + population[indexOfBest].GetComponent<Player>().fitness.ToString("0.0000000000000000000000") + " " + Vector3.Distance(population[indexOfBest].transform.position, goal.transform.position));
        return population[indexOfBest];
    }


}