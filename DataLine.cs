
namespace Ils;

using System.IO;
using System.Text;

public readonly struct DataLine
{
    public int TimeStep { get; init; }
    public string Values { get; init; }

    public static DataLine? FromString(string? line)
    {
        if (string.IsNullOrWhiteSpace(line)) return null;

        //TTW is timestep

        /*
        ID;TTW;FeedMode;T_SP;Total Rich;Total Lean;O2 rich;CO rich;CO2 rich;NO rich;NO2 rich;NH3 rich;N2O rich;H2 rich;SO2 rich;C2H4 rich;C3H6 rich;C3H8 rich;CH4 rich;H2O rich;O2 lean;CO lean;CO2 lean;NO lean;NO2 lean;NH3 lean;N2O lean;H2 lean;SO2 lean;C2H4 lean;C3H6 lean;C3H8 lean;CH4 lean;H2O lean
        1;200;3;352.38;104.6586;0;0.947022093;0.596118309;16.28481605;0.260859092;0;0;0;0;0;0;0.107961409;0;0;14.31466;0;0;0;0;0;0;0;0;0;0;0;0;0;0
        */

        string[] tokens = line.Split(';', count: 3);

        int timeStep = int.Parse(tokens[1]);
        string values = tokens[2];

        return new DataLine() { TimeStep = timeStep, Values = values };
    }

    public static (string Header, List<DataLine> DataLines) ReadFromFile(string csvFile)
    {
        using var reader = new StreamReader(csvFile, Encoding.UTF8);

        string? header = reader.ReadLine();
        if (header is null)
        {
            Console.WriteLine("The file is empty.");
            return ("", []);
        }

        string? firstDataLine = reader.ReadLine();
        if (firstDataLine is null)
        {
            Console.WriteLine("The file has only the header.");
            return (header, []);
        }

        List<DataLine> dataLines = [];

        dataLines.Add(DataLine.FromString(firstDataLine!)!.Value);
        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine();
            if (line is null) break;

            DataLine? dateLine = DataLine.FromString(line);
            if (dateLine is not null) dataLines.Add(dateLine.Value);
        }
        return (header, dataLines);
    }


}
