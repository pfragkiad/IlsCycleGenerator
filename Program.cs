

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



        using var reader = new StreamReader(csvFile, Encoding.UTF8);
        string? header = reader.ReadLine();
        if (header is null)
        {
            Console.WriteLine("The file is empty.");
            return;
        }

        string? firstDataLine = reader.ReadLine();
        if (firstDataLine is null)
        {
            Console.WriteLine("The file has only the header.");
            return;
        }

        //read datalines
        List<DateLine> dateLines = [];
        dateLines.Add(DateLine.FromString(firstDataLine!)!.Value);
        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine();
            if (line is not null)
            {
                DateLine? dateLine = DateLine.FromString(line);
                dateLines.Add(dateLine!.Value);
            }
        }

        Debugger.Break();
    }
}



