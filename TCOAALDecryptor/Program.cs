using System.Diagnostics;
using Raylib_cs;

class Program {
    public static string userName = Environment.UserName;
    public static string version = "1.0.0";
    public static string branch = "Dev";
    private const string TryAgainMessage = "Press ANY key...";
    public static string[] logo = {
        "",
        "",
        "",
        "  ooooooooooo                                               o888  ",
        "  88  888  88  ooooooo     ooooooo     ooooooo    ooooooo    888  ",
        "      888    888     888 888     888   ooooo888   ooooo888   888  ",
        "      888    888         888     888 888    888 888    888   888  ",
        "     o888o     88ooo888    88ooo88    88ooo88 8o 88ooo88 8o o888o "
    };

    private static CancellationTokenSource _cts = new CancellationTokenSource();
    
    static async Task Main(string[] args) {

        Raylib.InitAudioDevice();

        string musicPath = Path.Combine(AppContext.BaseDirectory, "keygen.xm");
        Music music = Raylib.LoadMusicStream(musicPath);
        Raylib.PlayMusicStream(music);

        Task.Factory.StartNew(() => UpdateMusicBackground(music, _cts.Token), 
        TaskCreationOptions.LongRunning);

        if (OperatingSystem.IsWindows()) {
           try {
                Console.SetWindowSize(90, 40);
                Console.SetBufferSize(90, 40);
            } catch { } 
        }
        
        string GameFolderPath = StartUp();
        await menu(GameFolderPath);
    }

    private static void UpdateMusicBackground(Music music, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Raylib.UpdateMusicStream(music);
            Thread.Sleep(10);
        }
    }

    static string StartUp() {
        Console.Title = $"Welcome, {userName}";

        while (true) {
            Console.Clear();
            foreach (string line in logo) {
                PrintCentered(line);
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            PrintCentered("Please, add path to your TCOAAL folder:");
            PrintCentered("");
            Console.CursorVisible = true;
            int inputStartLeft = (Console.WindowWidth - 73) / 2;
            Console.SetCursorPosition(inputStartLeft < 0 ? 0 : inputStartLeft, Console.CursorTop);

            string? Folder = Console.ReadLine();

            if (string.IsNullOrEmpty(Folder)) {
                Console.WriteLine();
                PrintCentered("Oops, please enter a valid path.");
                PrintCentered(TryAgainMessage);
                Console.ReadKey();
                continue;
            }

            if (!Directory.Exists(Folder)) {
                Console.WriteLine();
                PrintCentered("This directory does not exist.");
                PrintCentered(TryAgainMessage);
                Console.ReadKey();
                continue;
            }

            string GameFolderPath = Path.Combine(Folder, "www");

            if (!Directory.Exists(GameFolderPath)) {
                Console.WriteLine();
                PrintCentered("Please, check you choice a correct path to TCOAAL folder");
                PrintCentered(TryAgainMessage);
                Console.ReadKey();
                continue;
            }

            return GameFolderPath;
        }
    }

    static async Task menu(string GameFolderPath) {
        Console.Clear();
        bool repeat = true;
    
        string[]? files = null;
        bool recoveryNames = false;
        
        Console.Title = $"TCOAAL Decryptor {version} ({branch} build)*";
    
        string[] menuItems = {
            " Decrypt & restore original names ",
            " Decrypt only (Keep encrypted names) ",
            " Abouts ",
            " Quit "
        };
        
        int selectedIndex = 0;
    
        while (repeat) {
            Console.CursorVisible = false;
            Console.Clear();
    
            foreach (string line in logo) {
                PrintCentered(line);
            }
            Console.WriteLine();
    
            PrintCentered($"Welcome to TCOAAL Decryptor {version} ({branch})");
            PrintCentered("By Axell (aka Ar4yk)");
            Console.WriteLine();
            Console.WriteLine();
    
            for (int i = 0; i < menuItems.Length; i++) {
                string textToPrint = menuItems[i];
    
                int spacesCount = (Console.WindowWidth - textToPrint.Length) / 2;
                if (spacesCount < 0) spacesCount = 0;
                string spaces = new string(' ', spacesCount);
    
                if (i == selectedIndex) {
                    Console.Write(spaces); 
    
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine(textToPrint); 
                    Console.ResetColor();
                }
                else {
                    Console.WriteLine(spaces + textToPrint);
                }
            }    
    
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
    
            if (keyInfo.Key == ConsoleKey.UpArrow) {
                selectedIndex--;
                if (selectedIndex < 0) selectedIndex = menuItems.Length - 1;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow) {
                selectedIndex++;
                if (selectedIndex >= menuItems.Length) selectedIndex = 0;
            }
            else if (keyInfo.Key == ConsoleKey.Enter) {
                Console.CursorVisible = true; 
    
                switch (selectedIndex) {
                    case 0:
                        files = FileScanner(GameFolderPath);
                        recoveryNames = true;
                        Decryptor(files, recoveryNames, GameFolderPath);
                    break;
    
                    case 1:
                        files = FileScanner(GameFolderPath);
                        recoveryNames = false;
                        Decryptor(files, recoveryNames, GameFolderPath);
                    break;
    
                    case 2: {
                        Console.CursorVisible = false;
                        Console.Clear();
                    
                        for (int i = 0; i < 90; i++) Console.WriteLine();
                    
                        string[] credits = {
                            $"TCOAAL Decryptor {version} {branch}",
                            "By Axell (aka Ar4yk (Aka Kirieska2007))",
                            "",
                            "------ DISCLAIMER ------",
                            "This tool is for educational purposes only.",
                            "The author is not responsible for any misuse.",
                            "I use Arch, BTW",
                            "",
                            "------ SPECIAL THANKS ------",
                            $"To {userName} - for launching and using this tool :D",
                            "To a certain guy who gave me motivation",
                            "To Dareeo - for reuploading tracker music on YouTube",
                            "To Raylib creators - for an awesome audio library",
                            "To keygenmusic.tk - for the keygen music",
                            "(Special hello to the site owner for that cool RU IP block lol)",
                            "",
                            "------ MENTIONS ------",
                            "Hello to byboba!",
                            "Hello to GILMUTDINOFF!",
                            "",
                        };
                    
                        using var creditsCts = new CancellationTokenSource();
                        bool creditsFinished = false;
                    
                        Task backgroundKeyListen = Task.Run(async () => {
                            while (!creditsCts.Token.IsCancellationRequested && !creditsFinished) {
                                if (Console.KeyAvailable) {
                                    creditsCts.Cancel();
                                    break;
                                }
                                await Task.Delay(50);
                            }
                        });
                    
                        try {
                            foreach (var credit in credits) {
                                creditsCts.Token.ThrowIfCancellationRequested();
                                PrintCentered(credit);
                                await Task.Delay(450, creditsCts.Token);
                            }
                    
                            for (int i = 0; i < Console.WindowHeight; i++) {
                                Console.WriteLine(); 
                                await Task.Delay(450, creditsCts.Token); 
                            }
                    
                            creditsFinished = true;
                            
                            Console.WriteLine();
                            PrintCentered(TryAgainMessage);
                            Console.WriteLine();
                            
                            Console.ReadKey(true); 
                    
                        } catch (Exception) {
                            if (Console.KeyAvailable) Console.ReadKey(true); 
                        }
                    
                        Console.Clear();
                    } break;                    
                    case 3:
                        repeat = false;
                    break;
                }
            }
        }
    }    

    static void PrintCentered(string text) {
        int windowWidth = Console.WindowWidth;
        int spaces = (windowWidth - text.Length) / 2;
        if (spaces < 0) spaces = 0;
        Console.WriteLine(new string(' ', spaces) + text);
    }

    static void PrintCentered(string text, bool newLine = true) {
        int windowWidth = Console.WindowWidth;
        int spaces = (windowWidth - text.Length) / 2;
        if (spaces < 0) spaces = 0;
    
        string output = new string(' ', spaces) + text;
    
        if (newLine) {
            Console.WriteLine(output);
        } else {
            Console.Write(output);
        }
    }

    static string[] FileScanner(string GameFolderPath) {
        Console.Clear();

        foreach (string line in logo) {
                PrintCentered(line);
            }
        Console.WriteLine();
        
        PrintCentered("Scanning...  ", false); 

        string[] files = Directory.GetFiles(GameFolderPath, "*.*", SearchOption.AllDirectories);

        Console.Write("OK");
        Console.WriteLine();

        return(files);
    }
    
    static void Decryptor(string[] files, bool recovery_names, string GameFolderPath) {
        PrintCentered("Starting Decrypting...");
        
        Console.WriteLine();

        string message = "Enter path to import decrypted files:";
        PrintCentered(message);

        Console.CursorVisible = true;
        int inputStartLeft = (Console.WindowWidth - message.Length) / 2;
        if (inputStartLeft < 0) inputStartLeft = 0;
        Console.SetCursorPosition(inputStartLeft, Console.CursorTop);

        string importPath = Console.ReadLine();

        if (string.IsNullOrEmpty(importPath)) {
            Console.WriteLine();
            PrintCentered("Oops, please enter a valid path.");
        }

        Stopwatch stopwatch = Stopwatch.StartNew();
        
        byte[] magicBytes = { 84, 67, 79, 65, 65, 76 };

        foreach (string file in files) {
            try {
                bool isEncrypted = true;
                using (FileStream fs = File.OpenRead(file)) {
                    if (fs.Length < magicBytes.Length) 
                    continue;

                    byte[] header = new byte[magicBytes.Length];
                    fs.Read(header, 0, header.Length);

                    for (int i = 0; i < magicBytes.Length; i++) {
                        if (header[i] != magicBytes[i]) {
                            isEncrypted = false;
                            break;
                        }
                    }
                }
                
                if (isEncrypted) {
                    string fileName = Path.GetFileName(file);
                    string fname = fileName.ToUpper();

                    int mask = 0;

                    foreach (char ch in fname) {
                        mask = (mask << 1) ^ ch;
                    }

                    mask = (mask + 1) % 256;

                    byte[] allBytes = File.ReadAllBytes(file);
                    int keyByte = allBytes[6];
                    int dataLength = allBytes.Length - 7;

                    if (keyByte == 0) {
                        keyByte = dataLength;
                    }

                    byte[] decryptedData = new byte[dataLength];

                    for (int i = 0; i < dataLength; i++) {
                        byte originalByte = allBytes[i + 7];

                        if (i < keyByte) {
                            decryptedData[i] = (byte)(originalByte ^ mask);
                            mask = ((mask << 1) ^ originalByte) & 255;
                        } else {
                            decryptedData[i] = originalByte;
                        }
                    }

                    string mainGamePath = Path.GetDirectoryName(GameFolderPath) ?? GameFolderPath;
                    string relativePath = file.Replace(mainGamePath, "").TrimStart(Path.DirectorySeparatorChar);
                    
                    if (recovery_names == true) {
                        string[] systemFiles = {
                            "data/System.json", "data/Actors.json", "data/Classes.json",
                            "data/Skills.json", "data/Items.json", "data/Weapons.json",
                            "data/Armors.json", "data/Enemies.json", "data/Troops.json",
                            "data/States.json", "data/Animations.json", "data/Tilesets.json",
                            "data/CommonEvents.json", "data/LangData.json", "data/Credits.txt",
                        };
                    
                        foreach (string sysFile in systemFiles) {
                            string sysHashPath = "www/" + HashPath(sysFile);
                            string windowsSysHash = sysHashPath.Replace('/', Path.DirectorySeparatorChar);

                            if (relativePath.Equals(windowsSysHash, StringComparison.OrdinalIgnoreCase)) {
                                relativePath = Path.Combine("www", sysFile.Replace('/', Path.DirectorySeparatorChar));
                                break;
                            }
                        }
                    
                        if (relativePath.StartsWith("www" + Path.DirectorySeparatorChar + "data", StringComparison.OrdinalIgnoreCase)) {
                            for (int i = 1; i <= 999; i++) {
                                string mapLogical = $"data/Map{i:D3}.json";
                                
                                string mapHashPath = "www/" + HashPath(mapLogical);
                                string windowsMapHash = mapHashPath.Replace('/', Path.DirectorySeparatorChar);
                    
                                if (relativePath.Equals(windowsMapHash, StringComparison.OrdinalIgnoreCase)) {

                                    relativePath = Path.Combine("www", mapLogical.Replace('/', Path.DirectorySeparatorChar));
                                    break;
                                }
                            }
                        }
                    }

                    if (!Path.HasExtension(relativePath)) {
                        if (relativePath.Contains("data")) relativePath += ".json";
                        else if (relativePath.Contains("img")) relativePath += ".png";
                        else if (relativePath.Contains("audio")) relativePath += ".ogg";
                    }

                    string finalOutputPath = Path.Combine(importPath, relativePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(finalOutputPath));
                    
                    File.WriteAllBytes(finalOutputPath, decryptedData);
                    
                    Console.WriteLine($"     [Saved] {relativePath}");

                }
                else if (!isEncrypted) {
                    string mainGamePath = Path.GetDirectoryName(GameFolderPath) ?? GameFolderPath;
                    string relativePath = file.Replace(mainGamePath, "").TrimStart(Path.DirectorySeparatorChar);

                    if (relativePath.StartsWith("www" + Path.DirectorySeparatorChar + "data", StringComparison.OrdinalIgnoreCase)) {
                        string worstPath = Path.Combine(importPath, "lost", Path.GetFileName(file));
                        Directory.CreateDirectory(Path.GetDirectoryName(worstPath));
                        File.Copy(file, worstPath, true);
                        Console.WriteLine($"     [Lost] {Path.GetFileName(file)}");
                    }
                    continue;
                }
                
            } catch (Exception ex) { Console.WriteLine($"Error checking file {file}: {ex.Message}"); }
            

        }

        stopwatch.Stop();
        double totalSeconds = stopwatch.Elapsed.TotalSeconds;

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        PrintCentered($"Done! ({totalSeconds:F2}s).");
        Console.WriteLine();
        Console.WriteLine();

        PrintCentered(TryAgainMessage);
        Console.ReadKey();
    }

    static string HashPath(string logicalPath) {
        string normalizedPath = logicalPath.Replace('\\', '/');
        string[] parts = normalizedPath.Split('/');
        string fname = parts[parts.Length - 1];
        byte[] encoded = System.Text.Encoding.UTF8.GetBytes(normalizedPath);

        using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create()) {
            byte[] hashBuf = sha256.ComputeHash(encoded);
            System.Text.StringBuilder hex = new System.Text.StringBuilder(hashBuf.Length * 2);
            foreach (byte b in hashBuf) {
                hex.Append(b.ToString("x2"));
            }
            string h = hex.ToString().Substring(0, 16);
            if (fname.ToUpper().Contains("[BUST]")) h += "[BUST]";
            if (fname.StartsWith("!")) h = "!" + h;
            parts[parts.Length - 1] = h;
        }
        return string.Join("/", parts);
    }
}
