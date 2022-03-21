using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace CSVReader
{
    internal class Order
    {
        public DateTime OrderDate { get; set; }
        public string Region { get; set; }
        public string Rep { get; set; }
        public string Item { get; set; }
        public int Unit { get; set; }
        public decimal UnitCost { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            //Setting up foldername for Input and Output
            string INPUT_CSV_FOLDERNAME = "Input";
            string OUTPUT_CSV_FOLDERNAME = "Output";
            //Taking inputs from the user
            string separator, inputLocale, outputLocale, fileName = string.Empty;
            Console.Write("Enter character indicator for the separator : ");
            separator = Console.ReadLine();
            Console.Write("Enter locale indicator for the input csv : ");
            inputLocale = Console.ReadLine();
            Console.Write("Enter locale indicator for output csv : ");
            outputLocale = Console.ReadLine();
            Console.Write("Enter csv filename : ");
            fileName = Console.ReadLine();

            //Input filepath is \CSVReader\bin\Debug\net5.0\Input and Output file Path is \CSVReader\bin\Debug\net5.0\Output
            var inputFilePath = $"{INPUT_CSV_FOLDERNAME}\\{fileName}";
            var outputFilePath = $"{OUTPUT_CSV_FOLDERNAME}\\Outputdata.csv";
            var path = Path.Combine(Directory.GetCurrentDirectory(), inputFilePath);

            //Returns invalid if CSV file is not found
            if (!File.Exists(path))
            {
                Console.WriteLine("CSV File doesn't exists!");
                Console.ReadKey();
                return;
            }


            string[] lines = File.ReadAllLines(path);

            var inputOrders = new List<Order>();
            var inputCulture = new CultureInfo(inputLocale);
            var outputCulture = new CultureInfo(outputLocale);
            //Declaring Numberstyle in according to the locale
            NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            //This is the argument to run a loop and parse Valid Date unit and cost accordingly
            for (var i = 1; i < lines.Length; i++)
            {
                var columns = lines[i].Split(separator);
                var isValidDate = DateTime.TryParse(columns[0].ToString().Trim(), inputCulture, DateTimeStyles.None, out DateTime date);
                var isValidUnit = int.TryParse(columns[4].ToString().Trim(), out int unit);
                var isUnitCost = decimal.TryParse(columns[5].ToString().Trim().ToString(), style, inputCulture, out decimal unitCost);
                //If no valid then output Invalid Data Format
                if (!isValidDate || !isValidUnit || !isUnitCost)
                {
                    Console.WriteLine("Invalid Data Format");
                    Console.ReadKey();
                    return;
                }
                //Passing the values to a list
                inputOrders.Add(new Order()
                {
                    OrderDate = date,
                    Region = columns[1].ToString().Trim(),
                    Rep = columns[2].ToString().Trim(),
                    Item = columns[3].ToString().Trim(),
                    Unit = unit,
                    UnitCost = unitCost,
                });
            }
            //Declaring Outputpath an outputlines
            var outputFullPath = Path.Combine(Directory.GetCurrentDirectory(), outputFilePath);
            var outputLines = new List<string>();
            //Add top line with given separator from user
            outputLines.Add($"Last OrderDate{ separator }Region{ separator }TotalUnits{ separator }Total Cost");
            //Sorting in order of date and time
            inputOrders.Sort((x, y) => DateTime.Compare(x.OrderDate, y.OrderDate));
            //this loop adds all other rows by serial: Last order date, Region, Unit and total cost
            foreach (var order in inputOrders)
            {
                var totalCost = order.UnitCost * order.Unit;
                outputLines.Add($"{order.OrderDate.ToString(outputCulture)}{separator}{order.Region}{separator}{order.Unit}{separator}{totalCost.ToString(outputCulture)}");
            }
            //Finally it writes all the lines into the output path
            File.WriteAllLinesAsync(outputFullPath, outputLines);
            Console.WriteLine("Successfully exported data");
            Console.WriteLine("---------------------------");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
