using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace TSP
{
    class CityState
    {
        public double[,] CostMatrix { get; private set; }
        public double LowerBound { get; private set; }
        public ArrayList Path { get; private set; }

        public double[,] hi;

        public CityState(double[,] costMatrix, double parentBound, ArrayList parentPath, City WhoAmI)
        {
            CostMatrix = costMatrix;
            LowerBound = parentBound;
            Path = (ArrayList)parentPath.Clone();
            Path.Add(WhoAmI);
            //add WhoAmI to cost matrix
        }

        private void ReduceCostMatrix()
        {
            //Check for zeros in each row and column
            bool[] rowHasZero = new bool[CostMatrix.GetLength(0)];
            bool[] colHasZero = new bool[CostMatrix.GetLength(0)];

            //TODO account for already visited edges using the PATH.

            for (int i = 0; i < rowHasZero.Length; i++)
            {
                for (int j = 0; j < colHasZero.Length; j++)
                {
                    if (CostMatrix[i, j] == 0)
                    {
                        rowHasZero[i] = true;
                        colHasZero[j] = true;
                    }
                }
            }

            //Reduce the matrix by row
            for (int i = 0; i < rowHasZero.Length; i++)
            {
                if (rowHasZero[i] == false)
                {
                    MinimizeRow(i, ref colHasZero);
                }
            }

            //Then by column
            for (int j = 0; j < colHasZero.Length; j++)
            {
                if (colHasZero[j] == false)
                {
                    MinimizeCol(j);
                }
            }
        }

        private void MinimizeRow(int row, ref bool[] colHasZero)
        {
            double min = Double.PositiveInfinity;

            //Find the Minimum
            for (int j = 0; j < CostMatrix.GetLength(0); j++ )
            {
                if (CostMatrix[row, j] < min)
                    min = CostMatrix[row, j];
            }

            if (Double.IsPositiveInfinity(min))
            //This entire row is infinity == infeasible
            {
                return;
            }

            //Minimize the row
            for (int j = 0; j < CostMatrix.GetLength(0); j++)
            {
                if ((CostMatrix[row, j] -= min) == 0)
                //If a value ends up as zero, mark that column as having a zero
                {
                    colHasZero[j] = true;
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
            //Entire col is infinity, return
            {
                return;
            }

            //Minimize Col
            for (int i = 0; i < CostMatrix.GetLength(0); i++)
            {
                CostMatrix[i, col] -= min;
            }

            LowerBound += min;
        }

    }
}
