namespace BruteForce;

internal class Program
{
    private static int _maxWeight;
    
    private static string _bestCombination = null!;
    private static int _bestWeight = 0;
    private static int _bestWartosc = 0;

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
        var items = Config(@"C:\Users\r3mm1\Desktop\plecak.txt");
        
        for (int i = 1; i <= Math.Pow(2, items.Count); i++)
        {
            var bin = Convert_binary(i, items.Count);
            
            var tmpWartosc = 0;
            var tmpWeight = 0;

            for (int j = 0; j < bin.Length; j++)
            {
                if (bin[j] == '1')
                {
                    tmpWeight += items.ElementAt(j).Waga;
                    tmpWartosc += items.ElementAt(j).Wartosc;
                }
            }
            if (tmpWeight <= _maxWeight)
            {
                if (tmpWartosc > _bestWartosc)
                {
                    _bestCombination = bin;
                    _bestWeight = tmpWeight;
                    _bestWartosc = tmpWartosc;
                    Console.Out.WriteLine("Successfully found a new combination: " + _bestCombination + " with weight = " + _bestWeight + " and value = " + _bestWartosc);
                }
            }
        }
        
        Console.Out.WriteLine("======================");
        Console.Out.WriteLine("BruteForce completed.");
        Console.Out.WriteLine("Founded the following values: ");
        Console.Out.WriteLine("Best value: " + _bestWartosc);
        Console.Out.WriteLine("Best weight: " + _bestWeight);
        Console.Out.WriteLine("Best combination (binary): " + _bestCombination);
    }
    
    
    private static List<Item> Config(string path)
    {
        var lines = File.ReadLines(path).ToList();
        _maxWeight = Convert.ToInt32(lines[0].Split(" ")[0]);
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
        return items;
    }
    

    private static string Convert_binary(int i, int lenght)
    {
        var bin = Convert.ToString(i, 2);
            
        if (bin.Length < lenght)
        {
            var extend = "";
            for (int j = 0; j < lenght - bin.Length; j++)
            {
                extend += "0";
            }
            extend += bin;
            bin = extend;
        }

        return bin;
    }

    //Версия 2 на StringBuilder, но эта версия жрет еще больше памяти...
    // var extend = new StringBuilder();
    // if (bin.Length < items.Count)
    // {
    //     for (int j = 0; j < items.Count - bin.Length; j++)
    //     {
    //         extend.Append("0");
    //     }
    //     extend.Append(bin);
    //     bin = extend.ToString();
    // }
        
    //Версия 1
    // if (bin.Length < items.Count)
    // {
    //     var extend = "";
    //     for (int j = 0; j < items.Count - bin.Length; j++)
    //     {
    //         extend += "0";
    //     }
    //     extend += bin;
    //     bin = extend;
    // }
}