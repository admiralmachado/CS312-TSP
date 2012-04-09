using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C5;

namespace TSP
{
    class TSPAgenda
    {
        private Stack<CityState> agenda;
        private TreeSet<CityState> candidates;
        public int PruneCount { get; private set; }
        public int MaxSize { get; private set; }

        public TSPAgenda()
        {
            agenda = new Stack<CityState>();
            candidates = new TreeSet<CityState>();
            PruneCount = 0;
            MaxSize = 0;
        }

        public CityState GetNext()
        {
            return agenda.Pop();
        }

        public void AddCandidate(CityState state)
        {
            candidates.Add(state);
        }

        public void CommitCandidates()
        {
            foreach(CityState state in candidates)
            {
                agenda.Push(state);
            }

            candidates.Clear();
            if (agenda.Count > MaxSize)
                MaxSize = agenda.Count;
        }

        public bool IsEmpty()
        {
            return (agenda.Count == 0);
        }

        public void Prune(double bssf)
        {
            List<CityState> temp = new List<CityState>(agenda.Count);
            foreach (CityState state in agenda)
            {
                if (state.LowerBound < bssf)
                {
                    temp.Add(state);
                }
                else
                {
                    ++PruneCount;
                }
            }
            
            agenda = new Stack<CityState>(temp.Count);
            for (int i = temp.Count - 1; i >= 0; i--)
            {
                agenda.Push(temp[i]);
            }
        }
    }
}
