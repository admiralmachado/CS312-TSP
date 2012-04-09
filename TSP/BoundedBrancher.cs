using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;

namespace TSP
{
    class BoundedBrancher
    {
        public double bssf_cost { get; private set; }
        public ArrayList bssf { get; private set; }
        private int pruneCnt;
        private City[] cities;
        private TSPAgenda agenda;

        public int PruneCount
        {
            get
            {
                return pruneCnt + agenda.PruneCount;
            }
        }

        public int AgendaMaxSize
        {
            get
            {
                return agenda.MaxSize;
            }
        }

        public BoundedBrancher(City[] _cities)
        {
            cities = _cities;
            agenda = new TSPAgenda();
        }

        public void BranchAndBound()
        {
            double[,] CostMatrix = new double[cities.Length, cities.Length];
            InitCostMatrix(ref CostMatrix);
            InitBSSF(ref CostMatrix);
            InitAgenda(ref CostMatrix);

            RunMainLoop();
        }

        private void InitCostMatrix(ref double[,] CostMatrix)
        {
            for (int i = 0; i < cities.Length; i++)
            {
                //Set diagonal
                CostMatrix[i, i] = Double.PositiveInfinity;
                for (int j = i + 1; j < cities.Length; j++)
                {
                    //Distance is symmetrical
                    CostMatrix[j, i] = CostMatrix[i, j] = cities[i].costToGetTo(cities[j]);
                }
            }
        }

        private void InitBSSF(ref double[,] CostMatrix)
        {
            //Straight iteration through cities
            bssf = new ArrayList(cities);
            bssf_cost = 0;
            for (int i = 1; i < cities.Length; i++)
            {
                //Add cost of simple tour
                bssf_cost += CostMatrix[i - 1, i];
            }

            //Plus last edge returning to start
            bssf_cost += CostMatrix[cities.Length - 1, 0];
        }

        private void InitAgenda(ref double[,] CostMatrix)
        {
            CityState firstState = new CityState(CostMatrix, 0, new ArrayList(), cities[0]);
            agenda.AddCandidate(firstState);
            agenda.CommitCandidates();
        }

        private void RunMainLoop()
        {
            CityState cur;
            while (!agenda.IsEmpty())
            {
                cur = agenda.GetNext();

                if (cur.Children.Count > 1)
                //Generate Child States
                {
                    CreateChildren(cur);
                }
                else
                //We've reached the fringe
                {
                    CheckAgainstBSSF(cur);
                }
            }
        }

        private void CheckAgainstBSSF(CityState state)
        {
            //Console.WriteLine("Solution: " + state.LowerBound + " BSSF: " + bssf_cost);
            //Check if the solution beats the bssf
            if (state.LowerBound < bssf_cost)
            {
                bssf_cost = state.LowerBound;
                bssf = state.Path;
                //Complete the path by adding the only destination left
                bssf.Add(cities[state.Children[0]]);
                

                //Comment out this line to disable pruning
                agenda.Prune(bssf_cost);
            }
        }

        private void CreateChildren(CityState parent)
        {
            CityState child;
            for (int i = 0; i < parent.Children.Count; i++)
            {
                //make child
                child = new CityState(parent.CostMatrix,
                                        parent.LowerBound,
                                        parent.Path,
                                        cities[parent.Children[i]]);
                //The last argument is get the next feasible destination from
                //a list the current state has produced

                //Comment this if expression except for the AddCandidate statement to disable pruning on generation
                
                //check if lower bound < bssf
                if (child.LowerBound < bssf_cost)
                    //add to agenda
                    agenda.AddCandidate(child);
                else
                {
                    //Prune!
                    ++pruneCnt;
                }
            }

            //commit candidates to agenda
            agenda.CommitCandidates();
        }
    }
}
