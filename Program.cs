    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    namespace BackupSoftware
    {
        class Program
        {

            public static void Main(string[] args)
            {
                string langue = "EN";
                BackupJob[] jobs = new BackupJob[5];
                int jobCount = 0;
                
                

                while (true)
                {
                 ///On définit le type JSON par défault pour les fichiers LOG et State
                 string extension = "json";

                ///Affichage du menu.txt pour le Choix Fonctionnalitées
                string filePath = @"menu" + langue + ".txt";
                StreamReader readerfr = new StreamReader(filePath);
                string fileContent = readerfr.ReadToEnd();
                Console.WriteLine(fileContent);


                 int choice = int.Parse(Console.ReadLine());
                    
                    
                    

                    switch (choice)
                    {
                    ///Maximum de 5 save
                        case 1:
                            if (jobCount == 5)
                            {
                                Console.WriteLine("Maximum number of jobs reached.");
                                break;
                            }

                            
                            ///definition de tout nos parametres pour la save
                            BackupJob job = new BackupJob();
                            Console.WriteLine(File.ReadLines(@"save"+langue+".txt").Skip(0).Take(1).First());
                            job.Name = Console.ReadLine();
                            
                            Console.WriteLine(File.ReadLines(@"save" + langue + ".txt").Skip(1).Take(1).First());
                            job.SourceDirectory = Console.ReadLine();
                            
                            Console.WriteLine(File.ReadLines(@"save"+langue+".txt").Skip(2).Take(1).First());
                            job.TargetDirectory = Console.ReadLine();
                            
                            Console.WriteLine(File.ReadLines(@"save"+langue+".txt").Skip(3).Take(1).First());
                            job.Type = Console.ReadLine();
                            
                            jobs[jobCount++] = job;

                            Console.WriteLine(File.ReadLines(@"save" + langue + ".txt").Skip(4).Take(1).First());
                            job.size = GetDirectorySize(job.SourceDirectory);
                            
                            job.state = "not active";

                            job.fileCount = Directory.GetFiles(job.SourceDirectory).Length;
                            job.filetodo = (Directory.GetFiles(job.SourceDirectory).Length) - (Directory.GetFiles(job.TargetDirectory).Length);
                            job.sizetodo = GetDirectorySize(job.SourceDirectory) - GetDirectorySize(job.TargetDirectory);
                        
                        ///Création de nos log file et state file
                            string logFile = GetLogFilePath(extension);
                            string stateFile = GetStateFilePath(extension);
                            BackupLOG_STATE(logFile,stateFile,job);
                        break;

                        case 2:
                        ///selection de la save pour l'executer par un nom
                            Console.WriteLine(File.ReadLines(@"save" + langue + ".txt").Skip(5).Take(1).First());
                            string jobName = Console.ReadLine();
                            BackupJob selectedJob = null;
                            for (int i = 0; i < jobCount; i++)
                            {
                                if (jobs[i].Name == jobName)
                                {
                                    selectedJob = jobs[i];
                                    break;
                                }
                            }

                            if (selectedJob == null)
                            {
                                Console.WriteLine(File.ReadLines(@"save" + langue + ".txt").Skip(6).Take(1).First());
                                break;
                            }
                            Console.WriteLine((File.ReadLines(@"save" + langue + ".txt").Skip(7).Take(1).First())+ selectedJob.Name);
                            RunBackupJob(selectedJob,langue);
                            string logFile1 = GetLogFilePath(extension);
                            string stateFile1 = GetStateFilePath(extension);
                            BackupLOG_STATE(logFile1, stateFile1, selectedJob);

                        break;
                        case 3:
                        ///execution de toute les saves
                            Console.WriteLine(File.ReadLines(@"save" + langue + ".txt").Skip(8).Take(1).First());
                            for (int i = 0; i < jobCount; i++)
                            {
                                RunBackupJob(jobs[i],langue);
                                string logFile2 = GetLogFilePath(extension);
                                string stateFile2 = GetStateFilePath(extension);
                                BackupLOG_STATE(logFile2, stateFile2, jobs[i]);


                        }

                        break;

                        case 4:
                        ///affichage de toute les saves
                            Console.WriteLine(File.ReadLines(@"save" + langue + ".txt").Skip(9).Take(1).First());

                            for (int i = 0; i < jobCount; i++)
                            {
                                Console.WriteLine((File.ReadLines(@"save" + langue + ".txt").Skip(10).Take(1).First())+ jobs[i].Name);
                                Console.WriteLine((File.ReadLines(@"save" + langue + ".txt").Skip(11).Take(1).First()) + jobs[i].SourceDirectory);
                                Console.WriteLine((File.ReadLines(@"save" + langue + ".txt").Skip(12).Take(1).First()) + jobs[i].TargetDirectory);
                                Console.WriteLine((File.ReadLines(@"save" + langue + ".txt").Skip(13).Take(1).First()) + jobs[i].Type);
                                Console.WriteLine((File.ReadLines(@"save" + langue + ".txt").Skip(14).Take(1).First()) + jobs[i].state);
                                Console.WriteLine();
                            }
                            break;
                   
                    case 5:
                        ///choix du type JSON ou XML
                        Console.WriteLine(File.ReadLines(@"save" + langue + ".txt").Skip(15).Take(1).First());
                        extension = Console.ReadLine();
                        break;

                    case 6 :
                        ///choix de la langue
                        langue = "FR";
                        break;

                    case 7:
                            Environment.Exit(0);
                            break;





                    default:
                            Console.WriteLine(File.ReadLines(@"save" + langue + ".txt").Skip(16).Take(1).First());
                            break;
                        
                        

                    }
               
                }

            }

            public static void BackupLOG_STATE(string logFile,string stateFile,BackupJob job)
            {
                
                // Information minimales attendues
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                int transferTime = -1;

                try
                {
                    // Mesurer le temps de transfert
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    stopwatch.Stop();
                    transferTime = (int)stopwatch.ElapsedMilliseconds;
                }
                catch (Exception ex)
                {
                    transferTime = -1;
                    Console.WriteLine("Erreur lors de la sauvegarde : " + ex.Message);
                }

                // Écrire les informations dans le fichier log
                using (StreamWriter writer = new StreamWriter(logFile, true))
                {
                    writer.WriteLine("{0},{1},{2},{3},{4},{5}",timestamp, "Name :"+ job.Name, "Source Path :" + job.SourceDirectory , "Destination Path :" + job.TargetDirectory ,"Size :"+ job.size +"octet(s)", "Transfer Time :" + transferTime);
                }

                

                using (StreamWriter writer = new StreamWriter(stateFile, true))
                {
                    writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7}", timestamp, "Name :" + job.Name, "Source Path :" + job.SourceDirectory, "Destination Path :" + job.TargetDirectory, "NumberofFiles :" + job.fileCount,"TotalFileSize :"+ job.size, "FileToDo :" + job.filetodo,"SizeOfFileLeftToDo :"+ job.sizetodo);
                }
        }


            public static string GetLogFilePath(string extension)
            {
                string logDirectory = Path.Combine(Environment.CurrentDirectory, "logs");
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                string logFileName = DateTime.Now.ToString("yyyy-MM-dd") + "."+extension;
                return Path.Combine(logDirectory, logFileName);
            }

        public static string GetStateFilePath(string extension)
        {
            string stateDirectory = Path.Combine(Environment.CurrentDirectory, "state");
            if (!Directory.Exists(stateDirectory))
            {
                Directory.CreateDirectory(stateDirectory);
            }

            string stateFileName = DateTime.Now.ToString("yyyy-MM-dd") + "."+extension;
            return Path.Combine(stateDirectory, stateFileName);
        }




        private static void RunBackupJob(BackupJob job,string langue)
            {
                Console.WriteLine((File.ReadLines(@"save" + langue + ".txt").Skip(10).Take(1).First())+ job.Name);
                Console.WriteLine((File.ReadLines(@"save" + langue + ".txt").Skip(11).Take(1).First()) + job.SourceDirectory);
                Console.WriteLine((File.ReadLines(@"save" + langue + ".txt").Skip(12).Take(1).First()) + job.TargetDirectory);
                job.state = "active";

                

                // Code to run the backup job
                try
                {
                    DirectoryInfo source = new DirectoryInfo(job.SourceDirectory);
                    DirectoryInfo destination = new DirectoryInfo(job.TargetDirectory);

                    // Check if the source directory exists
                    if (!source.Exists)
                    {
                        Console.WriteLine((File.ReadLines(@"save" + langue + ".txt").Skip(17).Take(1).First()) + job.SourceDirectory);
                        return;
                    }

                    // Check if the destination directory exists
                    if (!destination.Exists)
                    {
                        Console.WriteLine((File.ReadLines(@"save" + langue + ".txt").Skip(18).Take(1).First()) + job.TargetDirectory);
                        return;
                    }

                    // Get the files to backup
                    FileInfo[] files = source.GetFiles();

                    // Log the number of files eligible for backup
                    Console.WriteLine((File.ReadLines(@"save" + langue + ".txt").Skip(19).Take(1).First()) + files.Length);

                    // Backup each file
                    foreach (FileInfo file in files)
                    {
                        // Log the start of the backup
                        Console.WriteLine((File.ReadLines(@"save" + langue + ".txt").Skip(20).Take(1).First()) + file.FullName);

                        // Calculate the backup start time
                        Stopwatch stopwatch = Stopwatch.StartNew();

                        // Copy the file to the destination directory
                        file.CopyTo(Path.Combine(destination.FullName, file.Name), true);

                        // Log the end of the backup
                        stopwatch.Stop();
                        Console.WriteLine((File.ReadLines(@"save" + langue + ".txt").Skip(21).Take(1).First()) + stopwatch.ElapsedMilliseconds + "ms");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine((File.ReadLines(@"save" + langue + ".txt").Skip(22).Take(1).First()) + ex.Message);
                }
            }

        public static long GetDirectorySize(string folderPath)
        {
            long size = 0;
            try
            {
                DirectoryInfo di = new DirectoryInfo(folderPath);
                foreach (FileInfo fi in di.GetFiles())
                {
                    size += fi.Length;
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    size += GetDirectorySize(dir.FullName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return size;
        }

    }
    }

        class BackupJob
        {
            public string Name { get; set; }
            public string SourceDirectory { get; set; }
            public string TargetDirectory { get; set; }
            public string Type { get; set; }
            public long size { get; set; }
            public string state { get; set; }
            public int filetodo { get; set; }
            public long sizetodo { get; set; }
            public int fileCount { get; set; }
}

