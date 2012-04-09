using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace TSP
{
    class CityState : IComparable<CityState>
    {
        public double[,] CostMatrix { get; private set; }
        public double LowerBound { get; private set; }
        public ArrayList Path { get; private set; }
        public List<int> Children { get; private set; }        

        public CityState(double[,] costMatrix, double parentBound, ArrayList parentPath, City WhoAmI)
        {
            CostMatrix = (double[,])costMatrix.Clone();
            LowerBound = parentBound;
            Path = (ArrayList)parentPath.Clone();
            Children = new List<int>();

            if (Path.Count > 0)
                AddNewEdge(WhoAmI);
            else
                Path.Add(WhoAmI);
            
            ReduceCostMatrix();
        }

        private void AddNewEdge(City NewDest)
        {
            City NewSrc = (City)Path[Path.Count - 1];

            //Add the new City to the Path
            Path.Add(NewDest);

            //Add the cost of taking the edge
            LowerBound += CostMatrix[NewSrc.CityId, NewDest.CityId];

            //The src of edge can't go anywhere else
            for (int j = 0; j < CostMatrix.GetLength(0); j++)
            {
                CostMatrix[NewSrc.CityId, j] = Double.PositiveInfinity;
            }

            //No other edge can arrive at dest
            for (int i = 0; i < CostMatrix.GetLength(0); i++)
            {
                CostMatrix[i, NewDest.CityId] = Double.PositiveInfinity;
            }

            //If this isn't the last city, make returning to the start infeasible.
            if (Path.Count < CostMatrix.GetLength(0))
            {
                CostMatrix[NewDest.CityId, ((City)Path[0]).CityId] = Double.PositiveInfinity;
            }

        }

        private void ReduceCostMatrix()
        {
            //Check if a row or column must be reduced
            bool[] rowNotReducible = new bool[CostMatrix.GetLength(0)];
            bool[] colNotReducible = new bool[CostMatrix.GetLength(0)];

            //account for already visited edges using the path.
            AvoidReducingInfinity(ref rowNotReducible, ref colNotReducible);

            for (int i = 0; i < rowNotReducible.Length; i++)
            {
                //If we've already deemed it irreducible, skip it
                if (rowNotReducible[i] == true)
                    continue;

                //If this row corresponds to the most recent city in the path
                if (i == ((City)Path[Path.Count - 1]).CityId)
                //track its potential destinations (children states)
                {
                    for (int j = 0; j < colNotReducible.Length; j++)
                    {
                        if (!Double.IsPositiveInfinity(CostMatrix[i, j]))
                            Children.Add(j);

                        if (CostMatrix[i, j] == 0)
                        {
                            rowNotReducible[i] = true;
                            colNotReducible[j] = true;
                        }
                    }
                }
                else
                //otherwise just run this for loop instead
                {
                    for (int j = 0; j < colNotReducible.Length; j++)
                    {
                        //If we've already deemed it irreducible, skip it
                        if (colNotReducible[j] == true)
                            continue;

                        if (CostMatrix[i, j] == 0)
                        {
                            rowNotReducible[i] = true;
                            colNotReducible[j] = true;
                        }
                    }
                }

                //I know, I know... duplicate code is bad
                //But this way the row check occurs n times and not n^2 times.
            }

            //Reduce the matrix by row
            for (int i = 0; i < rowNotReducible.Length; i++)
            {
                if (rowNotReducible[i] == false)
                //so it IS reducible
                {
                    MinimizeRow(i, ref colNotReducible);
                }
            }

            //Then by column
            for (int j = 0; j < colNotReducible.Length; j++)
            {
                if (colNotReducible[j] == false)
                {
                    MinimizeCol(j);
                }
            }
        }

        private void AvoidReducingInfinity(ref bool[] rowNotReducible, ref bool[] colNotReducible)
        {
            //for each edge in the path
            //States with only 1 city (no edges) get skipped because
            // 1(i) !< 1(Path.Count)
            for (int i = 1; i < Path.Count; i++)
            {
                //the src's row is infinity
                rowNotReducible[((City)Path[i - 1]).CityId] = true;

                //the dest's col is infinity
                colNotReducible[((City)Path[i]).CityId] = true;
            }
        }

        private void MinimizeRow(int row, ref bool[] colNotReducible)
        {
            double min = Double.PositiveInfinity;

            //Find the Minimum
            for (int j = 0; j < CostMatrix.GetLength(0); j++ )
            {
                if (CostMatrix[row, j] < min)
                    min = CostMatrix[row, j];
            }

            if (Double.IsPositiveInfinity(min))
            {
                //Sanity check, this should never happen
                Console.WriteLine("[MinimizeRow] Hey buddy you did something wrong.");
            }

            //Minimize the row
            for (int j = 0; j < CostMatrix.GetLength(0); j++)
            {
                if ((CostMatrix[row, j] -= min) == 0)
                //If a value ends up as zero, mark that column as having a zero
                //so it won't be reduced later
                {
                    colNotReducible[j] = true;
                }
            }

            LowerBound += min;
        }

        private void MinimizeCol(int col)
        {
            double min = Double.PositiveInfinity;

            for (int i = 0; i < CostMatrix.GetLength(0); i++)
            {
                if (CostMatrix[i, col] < min)
                    min = CostMatrix[i, col];
            }

            if (Double.IsPositiveInfinity(min))
            {
                //Sanity check, this should never happen
                Console.WriteLine("[MinimizeCol] Hey buddy you did something wrong.");
            }

            //Minimize Col
            for (int i = 0; i < CostMatrix.GetLength(0); i++)
            {
                CostMatrix[i, col] -= min;
            }

            LowerBound += min;
        }


        public int CompareTo(CityState other)
        {
            double delta = other.LowerBound - this.LowerBound;
            if (delta > 0)
                return 1;
            else if (delta < 0)
                return -1;
            else
                return 0;
        }
    }
}
