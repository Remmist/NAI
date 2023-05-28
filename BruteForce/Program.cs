namespace BruteForce;

internal class Program
{
    //private static int _maxCapacity;
    private static int _maxWeight;
    
    private static string _bestCombination;
    private static int _bestWeight;
    private static int _bestWartosc;

    private class Item
    {
        private int _wartosc;
        private int _waga;

        public int Wartosc
        {
            get => _wartosc;
            set => _wartosc = value;
        }

        public int Waga
        {
            get => _waga;
            set => _waga = value;
        }
    }

    static void Main(string[] args)
    {
        var path = @"C:\Users\r3mm1\Desktop\plecak.txt";
        var lines = File.ReadLines(path).ToList();

        _maxWeight = Convert.ToInt32(lines[0].Split(" ")[0]);
        //_maxCapacity = Convert.ToInt32(lines[0].Split(" ")[1]);

        var items = new List<Item>();
        var config1 = lines[1].Split(",");
        var config2 = lines[2].Split(",");

        for (int i = 0; i < config1.Length; i++)
        {
            items.Add(new Item()
            {
                Wartosc = Convert.ToInt32(config1[i]),
                Waga = Convert.ToInt32(config2[i]),
            });
        }

        for (int i = 1; i <= Math.Pow(2,items.Count); i++)
        {
            var bin = Convert.ToString(i, 2);
        
            var extend = "";
            if (bin.Length < items.Count)
            {
                for (int j = 0; j < items.Count - bin.Length; j++)
                {
                    extend += "0";
                }
                extend += bin;
                bin = extend;
            }
            
            
            var tmpWartosc = 0;
            var tmpWeight = 0;
            var fit = true;
            
            for (int j = 0; j < bin.Length; j++)
            {
                if (bin[j] == '1')
                {
                    if (tmpWeight + items.ElementAt(j).Waga <= _maxWeight)
                    {
                        tmpWeight += items.ElementAt(j).Waga;
                        tmpWartosc += items.ElementAt(j).Wartosc;
                    }
                    else
                    {
                        fit = false;
                        break;
                    }
                }
            }
            if (fit && tmpWartosc > _bestWartosc)
            {
                _bestCombination = bin;
                _bestWeight = tmpWeight;
                _bestWartosc = tmpWartosc;
                Console.Out.WriteLine("Successfully found a new combination: " + _bestCombination + " with weight = " + _bestWeight + " and value = " + _bestWartosc);
            }
        }

        Console.Out.WriteLine("======================");
        Console.Out.WriteLine("BruteForce completed.");
        Console.Out.WriteLine("Founded the following values: ");
        Console.Out.WriteLine("Best value: " + _bestWartosc);
        Console.Out.WriteLine("Best weight: " + _bestWeight);
        Console.Out.WriteLine("Best combination (binary): " + _bestCombination);




        // var test = Convert.ToString(3, 2);
        // Console.Out.WriteLine(test);
        // var tt = "";
        // for (int i = 0; i < items.Count - test.Length; i++)
        // {
        //     tt += "0";
        // }
        //
        // tt += test;
        // test = tt;
        // Console.Out.WriteLine(test);
        // Console.Out.WriteLine(test.Length);




        // var test = "011001001";
        // Console.Out.WriteLine(items[1].Wartosc);
        //
        // Console.Out.WriteLine(test[1]);
        // var t = Int32.Parse(test[1].ToString());
        // Console.Out.WriteLine(t);
        //
        // Console.Out.WriteLine(items[t].Wartosc);



    }
}