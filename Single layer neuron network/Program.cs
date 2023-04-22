using System.Text.RegularExpressions;

namespace Single_layer_neuron_network;

internal class Program
{
    private static double alpha = 0.5;
    private static List<Text> TrainList = new List<Text>();
    private static List<Text> TestList = new List<Text>();
    private static List<Perceptron> Perceptrons = new List<Perceptron>();


    static void Main(string[] args)
    {
        var languages = Directory.GetDirectories("Languages");
        var testLanguages = Directory.GetDirectories("LanguagesForTest");

        foreach (var language in languages)
        {
            var files = Directory.GetFiles(language);
            foreach (var file in files)
            {
                var text = new Text();
                string pattern = @"(?<=\\)[^\\]+(?=\\[^\\]+$)";
                Match match = Regex.Match(file, pattern);
                text.SetUpProportions(file, match.Groups[0].Value);
                TrainList.Add(text);
            }
        }
        
        foreach (var language in testLanguages)
        {
            var files = Directory.GetFiles(language);
            foreach (var file in files)
            {
                var text = new Text();
                string pattern = @"(?<=\\)[^\\]+(?=\\[^\\]+$)";
                Match match = Regex.Match(file, pattern);
                text.SetUpProportions(file, match.Groups[0].Value);
                TestList.Add(text);
            }
        }
        
        
        foreach (var language in languages)
        {
            var per = new Perceptron();
            SetupPerceptron(per);
            per.Type = language.Remove(0, 10);
            Perceptrons.Add(per);
        }


        Console.Out.WriteLine("Perceptrons (before learning):");
        foreach (var perceptron in Perceptrons)
        {
            Console.Out.WriteLine(perceptron.Type + "\t" +  " |Ready? - "+ perceptron.Ready + ", epochs - " + perceptron.Epoki + "|");
        }
        
        
        
        
        for (int i = 0; i < 1000; i++)
        {
            foreach (var perceptron in Perceptrons)
            {
                if (perceptron.Ready)
                {
                    continue;
                }
                int accuracy = 0;
                var incorrect = new List<Text>();
                foreach (var text in TrainList)
                {
                    int dec = perceptron.MakeDecision(perceptron.CalculateNet(text.Proportions));
                    if ((dec == 1 && text.Language == perceptron.Type) || (dec == 0 && text.Language != perceptron.Type))
                    {
                        accuracy++;
                        continue;
                    }
                    incorrect.Add(text);
                }
                
                if (incorrect.Count == 0)
                {
                    perceptron.SavePreset(accuracy);
                    perceptron.Ready = true;
                    perceptron.Epoki = i;
                    continue;
                }
                
                if (accuracy > perceptron.BestAccuracy)
                {
                    perceptron.SavePreset(accuracy);
                }
                
                if (incorrect.ElementAt(0).Language == perceptron.Type &&
                    perceptron.MakeDecision(perceptron.CalculateNet(incorrect.ElementAt(0).Proportions)) == 0)
                {
                    perceptron.Learn(incorrect.ElementAt(0).Proportions, 1);
                }
                
                if (incorrect.ElementAt(0).Language != perceptron.Type &&
                    perceptron.MakeDecision(perceptron.CalculateNet(incorrect.ElementAt(0).Proportions)) == 1)
                {
                    perceptron.Learn(incorrect.ElementAt(0).Proportions, 0);
                }
                
            }
        }

        foreach (var perceptron in Perceptrons)
        {
            perceptron.LoadPreset();
        }

        Console.Out.WriteLine();
        Console.Out.WriteLine("Perceptrons (after learning on train-set):");
        foreach (var perceptron in Perceptrons)
        {
            Console.Out.WriteLine(perceptron.Type + "\t" +  " |Ready? - "+ perceptron.Ready + ", epochs - " + perceptron.Epoki + "|");
        }
        Console.Out.WriteLine();
        Console.Out.WriteLine("Test-set results:");
        Console.Out.Write("\t\t");
        
        foreach (var perceptron in Perceptrons)
        {
            Console.Out.Write(perceptron.Type + "\t");
        }
        Console.Out.WriteLine();
        
        
        
        
        int rightAnswers = 0;
        int incorrectAnswers = 0;
        foreach (var text in TestList)
        {
            Console.Out.Write("Text " + text.Language + "\t");
            var answers = new List<int>();
            foreach (var perceptron in Perceptrons)
            {
                int ans = perceptron.MakeDecision(perceptron.CalculateNet(text.Proportions));
                answers.Add(ans);
                //Console.Out.Write(perceptron.MakeDecision(perceptron.CalculateNet(text.Proportions)) + "\t");
            }

            if (answers.Sum() != 1)
            {
                double maxNet = 0;
                string activatedType = "";
                foreach (var perceptron in Perceptrons)
                {
                    if (maxNet < perceptron.CalculateNet(text.Proportions))
                    {
                        maxNet = perceptron.CalculateNet(text.Proportions);
                        activatedType = perceptron.Type;
                    }
                }
                if (activatedType != text.Language)
                {
                    incorrectAnswers++;
                }
                if (activatedType == text.Language)
                {
                    rightAnswers++;
                }

                foreach (var perceptron in Perceptrons)
                {
                    if (perceptron.Type == activatedType)
                    {
                        Console.Out.Write(1 + "\t");
                    }
                    else
                    {
                        Console.Out.Write(0 + "\t");
                    }
                }
            }
            else
            {
                rightAnswers++;
                foreach (var perceptron in Perceptrons)
                {
                    Console.Out.Write(perceptron.MakeDecision(perceptron.CalculateNet(text.Proportions)) + "\t");
                }
            }
            Console.Out.WriteLine();
        }
        Console.Out.WriteLine();
        double finalAccuracy = (double)rightAnswers / ( double)TestList.Count * 100.0;
        Console.Out.WriteLine("Accuracy of neuron network - " + Math.Round(finalAccuracy, 2) + "% with " + incorrectAnswers + " incorrect answers");
        Console.Out.WriteLine();

        Console.Out.WriteLine("You can enter yours text to test neuron network (WARNING, нou must enter text that matches perceptron languages)");

        while (true)
        {
            Console.Out.WriteLine("Enter the text: ");
            var input = Console.ReadLine();
            Text text = new Text();
            text.SetUpProportionsConsole(input);
            double maxNet = 0;
            string activatedType = "";
            foreach (var perceptron in Perceptrons)
            {
                if (maxNet < perceptron.CalculateNet(text.Proportions))
                {
                    maxNet = perceptron.CalculateNet(text.Proportions);
                    activatedType = perceptron.Type;
                }
            }
            Console.Out.WriteLine("Assigned language - " + activatedType);
            Console.Out.WriteLine();
        }
    }
    
    
    
    private class Perceptron
    {
        private string _type;
        private int _epoki = 0;
        private bool _ready = false;
        private List<double> _BestWektroWag = new List<double>();
        private double _BestProg;
        private int _BestAccuracy = 0;

        private List<double> _WectorWag = new List<double>();
        private double _prog;

        public List<double> WectorWag
        {
            get => _WectorWag;
            set => _WectorWag = value ?? throw new ArgumentNullException(nameof(value));
        }


        public int Epoki
        {
            get => _epoki;
            set => _epoki = value;
        }

        public bool Ready
        {
            get => _ready;
            set => _ready = value;
        }

        public string Type
        {
            get => _type;
            set => _type = value ?? throw new ArgumentNullException(nameof(value));
        }

        public double Prog
        {
            get => _prog;
            set => _prog = value;
        }

        public double CalculateNet(List<double> Wektor)
        {
            double net = 0;
            for (int i = 0; i < _WectorWag.Count; i++)
            {
                net += _WectorWag.ElementAt(i) * Wektor.ElementAt(i);
            }
            return net;
        }

        public int MakeDecision(double net)
        {
            return net >= _prog ? 1 : 0;;
        }
        

        public void Learn(List<double> WektorWejsciowy, int WyjscieOczekiwane)
        {
            int AktualneWyjscie = MakeDecision(CalculateNet(WektorWejsciowy));
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
    
    private static void SetupPerceptron(Perceptron perceptron)
    {
        List<double> wagi = new List<double>();
        Random random = new Random();

        for (int i = 0; i < TrainList.ElementAt(0).Proportions.Count(); i++)
        {
            wagi.Add(random.Next(0, 10));
        }
        double prog = random.Next(0, 3);
        perceptron.Prog = prog;
        perceptron.WectorWag = wagi;
    }
    
    
    
    private class Text
    {
        private string _language;
        private List<double> _proportions = new List<double>();

        public string Language
        {
            get => _language;
            set => _language = value ?? throw new ArgumentNullException(nameof(value));
        }

        public List<double> Proportions
        {
            get => _proportions;
            set => _proportions = value ?? throw new ArgumentNullException(nameof(value));
        }

        public void SetUpProportions(string path, string language)
        {
            _language = language;
            var lettersList = new List<char>();
            var text = File.ReadAllText(path);
            foreach (var symbol in text)
            {
                char letter = Char.ToUpper(symbol);
                if (letter >= 65 && letter <= 90)
                {
                    lettersList.Add(letter);
                }
            }
            _proportions.Add((double)lettersList.Count(c => c == 'A')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'B')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'C')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'D')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'E')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'F')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'G')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'H')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'I')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'J')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'K')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'L')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'M')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'N')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'O')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'P')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'Q')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'R')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'S')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'T')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'U')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'V')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'W')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'X')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'Y')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'Z')/(double)lettersList.Count);
        }

        public void SetUpProportionsConsole(string input)
        {
            _language = "NP";
            var lettersList = new List<char>();
            foreach (var symbol in input)
            {
                char letter = Char.ToUpper(symbol);
                if (letter >= 65 && letter <= 90)
                {
                    lettersList.Add(letter);
                }
            }
            _proportions.Add((double)lettersList.Count(c => c == 'A')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'B')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'C')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'D')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'E')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'F')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'G')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'H')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'I')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'J')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'K')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'L')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'M')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'N')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'O')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'P')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'Q')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'R')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'S')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'T')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'U')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'V')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'W')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'X')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'Y')/(double)lettersList.Count);
            _proportions.Add((double)lettersList.Count(c => c == 'Z')/(double)lettersList.Count);
        }
        
        
        
    }
    
    
    
    
}