// See https://aka.ms/new-console-template for more information


using ConsoleTables;

Console.WriteLine("ASH/ASA Sum comparer\n");
Console.WriteLine("First file path: ");
var filesToRead = new List<string>();
string? firstFilePath = Console.ReadLine();
IsValidFile(firstFilePath);
filesToRead.Add(firstFilePath!);

Console.WriteLine("Second file path: ");
string? secondFilePath = Console.ReadLine();
IsValidFile(secondFilePath);
filesToRead.Add(secondFilePath!);

//Ask if more files to compare
Console.WriteLine("Do you want to compare more files? (y/n)");
string? moreFiles = Console.ReadLine();
while (moreFiles?.StartsWith("y") == true)
{
    Console.WriteLine("File path: ");
    string? filePath = Console.ReadLine();
    IsValidFile(filePath);
    filesToRead.Add(filePath!);
    Console.WriteLine("Do you want to compare more files? (y/n)");
    moreFiles = Console.ReadLine();
}

var sumLists = new List<float[]>();
foreach (var path in filesToRead)
{
    var sums = ReadAshFileSums(path);
    sumLists.Add(sums);
}

var headers = new string[filesToRead.Count * 2];
headers[0] = "#";
for (int i = 1; i < filesToRead.Count + 1; i++)
{
    headers[i] = "File " + i;
}

for (int i = filesToRead.Count + 1; i < filesToRead.Count * 2; i++)
{
    headers[i] = "Diff 1 & " + (i - filesToRead.Count + 1);
}

var ct = new ConsoleTable(headers);

var durationsRow = new object[headers.Length];
durationsRow[0] = "Duration";
for (int i = 1; i < durationsRow.Length; i++)
{
    if (sumLists.Count < i)
    {
        durationsRow[i] = "N/A";
    }
    else
    {
        durationsRow[i] = sumLists[i-1].Length;
    }
}
ct.AddRow(durationsRow);

var highestSumCount = FindHighestNumber(sumLists.Select(x => x.Length).ToArray());

for (int i = 0; i < highestSumCount; i++)
{
    var row = new object[headers.Length];
    row[0] = i;
    for (int j = 1; j < row.Length; j++)
    {
        //Values
        if (j < sumLists.Count + 1)
        {
            if (i < sumLists[j-1].Length)
            {
                row[j] = sumLists[j-1][i];
            }
            else
            {
                row[j] = "n/a";
            }
        }
        //Difference between 0 and j
        else
        {
            if (i < sumLists[j-sumLists.Count].Length && i < sumLists[0].Length)
            {
                row[j] = sumLists[0][i] - sumLists[j-sumLists.Count][i];
            }
            else
            {
                row[j] = "n/a";
            }
        }
    }
    ct.AddRow(row);
}
var asString = ct.ToMinimalString();
await File.WriteAllTextAsync("output.txt", asString);
ct.Write(Format.Minimal);

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

//Finds the highest number in the array
static int FindHighestNumber(int[] numbers)
{
    int highestNumber = numbers[0];
    for (int i = 1; i < numbers.Length; i++)
    {
        if (numbers[i] > highestNumber)
        {
            highestNumber = numbers[i];
        }
    }
    return highestNumber;
}

static bool IsValidFile(string? filePath)
{
    if (filePath is null)
    {
        return false;
    }
    if (!File.Exists(filePath))
    {
        return false;
    }
    return true;
}


static float[] ReadAshFileSums(string path)
{
    float[] sums;
    var normalizedPath = path.Replace("\"", "");
    using var input = File.OpenRead(normalizedPath);
    using var binaryReader = new BinaryReader(input);

    sums = Array.Empty<float>();
    try
    {
        //_shape = null;
        //_shape = new sbyte[numShapeNodes];
        for (int j = 0; j < 256; j++)
        {
            //Skip shape stuff
            _ = (sbyte)binaryReader.ReadByte();
        }
        int sumCount = binaryReader.ReadInt32();
        if (sumCount != 0)
        {
            sums = new float[sumCount];
            for (int k = 0; k < sumCount; k++)
            {
                sums[k] = binaryReader.ReadSingle();
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        Console.WriteLine(ex.Message);
    }
    return sums;
}