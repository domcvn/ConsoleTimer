using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;

struct Config
{
    internal uint numberOfTimers;
    internal uint numberOfIntervals;
    internal uint[] timers;
    internal string[] startIntervalMessage;
    internal string[] endIntervalMessage;
    internal string[] timerNames;
}
class Program
{
    static void Help()
    {
        Console.WriteLine("ConsoleTimer by domcvn\n");
        Console.WriteLine("Press \"1\" to open the helpbook, for FAQ\n");
        Console.WriteLine("If you have questions that are not in the helpbook or you found bugs, press \"2\" to go to my GitHub repository. \nFeel free to submit an issue with bug or question label!");
        Console.WriteLine("\nPress \"3\" to go back to menu");
        switch(Console.ReadKey().Key)
        {
            case ConsoleKey.D1: 
            case ConsoleKey.NumPad1: break;
            case ConsoleKey.D2:
            case ConsoleKey.NumPad2: Process.Start("cmd", "/c start https://github.com/domcvn/consoletimer/issues"); Console.Clear(); Console.WriteLine("Opened the web! Press any key to return to main menu"); Console.ReadKey(); Console.Clear(); Main(); Main(); break;
            case ConsoleKey.D3:
            case ConsoleKey.NumPad3: Console.Clear(); Main(); break;
        }
    }
    static void About()
    {

    }
    static void Main()
    {
        try
        {
            if (Directory.Exists("importconfig")) Directory.Delete("importconfig", true);
            Console.WriteLine("ConsoleTimer by domcvn\n");
            Console.WriteLine("1. Start\n2. Help\n3. About\n4. Exit");
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.D1: 
                case ConsoleKey.NumPad1: Console.Clear(); ImportConfig(); break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2: Console.Clear(); Help(); break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3: break;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4: Environment.Exit(0); break;
                default: throw new Exception();
            }
        }
        catch (Exception)
        {
            Console.Clear();
            Console.WriteLine("Invalid command");
            Main();
        }
    }
    static void ImportConfig()
    {
        Config c = new Config();
        char isMessagesPerInterval;
        char nameTimer;
        string configPath;
        string[] allarg;
        string[] arg;
        int countFiles;
        ConsoleKey input;
        DirectoryInfo info;

        try
        {
            Console.WriteLine("ConsoleTimer by domcvn\n");
            //Options for User
            Console.WriteLine("1. Start new Timer\n2. Import timer configuration\n3. Go back");
            input = Console.ReadKey().Key;
            //import timer configuration
            if (input == ConsoleKey.D2 || input == ConsoleKey.NumPad2)
            {
                //initialize folder for copying config file
                Console.Clear();
                Console.WriteLine("ConsoleTimer by domcvn\n");
                if (!Directory.Exists(@"importconfig")) Directory.CreateDirectory(@"importconfig");
                Process.Start("explorer.exe", @"importconfig");
                Console.Write("Please copy your configuration file into this folder, press any key when you finished.");
                Console.ReadKey();
                //counting how many files in the folder, if == 1 then cont, else exception
                info = new DirectoryInfo(@"importconfig");
                countFiles = info.GetFiles("*.conf", SearchOption.AllDirectories).Length;
                if (countFiles != 1) throw new Exception();
                else
                {
                    //get file name
                    string[] files = Directory.GetFiles(@"importconfig");
                    configPath = files[0];
                    //read all lines in the file
                    allarg = File.ReadAllLines(configPath);
                    if (allarg[0] != "consoletimerconfig") throw new Exception();
                    else
                    {
                        //check the number of intervals
                        arg = allarg[1].Split('-');
                        if (arg[0] != "numberOfIntervals" || Convert.ToInt32(arg[1]) < 0) throw new Exception();
                        else
                        {
                            c.numberOfIntervals = Convert.ToUInt32(arg[1]);
                            c.startIntervalMessage = new string[c.numberOfIntervals];
                            c.endIntervalMessage = new string[c.numberOfIntervals];
                        }

                        //check the number of timers
                        arg = allarg[2].Split('-');
                        if (arg[0] != "numberOfTimers" || Convert.ToInt32(arg[1]) < 0) throw new Exception();
                        else
                        {
                            c.numberOfTimers = Convert.ToUInt32(arg[1]);
                            c.timers = new uint[c.numberOfTimers];
                            c.timerNames = new string[c.numberOfTimers];
                        }

                        //check the timer section
                        for (uint i = 0; i < c.numberOfTimers; i++)
                        {
                            arg = allarg[i + 3].Split('-');
                            if (arg[0] != "timer" || arg[1] != Convert.ToString(i + 1) || Convert.ToInt32(arg[3]) < 0) throw new Exception();
                            else
                            {
                                c.timers[i] = Convert.ToUInt32(arg[3]);
                                c.timerNames[i] = arg[2];
                            }
                        }

                        //check the interval section
                        for (uint i = 0; i < c.numberOfIntervals; i++)
                        {
                            arg = allarg[i + 3 + c.numberOfTimers].Split('-');
                            if (arg[0] != "interval" || arg[1] != Convert.ToString(i + 1)) throw new Exception();
                            else
                            {
                                c.startIntervalMessage[i] = arg[2];
                                c.endIntervalMessage[i] = arg[3];
                            }
                        }

                        //Delete the importconfig folder
                        Directory.Delete("importconfig", true);
                    }
                }
            }
            //user manually input
            else if (input == ConsoleKey.D1 || input == ConsoleKey.NumPad1)
            {
                Console.Clear();
                Console.WriteLine("ConsoleTimer by domcvn\n");
                //how many intervals
                Console.Write("How many intervals do you want?: ");
                c.numberOfIntervals = uint.Parse(Console.ReadLine());

                //timers per interval
                Console.Write("How many timers per interval do you want?: ");
                c.numberOfTimers = uint.Parse(Console.ReadLine());
                c.timers = new uint[c.numberOfTimers];

                //duration for each timer
                Console.WriteLine("Enter duration for each timer (in seconds): ");
                for (int i = 0; i < c.numberOfTimers; i++)
                {
                    Console.Write($"Timer #{i + 1}: ");
                    c.timers[i] = uint.Parse(Console.ReadLine());
                }

                //naming the timer
                Console.Write("Do you want to name the timers? (y/n): ");
                nameTimer = char.Parse(Console.ReadLine().ToLower());
                c.timerNames = new string[c.numberOfTimers];
                switch (nameTimer)
                {
                    case 'y':
                        Console.WriteLine("Your timers name must not contain a dash (\"-\")!");
                        for (int i = 0; i < c.numberOfTimers; i++)
                        {
                            Console.Write($"Timer #{i + 1}: ");
                            c.timerNames[i] = Console.ReadLine().Trim();
                            //check if timer name contains a dash
                            for (int j = 0; j < c.timerNames[i].Length; j++)
                            {
                                if (c.timerNames[i][j] == '-') throw new Exception();
                            }
                            //set default name if userinput is blank
                            if (c.timerNames[i] == "") c.timerNames[i] = $"Timer #{i + 1}";
                        }
                        break;
                    case 'n':
                        for (int i = 0; i < c.numberOfTimers; i++) c.timerNames[i] = $"Timer #{i + 1}";
                        break;
                    default: throw new Exception();
                }

                //messages after interval ends
                Console.Write("Do you want to customise start/end messages for each interval? (y/n): ");
                isMessagesPerInterval = char.Parse(Console.ReadLine().ToLower());
                c.startIntervalMessage = new string[c.numberOfIntervals];
                c.endIntervalMessage = new string[c.numberOfIntervals];
                switch (isMessagesPerInterval)
                {
                    case 'y':
                        Console.WriteLine("Your message must not contain a dash (\"-\")!");
                        for (int i = 0; i < c.numberOfIntervals; i++)
                        {
                            Console.Write($"Start Interval #{i + 1}: ");
                            c.startIntervalMessage[i] = Console.ReadLine().Trim();
                            //check message if contains a dash
                            for (int j = 0; j < c.startIntervalMessage[i].Length; j++)
                            {
                                if (c.startIntervalMessage[i][j] == '-') throw new Exception();
                            }
                            //set default message name if userinput is blank
                            if (c.startIntervalMessage[i] == "") c.startIntervalMessage[i] = $"Start of Interval #{i + 1}";
                            Console.Write($"End Interval #{i + 1}: ");
                            c.endIntervalMessage[i] = Console.ReadLine().Trim();
                            //check message if contains a dash
                            for (int j = 0; j < c.endIntervalMessage[i].Length; j++)
                            {
                                if (c.endIntervalMessage[i][j] == '-') throw new Exception();
                            }
                            //set default message name if userinput is blank
                            if (c.endIntervalMessage[i] == "") c.endIntervalMessage[i] = $"End of Interval #{i + 1}";
                        }
                        break;
                    case 'n':
                        for (int i = 0; i < c.numberOfIntervals; i++)
                        {
                            c.startIntervalMessage[i] = $"Start of Interval #{i + 1}";
                            c.endIntervalMessage[i] = $"End of Interval #{i + 1}";
                        }
                        break;
                    default: throw new Exception();
                }
            }
            else if (input == ConsoleKey.D3 || input == ConsoleKey.NumPad3)
            {
                Console.Clear();
                Main();
            }
            else throw new Exception();
            ConfigConfirm(c);
        }
        catch (Exception)
        {
            Console.Clear();
            Console.WriteLine("Invalid command");
            Main();
        }
    }
    static void ConfigConfirm(Config c)
    {
        ConsoleKey input;
        Console.Clear();
        Console.WriteLine("ConsoleTimer by domcvn\n");
        Console.WriteLine("Here are your settings:");
        Console.WriteLine($"Intervals count: {c.numberOfIntervals}");
        Console.WriteLine($"Timers per Interval count: {c.numberOfTimers}");
        Console.WriteLine("Intervals Configuration:");
        for (int i = 0; i < c.numberOfIntervals; i++) Console.WriteLine($"   Interval #{i + 1}:\n      Start message: {c.startIntervalMessage[i]}\n      End message: {c.endIntervalMessage[i]}");
        Console.WriteLine("Timers Configuration:");
        for (int i = 0; i < c.numberOfTimers; i++) Console.WriteLine($"   Timer #{i + 1}:\n      Name: {c.timerNames[i]}\n      Duration: {c.timers[i]} second(s)");
        Console.WriteLine("\nPress:\n   Enter to start your timer\n   \"E\" to export your configuration and start\n   Any other key to return to menu");
        input = Console.ReadKey().Key;
        if (input == ConsoleKey.Enter) Timer(c);
        else if (input == ConsoleKey.E) ExportTimerConfig(c);
        else
        {
            Console.Clear();
            ImportConfig();
        }
    }
    static void Timer(Config c)
    {
        //initialize spaces
        string spaces = "";
        int maxlength = 0;
        int maxtimerlength = 0;
        for (int i = 0; i < c.startIntervalMessage.Length; i++)
        {
            if (maxlength < c.startIntervalMessage[i].Length) maxlength = c.startIntervalMessage[i].Length;
            if (maxlength < c.endIntervalMessage[i].Length) maxlength = c.endIntervalMessage[i].Length;
        }
        for (int i = 0; i < c.timers.Length; i++)
        {
            if (Convert.ToString(c.timers[i]).Length > maxtimerlength) maxtimerlength = Convert.ToString(c.timers[i]).Length;
        }
        maxlength += maxtimerlength;
        for (int i = 0; i < maxlength + 15; i++) spaces += " ";
        Console.Clear();
        Console.WriteLine("ConsoleTimer by domcvn\n");

        //intervals
        for (uint i = 0; i < c.numberOfIntervals; i++)
        {
            //timers per interval
            Console.WriteLine(c.startIntervalMessage[i]);
            for (uint j = 0; j < c.numberOfTimers; j++)
            {
                for (uint m = 0; m <= c.timers[j]; m++)
                {
                    Console.Write($"\r{c.timerNames[j]}: {m} {spaces}");
                    Thread.Sleep(1000);
                }
                //beep sound when timer end
                Console.Beep();
            }
            //beep sound when interval end => 2 beeps at the end
            Console.Beep();
            Console.WriteLine("\r" + c.endIntervalMessage[i] + spaces);
        }
        Console.Write("Your timer ended! Press any key to return to main menu!");
        Console.ReadKey();
        Console.Clear();
        Main();
    }
    static void ExportTimerConfig(Config c)
    {
        try
        {
            Console.Clear();
            Console.WriteLine("ConsoleTimer by domcvn\n");
            //ask user to enter file name
            Console.Write("Enter file name: ");
            string filename = Console.ReadLine();
            //if filename empty implement default name
            if (filename == "") filename = "exported";
            StringBuilder sb = new StringBuilder();
            //first 3 lines: header, interval count, timer count
            sb.AppendLine("consoletimerconfig");
            sb.AppendLine($"numberOfIntervals-{c.numberOfIntervals}");
            sb.AppendLine($"numberOfTimers-{c.numberOfTimers}");
            //timers config
            for (uint i = 0; i < c.numberOfTimers; i++) sb.AppendLine($"timer-{i + 1}-{c.timerNames[i]}-{c.timers[i]}");
            //intervals config
            for (uint i = 0; i < c.numberOfIntervals; i++) sb.AppendLine($"interval-{i + 1}-{c.startIntervalMessage[i]}-{c.endIntervalMessage[i]}");
            //add to file
            if (!Directory.Exists(@"Exported")) Directory.CreateDirectory(@"Exported");
            File.Create(@$"Exported\{filename}.conf").Close();
            File.AppendAllText($@"Exported\{filename}.conf", sb.ToString());
            Console.WriteLine("Exported complete! Press any key to start the timer");
            Process.Start("explorer.exe", "Exported");
            Console.ReadKey();
            Console.Clear();
            Timer(c);
        }
        catch (Exception)
        {
            Console.Clear();
            Console.WriteLine("Invalid command");
            Main();
        }
    }
}