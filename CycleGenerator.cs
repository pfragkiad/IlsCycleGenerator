using System;

namespace Ils;

public static class CycleGenerator
{
    public static void GenerateCycle(string csvFile)
    {
        var originalDataLines = DataLine.ReadFromFile(csvFile);

        Console.WriteLine("Read {0} lines from the file.", originalDataLines.Count);

    }

}
