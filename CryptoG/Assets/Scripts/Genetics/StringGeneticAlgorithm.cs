using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringGeneticAlgorithm : MonoBehaviour
{

    [Header("Options")]
    [SerializeField] private string needed = "to be or not to be"; 
    [SerializeField] private string available = "qwertyuiopasdfghjklzxcvbnm ";
    [SerializeField] private int numAgents = 500;
    [SerializeField] private int iterations = 10000;

    [Space(10)]
    [SerializeField] private float chromosomeRate = 0.1f;
    [SerializeField] private float geneRate = 0.05f;

    private List<string> agents, genePool;
    private int length;
    private string bestAgent;

    private void Start()
    {
        length = needed.Length;
        CreateAllAgents();
        StartCoroutine(SolveGenetic());
    }

    private IEnumerator SolveGenetic()
    {
        for (int i = 0; i < iterations; i++)
        {
            AssessFitness();
            CrossReproduction();
            MutateAgents();

            yield return null;
            if (bestAgent == needed) {break;}
        }
    }

    private void MutateAgents()
    {
        List<string> mutates = new List<string>();

        foreach (var ag in agents)
        {
            bool mutateChromosome = (UnityEngine.Random.Range(0f, 1f) <= chromosomeRate);
            if (mutateChromosome)
            {
                string res = "";
                for (int i = 0; i < length; i++)
                {
                    bool mutateGene = (UnityEngine.Random.Range(0f, 1f) <= geneRate);
                    if (mutateGene)
                    {
                        res += available[UnityEngine.Random.Range(0, available.Length)];
                    }
                    else {res += ag[i];}
                }
                mutates.Add(res);
            }
            else {mutates.Add(ag);}
        }

        agents = new List<string>(mutates);
    }

    private void CrossReproduction()
    {
        agents = new List<string>();

        for (int i = 0; i < numAgents; i++)
        {
            string ag1 = genePool[UnityEngine.Random.Range(0, genePool.Count)];
            string ag2 = genePool[UnityEngine.Random.Range(0, genePool.Count)];

            int marker = UnityEngine.Random.Range(0, length+1); //0 -> length

            string child = "";
            for (int j = 0; j < marker; j++) {child += ag1[j];}
            for (int j = marker; j < length; j++) {child += ag2[j];}

            agents.Add(child);
        }
    }

    private void AssessFitness()
    {
        int maxFit = -1; string maxFitString = "";
        genePool = new List<string>();

        foreach(var ag in agents)
        {
            int fit = 0;
            
            for (int i = 0; i < length; i++)
            {
                if (ag[i] == needed[i]) {fit++;}
            }
            for (int i = 0; i < fit; i++)
            {
                genePool.Add(ag); //the fitter, the more inserted into next gene pool
            }

            if (fit > maxFit)
            {
                maxFit = fit; maxFitString = ag;
            }
        }
        Debug.Log("Best agent : " + maxFitString);
        bestAgent = maxFitString;
    }

    private void CreateAllAgents()
    {
        agents = new List<string>();
        for (int i = 0; i < numAgents; i++)
        {
            string ag = "";
            for (int j = 0; j < length; j++)
            {
                ag += available[UnityEngine.Random.Range(0, available.Length)];
            }
            agents.Add(ag);
        }
    }
}
