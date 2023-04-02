namespace Perceptron;

internal class Program
{
    private static double alpha;

    private static List<Flower> TrainList = new List<Flower>();
    private static List<Flower> TestList = new List<Flower>();

    private class Flower
    {
        private List<double> _arguments = new List<double>();
        private string _type;

        public List<double> Arguments
        {
            get => _arguments;
            set => _arguments = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Type
        {
            get => _type;
            set => _type = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
    
    private class Perceptron
    {
        private List<double> _BestWektroWag = new List<double>();
        private double _BestProg;
        private int _BestAccuracy;

        private List<double> _WectorWag = new List<double>();
        private double _prog;

        public List<double> WectorWag
        {
            get => _WectorWag;
            set => _WectorWag = value ?? throw new ArgumentNullException(nameof(value));
        }

        public double Prog
        {
            get => _prog;
            set => _prog = value;
        }

        public int Calculate(List<double> Wektor)
        {
            if (Wektor.Count > _WectorWag.Count)
            {
                Wektor = Wektor.GetRange(0, _WectorWag.Count-1);
            }
            if (Wektor.Count < _WectorWag.Count)
            {
                Random random = new Random();
                while (Wektor.Count < _WectorWag.Count)
                {
                    Wektor.Add(random.Next(0, 10));
                }
            }
            double number = 0;
            for (int i = 0; i < _WectorWag.Count; i++)
            {
                number += _WectorWag.ElementAt(i) * Wektor.ElementAt(i);
            }
            return number >= _prog ? 1 : 0; //1 - setosa        0 - versicolor
        }


        public void Learn(List<double> WektorWejsciowy, int WyjscieOczekiwane)
        {
            int AktualneWyjscie = Calculate(WektorWejsciowy);
            if (AktualneWyjscie == WyjscieOczekiwane)
            {
                return;
            }

            List<double> output = new List<double>();
            List<double> WektorWejsciowyUpdate = new List<double>();

            foreach (var value in WektorWejsciowy)
            {
                WektorWejsciowyUpdate.Add(value * alpha * (WyjscieOczekiwane - AktualneWyjscie));
            }

            double NewProg = (-1 * alpha * (WyjscieOczekiwane - AktualneWyjscie)) + _prog;

            for (int i = 0; i < _WectorWag.Count; i++)
            {
                output.Add(_WectorWag.ElementAt(i) + WektorWejsciowyUpdate.ElementAt(i));
            }

            _WectorWag = output;
            _prog = NewProg;
        }

        public void SavePreset(int Accuracy)
        {
            _BestWektroWag = _WectorWag;
            _BestProg = _prog;
            _BestAccuracy = Accuracy;
        }

        public void LoadPreset()
        {
            _WectorWag = _BestWektroWag;
            _prog = _BestProg;
        }

        public int BestAccuracy
        {
            get => _BestAccuracy;
        }
    }
    
    
    
    
    static void Main(string[] args)
    {
        if (args.Length is > 3 or < 3)
        {
            throw new ArgumentException("Too many args or less");
        }
        alpha = double.Parse(args[0]);
        var TrainSetPath = args[1];
        var TestSetPath = args[2];
        
        
        LoadSettings(TrainSetPath, 0);
        LoadSettings(TestSetPath, 1);

        Perceptron perceptron = new Perceptron();
        SetupPerceptron(perceptron);
        
        Console.Out.Write("Perceptron values before learning: ");
        foreach (var value in perceptron.WectorWag)
        {
            Console.Out.Write(value + "\t");
        }
        Console.Out.Write("Prog: " + perceptron.Prog + "\n");

        
        int iloscEpok = 50; //максимальное количество эпох для учебы
        int Epoki = 0;
        int accuracy = 0;
        
        
        for (int i = 0; i < iloscEpok; i++)
        {
            List<Flower> incorrect = new List<Flower>();
            foreach (var flower in TrainList)
            {
                int percOut = perceptron.Calculate(flower.Arguments);
                if ((percOut == 1 && flower.Type == "Iris-setosa") || (percOut == 0 && flower.Type == "Iris-versicolor"))
                {
                    //correct
                    accuracy++;
                }

                if ((percOut == 0 && flower.Type == "Iris-setosa") || (percOut == 1 && flower.Type == "Iris-versicolor"))
                {
                    //incorrect
                    incorrect.Add(flower);
                }
            }

            // Console.Out.WriteLine("Эпоха " + i + ": Correct = " + accuracy);
            
            if (incorrect.Count == 0)
            {
                perceptron.SavePreset(accuracy);
                // perceptron.LoadPreset();
                Epoki = i;
                break;
            }

            if (perceptron.BestAccuracy < accuracy)
            {
                perceptron.SavePreset(accuracy);
            }

            List<double> WektorWejsciowy = incorrect.ElementAt(0).Arguments;
            int WyjscieOczekiwane;
            if (incorrect.ElementAt(0).Type == "Iris-setosa")
            {
                WyjscieOczekiwane = 1;
            }
            else
            {
                WyjscieOczekiwane = 0;
            }
            
            perceptron.Learn(WektorWejsciowy, WyjscieOczekiwane);
            accuracy = 0;
        }
        perceptron.LoadPreset();
        
        double finalAccuracy = (70.0 - (70.0 - accuracy))/70.0 * 100.0;
        Console.Out.WriteLine("Perceptron learned within " + Epoki + " epochs on the train-set with an accuracy = " + Math.Round(finalAccuracy,2) + "%");
        Console.Out.Write("Perceptron values after learning: ");
        foreach (var value in perceptron.WectorWag)
        {
            Console.Out.Write(Math.Round(value, 2) + "\t");
        }
        Console.Out.Write("Prog: " + Math.Round(perceptron.Prog, 2) + "\n");


        Console.Out.WriteLine("\n"+"Test-set results:");
        int errors = 0;
        foreach (var flower in TestList)
        {
            int percOut = perceptron.Calculate(flower.Arguments);
            
            if ((percOut == 1 && flower.Type == "Iris-setosa") || (percOut == 0 && flower.Type == "Iris-versicolor"))
            {
                //correct
                Console.Out.Write("Flower: ");
                foreach (var val in flower.Arguments)
                {
                    Console.Out.Write(val + "\t");
                }
                Console.Out.Write(flower.Type + " // " + flower.Type + "\n");
            }
            
            if ((percOut == 0 && flower.Type == "Iris-setosa") || (percOut == 1 && flower.Type == "Iris-versicolor"))
            {
                //incorrect
                string answer = percOut == 0 ? "Iris-versicolor" : "Iris-setosa";
                Console.Out.Write("Flower: ");
                foreach (var val in flower.Arguments)
                {
                    Console.Out.Write(val + "\t");
                }
                Console.Out.Write(flower.Type + " // " + answer + "\n");
                errors++;
            }
        }
        double TestAccuracy = (70.0 - errors)/70.0 * 100.0;
        Console.Out.WriteLine("Accuracy is: " + Math.Round(TestAccuracy,2) + "% with " + errors + " incorrect answers");
        
        
        Console.Out.WriteLine("\n" + "You can input yours flower characteristics in form like  X,XX Y,YY Z,ZZ ... ");
        while (true)
        {
            Console.Out.Write("Input your characteristics: ");
            var line = Console.ReadLine();
            var settings = line.Split(" ");
            List<double> wagi = new List<double>();
            foreach (var value in settings)
            {
                wagi.Add(Convert.ToDouble(value));
            }
            Flower flower = new Flower()
            {
                Arguments = wagi
            };

            if (wagi.Count > perceptron.WectorWag.Count)
            {
                Console.Out.WriteLine("Characteristics contain more parameters than a perceptron, cutting characteristics for calculating...");
            }
            
            if (wagi.Count < perceptron.WectorWag.Count)
            {
                Console.Out.WriteLine("Characteristics contain less parameters than a perceptron, supplementing characteristics for calculating...");
            }
            
            int percOut = perceptron.Calculate(flower.Arguments);

            flower.Type = percOut == 0 ? "Iris-versicolor" : "Iris-setosa";

            Console.Out.Write("Flower: ");
            foreach (var val in flower.Arguments)
            {
                Console.Out.Write(val + "\t");
            }
            Console.Out.Write("Assigned type: " + flower.Type + "\n");
            Console.Out.WriteLine("");
        }
        
    }

    private static void LoadSettings(string path, int a)
    {
        {
            var lines = File.ReadLines(path);
            foreach (string line in lines)
            {
                var settings = line.Split(",");
                for (int i = 0; i < settings.Length; i++)
                {
                    settings[i] = settings[i].Replace(".", ",");
                }
                var newsettings = new List<string>(settings);
                var flower = new Flower();
                foreach (var value in newsettings.GetRange(0,newsettings.Count-1))
                {
                    flower.Arguments.Add(Convert.ToDouble(value));
                }
                flower.Type = newsettings.ElementAt(newsettings.Count-1);

                if (a == 0)
                {
                    TrainList.Add(flower);
                }
                else
                {
                    TestList.Add(flower);
                }
            }
        }
    }

    private static void SetupPerceptron(Perceptron perceptron)
    {
        List<double> wagi = new List<double>();
        Random random = new Random();

        for (int i = 0; i < TrainList.ElementAt(0).Arguments.Count; i++)
        {
            wagi.Add(random.Next(0, 10));
        }
        double prog = random.Next(0, 3);
        perceptron.Prog = prog;
        perceptron.WectorWag = wagi;
    }


}
