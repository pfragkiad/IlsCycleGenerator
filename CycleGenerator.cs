using System.Text;

namespace Ils;

//dotnet publish IlsCycleGenerator.csproj  -p:PublishSingleFile=true --self-contained false -c Release

public static class CycleGenerator
{
    public static void GenerateCycle(string csvFile)
    {
        (string header, List<DataLine> dataLines) = DataLine.ReadFromFile(csvFile);
        Console.WriteLine("{0} original lines were read.", dataLines.Count);

        var mergedDataLines = MergeLinesWithSameValues(dataLines);
        Console.WriteLine("{0} lines after merging lines with same values.", mergedDataLines.Count);

        var expandedDataLines = ExpandLinesWithTimeStepLargerThan20k(mergedDataLines);
        Console.WriteLine("{0} lines after expanding lines with timestep larger than 20 seconds.", expandedDataLines.Count);

        if(expandedDataLines.Count > 10000) 
            Console.WriteLine("WARNING: The number of lines exceeds 10,000. You should consider changing the source file.");

        string fileOutput = GetFileOutFileName(csvFile);
        Console.WriteLine("Writing to file {0}...", fileOutput);
        WriteToFile(fileOutput, header, expandedDataLines);

        Console.WriteLine("Done.");
    }

    public static List<DataLine> MergeLinesWithSameValues(List<DataLine> dataLines)
    {
        List<DataLine> mergedDataLines = [];


        for (int iCurrentLine = 0; iCurrentLine < dataLines.Count - 1; iCurrentLine++)
        {
            var currentLine = dataLines[iCurrentLine];
            int totalTimeStep = currentLine.TimeStep;

            int iLastLineWithSameValues;
            for (iLastLineWithSameValues = iCurrentLine + 1; iLastLineWithSameValues < dataLines.Count; iLastLineWithSameValues++)
            {
                if (dataLines[iLastLineWithSameValues].Values != currentLine.Values)
                {
                    iLastLineWithSameValues--;
                    break;
                }
                totalTimeStep += dataLines[iLastLineWithSameValues].TimeStep;
            }

            if (iLastLineWithSameValues == iCurrentLine)
            {
                mergedDataLines.Add(currentLine);
                continue;
            }

            iCurrentLine = iLastLineWithSameValues;
            mergedDataLines.Add(new DataLine { Values = currentLine.Values, TimeStep = totalTimeStep });
        }

        //add the very last item if it is different from the previous one
        if (dataLines[^1].Values != dataLines[^2].Values)
            mergedDataLines.Add(dataLines[^1]);

        return mergedDataLines;
    }

    public static List<DataLine> ExpandLinesWithTimeStepLargerThan20k(List<DataLine> dataLines)
    {
        List<DataLine> expandedDataLines = [];

        foreach (var dataLine in dataLines)
        {
            if (dataLine.TimeStep <= 20000)
            {
                expandedDataLines.Add(dataLine);
                continue;
            }

            int timeStep = dataLine.TimeStep;
            while (timeStep > 20000)
            {
                expandedDataLines.Add(new DataLine { TimeStep = 20000, Values = dataLine.Values });
                timeStep -= 20000;
            }
            expandedDataLines.Add(new DataLine { TimeStep = timeStep, Values = dataLine.Values });
        }

        return expandedDataLines;
    }

    public static string GetFileOutFileName(string csvFile)
    {
        string fileName = Path.GetFileNameWithoutExtension(csvFile);
        string fileExtension = Path.GetExtension(csvFile);
        return Path.Combine(Path.GetDirectoryName(csvFile)!, $"{fileName}_out{fileExtension}");
    }

    public static void WriteToFile(string csvFile, string header, List<DataLine> dataLines)
    {
        using var writer = new StreamWriter(csvFile, false, Encoding.UTF8);
        writer.WriteLine(header);

        int iLine = 0;
        foreach (var dataLine in dataLines)
        {
            writer.WriteLine($"{++iLine};{dataLine.TimeStep};{dataLine.Values}");
        }
    }

}


