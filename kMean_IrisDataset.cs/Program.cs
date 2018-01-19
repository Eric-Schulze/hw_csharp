using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kMean_IrisDataset.cs
{
    class Program
    {
        private static int numberOfClusters = 3;
        private static int numberOfAttributes = 0;

        static void Main(string[] args)
        {
            List<iris_datapoint> lid = readData();
            List<centroid> lc = populateCentroids(lid);

            lc = kMeans(ref lid, lc, 1000000);

            foreach(iris_datapoint id in lid)
            {
                Console.WriteLine("ID: " + id.id_number + "\t\tPredicted Species: " + id.currentCentroid + "\tTrue Species: " + id.species);
            }

            Console.WriteLine("KMeans Accruracy: " + calculateAccuracy(ref lid));
            Console.ReadLine();
        }

        public static List<iris_datapoint> readData()
        {
            int counter = 0;
            string line;
            List<iris_datapoint> id = new List<iris_datapoint>();
            iris_datapoint dp;
            // Read the file and display it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader("iris_data.txt");
            while ((line = file.ReadLine()) != null)
            {
                dp = new iris_datapoint();
                string[] s = line.Split(new char[] { ',' }, StringSplitOptions.None);
                if (s.Length > 1)
                {
                    dp.sepal_length = Convert.ToDouble(s[0]);
                    dp.sepal_width = Convert.ToDouble(s[1]);
                    dp.petal_length = Convert.ToDouble(s[2]);
                    dp.petal_width = Convert.ToDouble(s[3]);
                    switch (s[4])
                    {
                        case "Iris-setosa":
                            dp.species = 0;
                            break;
                        case "Iris-versicolor":
                            dp.species = 1;
                            break;
                        case "Iris-virginica":
                            dp.species = 2;
                            break;
                    }
                    dp.id_number = counter;
                    dp.currentCentroid = counter % numberOfClusters;
                    id.Add(dp);
                    counter++;
                }
            }
            file.Close();
            return id;
        }

        public static List<centroid> populateCentroids(List<iris_datapoint> lid)
        {
            List<centroid> lc = new List<centroid>();
            double sl_min = lid.Min(id => id.sepal_length);
            double sw_min = lid.Min(id => id.sepal_width);
            double pl_min = lid.Min(id => id.petal_length);
            double pw_min = lid.Min(id => id.petal_width);
            double sl_max = lid.Max(id => id.sepal_length);
            double sw_max = lid.Max(id => id.sepal_width);
            double pl_max = lid.Max(id => id.petal_length);
            double pw_max = lid.Max(id => id.petal_width);
            for(int i = 1; i < (numberOfClusters + 1); i++)
            {
                double pl_i = pl_min + (i * ((pl_max - pl_min) / (numberOfClusters + 1)));
                double pw_i = pw_min + (i * ((pw_max - pw_min) / (numberOfClusters + 1)));
                double sl_i = sl_min + (i * ((sl_max - sl_min) / (numberOfClusters + 1)));
                double sw_i = sw_min + (i * ((sw_max - sw_min) / (numberOfClusters + 1)));
                lc.Add(new centroid(pl_i, pw_i, sl_i, sw_i, i - 1));
            }
            return lc;
        }

        private static double calculateMultidimensionalDistance(double d1, double c1, double d2 = 0, double c2 = 0, double d3 = 0, double c3 = 0, double d4 = 0, double c4 = 0)
        {
            double d = Math.Sqrt(Math.Pow((c1 - d1), 2) + Math.Pow((c2 - d2),2) + Math.Pow((c3 - d3), 2) + Math.Pow((c4 - d4), 2));
            return d;
        }

        private static List<centroid> kMeans(ref List<iris_datapoint> lid, List<centroid> lc, int limit)
        {
            centroid temp = new centroid();
            bool noChange = false;
            int counter = 0;

            while (!noChange && counter < limit)
            {
                noChange = true;
                foreach (centroid c in lc)
                {
                    c.assignedPoints.Clear();
                }
                foreach (iris_datapoint id in lid)
                {
                    foreach (centroid c in lc)
                    {
                        c.tempDistance = calculateMultidimensionalDistance(id.petal_length, c.petal_length, id.petal_width, c.petal_width, id.sepal_length, c.sepal_length, id.sepal_width, c.sepal_width);
                    }
                    temp = lc.First(cent => cent.tempDistance == lc.Min(c => c.tempDistance));
                    if(id.currentCentroid != temp.species)
                    {
                        noChange = false;
                    }
                    id.currentCentroid = temp.species;
                    temp.assignedPoints.Add(id);
                }
                foreach (centroid c in lc)
                {
                    c.petal_length = (c.assignedPoints.Count() > 0) ? c.assignedPoints.Average(x => x.petal_length) : 0;
                    c.petal_width = (c.assignedPoints.Count() > 0) ? c.assignedPoints.Average(x => x.petal_width) : 0;
                    c.sepal_length = (c.assignedPoints.Count() > 0) ? c.assignedPoints.Average(x => x.sepal_length) : 0;
                    c.sepal_width = (c.assignedPoints.Count() > 0) ? c.assignedPoints.Average(x => x.sepal_width) : 0;
                }
                counter++;
            }

            if (noChange)
            {
                Console.WriteLine("Completed Cycles.");
            }
            else
            {
                Console.WriteLine("Exceeded Run Limit of " + limit);
            }

            return lc;
        }

        private static double calculateAccuracy(ref List<iris_datapoint> lid)
        {
            double accuracy = 0;
            int count = 0;
            int total = lid.Count();

            foreach(iris_datapoint id in lid)
            {
                if(id.currentCentroid == id.species)
                {
                    count++;
                }
            }

            accuracy = (double)count / (double)total;

            return accuracy;
        }
    }

    public class iris_datapoint
    {
        public int id_number { get; set; }
        public double petal_length { get; set; }
        public double petal_width { get; set; }
        public double sepal_length { get; set; }
        public double sepal_width { get; set; }
        public int species { get; set; }
        public int currentCentroid { get; set; }
    }

    public class centroid
    {
        public double petal_length { get; set; }
        public double petal_width { get; set; }
        public double sepal_length { get; set; }
        public double sepal_width { get; set; }
        public int species { get; set; } //use as id
        public List<iris_datapoint> assignedPoints { get; set; }
        public double tempDistance { get; set; }

        public centroid() { }
        public centroid(double pl, double pw, double sl, double sw, int sp)
        {
            petal_length = pl;
            petal_width = pw;
            sepal_length = sl;
            sepal_width = sw;
            species = sp;
            assignedPoints = new List<iris_datapoint>();
        }
    }
}
