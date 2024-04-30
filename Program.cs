using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CodingTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Read heightmap from the text file
            int[][] heightmap = ReadHeightmapFromFile("heightmap.txt");

            // Calculate the sum of risk levels of all low points
            int sumOfRiskLevels = CalculateSumOfRiskLevels(heightmap);

            // Output the result
            Console.WriteLine("Part-One \nSum of risk levels of all low points: " + sumOfRiskLevels);

            // Find and multiply the sizes of the three largest basins
            int result = FindAndMultiplyLargestBasins(heightmap);
            Console.WriteLine("\nPart-two \nResult: " + result);
            Console.ReadKey();
        }

        // Method to read heightmap from the text file
        public static int[][] ReadHeightmapFromFile(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            int[][] heightmap = new int[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                heightmap[i] = new int[line.Length];
                for (int j = 0; j < line.Length; j++)
                {
                    heightmap[i][j] = int.Parse(line[j].ToString());
                }
            }
            return heightmap;
        }

        #region Part-One
        // Method to calculate the sum of risk levels of all low points
        public static int CalculateSumOfRiskLevels(int[][] heightmap)
        {
            int sumOfRiskLevels = 0;
            for (int i = 0; i < heightmap.Length; i++)
            {
                for (int j = 0; j < heightmap[i].Length; j++)
                {
                    if (IsLowPoint(heightmap, i, j))
                    {
                        sumOfRiskLevels += heightmap[i][j] + 1;
                    }
                }
            }
            return sumOfRiskLevels;
        }

        // Method to check if a point is a low point
        static bool IsLowPoint(int[][] heightmap, int row, int col)
        {
            int height = heightmap[row][col];
            int[] adjacentHeights = {
            row > 0 ? heightmap[row - 1][col] : int.MaxValue,  // Up
            row < heightmap.Length - 1 ? heightmap[row + 1][col] : int.MaxValue,  // Down
            col > 0 ? heightmap[row][col - 1] : int.MaxValue,  // Left
            col < heightmap[row].Length - 1 ? heightmap[row][col + 1] : int.MaxValue  // Right
        };
            return height < Math.Min(adjacentHeights[0], Math.Min(adjacentHeights[1], Math.Min(adjacentHeights[2], adjacentHeights[3])));
        }
        #endregion

        #region Part-Two
        // Function to perform flood-fill algorithm to find the size of a basin
        public static int FindBasinSize(int[][] heightmap, (int, int) lowPoint)
        {
            var directions = new List<(int, int)> { (0, -1), (0, 1), (-1, 0), (1, 0) };
            var basinSize = 1; // Initialize basin size to include the low point itself
            var visited = new HashSet<(int, int)>(); // Set to keep track of visited locations
            var queue = new Queue<(int, int)>(); // Queue for BFS traversal

            visited.Add(lowPoint);
            queue.Enqueue(lowPoint);

            while (queue.Count > 0)
            {
                var (row, col) = queue.Dequeue();
                var height = heightmap[row][col];

                // Check neighboring locations
                foreach (var (dr, dc) in directions)
                {
                    var newRow = row + dr;
                    var newCol = col + dc;
                    if (newRow >= 0 && newRow < heightmap.Length && newCol >= 0 && newCol < heightmap[0].Length)
                    {
                        if (!visited.Contains((newRow, newCol)) && heightmap[newRow][newCol] != 9)
                        {
                            visited.Add((newRow, newCol));
                            queue.Enqueue((newRow, newCol));
                            basinSize++;
                        }
                    }
                }
            }
            return basinSize;
        }

        // Method to find the three largest basins and multiply their sizes together
        public static int FindAndMultiplyLargestBasins(int[][] heightmap)
        {
            var lowPoints = new List<(int, int)>();

            // Find all low points in the heightmap
            for (int row = 0; row < heightmap.Length; row++)
            {
                for (int col = 0; col < heightmap[0].Length; col++)
                {
                    if (IsLowPoint(heightmap, row, col))
                    {
                        lowPoints.Add((row, col));
                    }
                }
            }

            // Find the size of each basin
            var basinSizes = lowPoints.Select(lowPoint => FindBasinSize(heightmap, lowPoint)).ToList();

            // Find the three largest basins
            basinSizes.Sort();
            basinSizes.Reverse();
            var largestBasins = basinSizes.Take(3).ToList();

            // Multiply the sizes of the three largest basins together
            return largestBasins.Aggregate(1, (acc, val) => acc * val);
        }
        #endregion
    }
}
