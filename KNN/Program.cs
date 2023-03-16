namespace KNN;

internal class Program
{
    private static string trainSet;
    private static string testSet;
    private static int k;

    private static List<Flower> activeList = new List<Flower>();
    private static List<Flower> testList = new List<Flower>();
    private static List<Flower> answers = new List<Flower>();
    
    private class Flower
    {
        private string _type;
        private double _x;
        private double _y;
        private double _z;
        private double _c;

        public string Type
        {
            get => _type;
            set => _type = value ?? throw new ArgumentNullException(nameof(value));
        }

        public double X
        {
            get => _x;
            set => _x = value;
        }

        public double Y
        {
            get => _y;
            set => _y = value;
        }

        public double Z
        {
            get => _z;
            set => _z = value;
        }

        public double C
        {
            get => _c;
            set => _c = value;
        }
        
        public double CalculateDistance(double x1, double y1, double z1, double c1)
        {
            //Calculating distance without sqrt
            return (Math.Pow((_x - x1), 2) + Math.Pow((_y - y1), 2) + Math.Pow((_z - z1), 2) + Math.Pow((_c - c1), 2));
        }

        public bool IsSame(Flower flower)
        {
            if (flower.X == _x && flower.Y == _y && flower.Z == _z && flower.C == _c)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    
    private class Combination
    {
        private string _type;
        private double _distance;

        public string Type
        {
            get => _type;
            set => _type = value ?? throw new ArgumentNullException(nameof(value));
        }

        public double Distance
        {
            get => _distance;
            set => _distance = value;
        }
    }
    
    static void Main(string[] args)
    {
        if (args.Length is > 3 or < 3)
        {
            throw new ArgumentException("Too many args or less");
        }
        trainSet = args[0];
        testSet = args[1];
        k = int.Parse(args[2]);
        
        {
            var lines = File.ReadLines(trainSet);
            foreach (string line in lines)
            {
                var settings = line.Split(",");
                for (int i = 0; i < settings.Length; i++)
                {
                    settings[i] = settings[i].Replace(".", ",");
                }
                activeList.Add(new Flower
                {
                    X = Convert.ToDouble(settings[0]),
                    Y = Convert.ToDouble(settings[1]),
                    Z = Convert.ToDouble(settings[2]),
                    C = Convert.ToDouble(settings[3]),
                    Type = settings[4]
                });
            }
        }
        {
            var lines = File.ReadLines(testSet);
            foreach (string line in lines)
            {
                var settings = line.Split(",");
                for (int i = 0; i < settings.Length; i++)
                {
                    settings[i] = settings[i].Replace(".", ",");
                }
                testList.Add(new Flower
                {
                    X = Convert.ToDouble(settings[0]),
                    Y = Convert.ToDouble(settings[1]),
                    Z = Convert.ToDouble(settings[2]),
                    C = Convert.ToDouble(settings[3])
                });
            }
        }
        {
            var lines = File.ReadLines("test-set with answers.txt");
            foreach (string line in lines)
            {
                var settings = line.Split(",");
                for (int i = 0; i < settings.Length; i++)
                {
                    settings[i] = settings[i].Replace(".", ",");
                }
                answers.Add(new Flower
                {
                    X = Convert.ToDouble(settings[0]),
                    Y = Convert.ToDouble(settings[1]),
                    Z = Convert.ToDouble(settings[2]),
                    C = Convert.ToDouble(settings[3]),
                    Type = settings[4]
                });
            }
        }
        
        
        foreach (Flower testFlower in testList)
        {
            var distanceList = new List<Combination>();
            foreach (Flower activeFlower in activeList)
            {
                distanceList.Add(new Combination
                {
                    Type = activeFlower.Type,
                    Distance = activeFlower.CalculateDistance(testFlower.X,testFlower.Y,testFlower.Z,testFlower.C)
                });
            }
            
            distanceList.Sort((x,y) => 
                x.Distance.CompareTo(y.Distance));

            int counterSetosa = 0;
            int counterVersicolor = 0;
            int counterVirginica = 0;
            foreach (var combination in distanceList.GetRange(0,k))
            {
                if (combination.Type == "Iris-setosa")
                {
                    counterSetosa++;
                }
                if (combination.Type == "Iris-versicolor")
                {
                    counterVersicolor++;
                }
                if (combination.Type == "Iris-virginica")
                {
                    counterVirginica++;
                }
            }

            if (counterSetosa > counterVersicolor && counterSetosa > counterVirginica)
            {
                testFlower.Type = "Iris-setosa";
                continue;
            }
            if (counterVersicolor > counterSetosa && counterVersicolor > counterVirginica)
            {
                testFlower.Type = "Iris-versicolor";
                continue;
            }
            if (counterVirginica > counterVersicolor && counterVirginica > counterSetosa)
            {
                testFlower.Type = "Iris-virginica";
                continue;
            }
            if (counterSetosa == counterVersicolor)
            {
                Random random = new Random();
                if (random.Next(0,100) < 100)
                {
                    testFlower.Type = "Iris-versicolor";
                }
                else
                {
                    testFlower.Type = "Iris-setosa";
                }
                continue;
            }
            if (counterSetosa == counterVirginica)
            {
                Random random = new Random();
                if (random.Next(0,100) < 100)
                {
                    testFlower.Type = "Iris-virginica";
                }
                else
                {
                    testFlower.Type = "Iris-setosa";
                }
                continue;
            }
            if (counterVersicolor == counterVirginica)
            {
                Random random = new Random();
                if (random.Next(0,100) < 100)
                {
                    testFlower.Type = "Iris-virginica";
                }
                else
                {
                    testFlower.Type = "Iris-versicolor";
                }
            }
        }

        Console.Out.WriteLine("Results from test-set with k = " + k + "\t(Assigned type // Right answer type)");
        int errors = 0;
        foreach (var flower in testList)
        {
            foreach (var answer in answers)
            {
                if (answer.IsSame(flower))
                {
                    Console.Out.WriteLine("Flower X:" + flower.X + "\tY:" + flower.Y + "\tZ:" + flower.Z + 
                                          "\tC:" + flower.C +"\t" + flower.Type + " // " + answer.Type);
                    if (flower.Type != answer.Type)
                    {
                        errors++;
                    }
                    break;
                }
            }
        }

        double accuracy = (45.0 - errors)/45.0 * 100.0;
        Console.Out.WriteLine("Accuracy is: " + Math.Round(accuracy,2) + "% with " + errors + " incorrect answers");
        
        
        









    }
}