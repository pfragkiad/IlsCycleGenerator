

using System.Diagnostics;
using System.Text;

namespace Ils;

internal class Program
{
    private static void Main(string[] args)
    {

#if DEBUG
        args = [@"d:\Desktop\newconverter\2024-08-27 ILS\20240819_HMC_FCNT_2023_TR_v7.csv"];
#endif

        if (args.Length == 0)
        {
            Console.WriteLine("Please pass at least a file as an argument to the program.");
            return;
        }

        string csvFile = args[0];


        var originalDataLines = DataLine.ReadFromFile(csvFile);

        Console.WriteLine("Read {0} lines from the file.", originalDataLines.Count);


    }

}