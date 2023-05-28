internal class Program
{
    static void Main(string[] args)
    {
        // crearing k from args and list from file
        var k = Convert.ToInt32(args[0]);
        List<double[]> vectors = ReadFile(args[1]);

        // run k-means clustering with k=3 and maxIterations=100
        KMeans kMeans = new KMeans(k, 10, vectors);
        kMeans.Cluster();

        // print clusters
        List<List<double[]>> clusters = kMeans.GetClusters();
        for (int i = 0; i < clusters.Count; i++) 
        {
            Console.Out.WriteLine("Cluster " + (i+1) + ":");
            var cluster = clusters.ElementAt(i);
            foreach (var vector in cluster) 
            {
                Console.Out.WriteLine("(" + vector[0] + ", " + vector[1] + ", " + vector[2] + ")");
            }

            Console.Out.WriteLine("");
        }
    }

    public static List<double[]> ReadFile(string path)
    {
        var vectors = new List<double[]>();
        var lines = File.ReadLines(path);
        foreach (string line in lines)
        {
            var settings = line.Split(",");
            var doubles = new double[settings.Length];
            for (int i = 0; i < settings.Length; i++)
            {
                settings[i] = settings[i].Replace(".", ",");
                doubles[i] = Convert.ToDouble(settings[i]);
            }
            vectors.Add(doubles);
        }
        return vectors;
    }



    

    private class KMeans
    {
        private int _k; // number of clusters
        private int _maxIterations; // maximum number of iterations
        private List<double[]> _vectors; // input vectors
        private List<double[]> _centroids; // centroids of clusters


        public KMeans(int k, int maxIterations, List<double[]> vectors)
        {
            _k = k;
            _maxIterations = maxIterations;
            _vectors = vectors;

            var alreadyTakenIndexes = new List<int>();
            var random = new Random();
            int x = random.Next(0, 10) * vectors.Count;
            
            _centroids.Add(vectors.ElementAt(x));
            alreadyTakenIndexes.Add(x);

            for (int i = 1; i < k; i++) 
            {
                while (alreadyTakenIndexes.Contains(x))
                {
                    x = random.Next() * vectors.Count;
                }

                _centroids.Add(vectors.ElementAt(x));
                alreadyTakenIndexes.Add(x);
            }
            Console.Out.WriteLine("Centroid list size is: " + _centroids.Count);
        }
        
        
        public void Cluster() {
            List<Double> euclideanSums;
            // repeat until convergence or maximum iterations reached
            for (int iteration = 0; iteration < _maxIterations; iteration++) 
            {
                // checking if previous sum is not the same

                euclideanSums = new List<double>();
                // initialize clusters and k 0's for sum at iteration start
                var clusters = new List<List<double[]>>();
                for (int i = 0; i < _k; i++) 
                {
                    clusters.Add(new List<double[]>());
                    euclideanSums.Add(0.0);
                }

                // assign vectors to nearest centroid

                foreach (var vector in _vectors)
                {
                    var nearestCentroid = GetNearestCentroid(vector);
                    var tmp = euclideanSums.ElementAt(nearestCentroid);
                    tmp += EuclideanDistance(vector, _centroids.ElementAt(nearestCentroid));
                    euclideanSums[nearestCentroid] = tmp;
                    clusters.ElementAt(nearestCentroid).Add(vector);
                }
                
                

                // update centroids
                for (int i = 0; i < _k; i++) 
                {
                    var newCentroid = new double[3];
                    var cluster = clusters.ElementAt(i);
                    if (cluster.Count != 0) 
                    {
                        for (int j = 0; j < 3; j++) 
                        {
                            var sum = 0.0;
                            foreach (var vector in cluster) 
                            {
                                sum += vector[j];
                            }
                            newCentroid[j] = sum / cluster.Count;
                        }
                    } 
                    else
                    {
                        newCentroid = _centroids.ElementAt(i);
                    }
                    _centroids[i] = newCentroid;
                }
                Console.Out.WriteLine("Iteration number: " + iteration + " with sums: " + euclideanSums);
            }
        }
        
        public int GetNearestCentroid(double[] vector) 
        {
            int nearestCentroid = 0;
            double nearestDistance = Double.MaxValue;
            for (int i = 0; i < _k; i++) 
            {
                double[] centroid = _centroids.ElementAt(i);
                double distance = EuclideanDistance(vector, centroid);
                if (distance < nearestDistance) 
                {
                    nearestCentroid = i;
                    nearestDistance = distance;
                }
            }
            return nearestCentroid;
        }
        
        public double EuclideanDistance(double[] a, double[] b) 
        {
            double sum = 0.0;
            for (int i = 0; i < 3; i++) 
            {
                double diff = a[i] - b[i];
                sum += diff * diff;
            }
            return Math.Sqrt(sum);
        }
        
        public List<List<double[]>> GetClusters() 
        {
            var clusters = new List<List<double[]>>();
            for (int i = 0; i < _k; i++) 
            {
                clusters.Add(new List<double[]>());
            }
            foreach (var vector in _vectors) 
            {
                int nearestCentroid = GetNearestCentroid(vector);
                clusters.ElementAt(nearestCentroid).Add(vector);
            }
            return clusters;
        }
        
        
    }
}
