using System;
using System.IO;
using System.Collections.Generic;


namespace QAFrameServerValidator
{
    public static class ProfilesToExclude
    {
        //public static string filePath = @"C:\Users\rimarohx\Desktop\latest\QAFrameServerValidator\QAFrameServerValidator\QAFrameServerValidator\CsvInputs\Exclude.csv";
        //public static string filePath = @"..\..\..\QAFrameServerValidator\CsvInputs\Exclude.csv";
        public static string filePath = "CsvInputs\\Exclude.csv";
        public static FileInfo file = new System.IO.FileInfo(filePath);
        public static Dictionary<string, List<string>> listOfProfilesToExclude = new Dictionary<string, List<string>>();
        public static void GetExcludeFile()
        {
            //System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create();
            System.IO.File.AppendAllText(file.FullName, "Test," + Environment.NewLine);

        }

        public static void AddTestToExcludeFile(string name)
        {
            File.AppendAllText(file.FullName, "-," + name + Environment.NewLine);
        }

        public static void AddProfileToExcludeFile(BasicProfile profile)
        {
            string str = ProfileToString(profile);
            File.AppendAllText(file.FullName, str + Environment.NewLine);
            //System.IO.File.WriteAllText(file.FullName, str + Environment.NewLine);
        }

        public static string ProfileToString(BasicProfile profile)
        {
            string str = "Profile,";
            if (profile.ProfileType == Types.ModeType.Sync)
            {
                str += profile.ProfileTypeAsString + "," + profile.ColorModeAsString + "," + profile.ColorResolutionAsString + "," + profile.ColorFps.ToString() + "," +
                    profile.DepthModeAsString + "," + profile.DepthResolutionAsString + "," + profile.DepthFps.ToString();
            }
            if (profile.ProfileType == Types.ModeType.SyncIR)
            {
                str += profile.ProfileTypeAsString + "," + profile.IRModeAsString + "," + profile.IRResolutionAsString + "," + profile.IRFps.ToString() + "," +
                    profile.DepthModeAsString + "," + profile.DepthResolutionAsString + "," + profile.DepthFps.ToString();
            }
            else if (profile.ProfileType == Types.ModeType.Color)
            {
                str += profile.ProfileTypeAsString + "," + profile.ColorModeAsString + "," + profile.ColorResolutionAsString + "," + profile.ColorFps.ToString();
            }
            else if (profile.ProfileType == Types.ModeType.Depth)
            {
                str += profile.ProfileTypeAsString + "," + profile.DepthModeAsString + "," + profile.DepthResolutionAsString + "," + profile.DepthFps.ToString();
            }
            else if (profile.ProfileType == Types.ModeType.FishEye)
            {
                str += profile.ProfileTypeAsString + "," + profile.FishEyeModeAsString + "," + profile.FishEyeResolutionAsString + "," + profile.FishEyeFps.ToString();
            }
            else if (profile.ProfileType == Types.ModeType.IR)
            {
                str += profile.ProfileTypeAsString + "," + profile.IRModeAsString + "," + profile.IRResolutionAsString + "," + profile.IRFps.ToString();
            }
            if (profile.Controls.Count > 0)
            {
                //str += ",Properties";
                foreach (var con in profile.Controls)
                {
                    str += "," + con.Name + "," + con.Value.ToString();
                }
            }
            Console.WriteLine("Profile full name: " + str);
            return str;
        }

        public static void GetAllExcludedProfiles()
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                String line;
                char[] delim = { ',' };
                string[] values = null;

                while (sr.Peek() >= 0)
                {
                    line = sr.ReadLine().Trim();
                    Console.WriteLine(line);

                    if (line.StartsWith("-"))
                    {
                        values = line.Split(delim);
                        if (!listOfProfilesToExclude.ContainsKey(values[1]))
                        {
                            listOfProfilesToExclude.Add(values[1], new List<string>());
                        }
                    }
                    else if (line.StartsWith("Profile") && values != null)
                    {
                        listOfProfilesToExclude[values[1]].Add(line);
                    }
                }

            }
        }

        public static void RemoveAllExcludedProfilesFromProfileList(List<Profile> profiles, string testName)
        {
            if (listOfProfilesToExclude.ContainsKey(testName))
            {
                Console.WriteLine("Profile list count is: " + profiles.Count);
                int count = 0;
                foreach (BasicProfile bp in profiles.ToArray())
                {
                    //Console.WriteLine("==>" + listOfProfilesToExclude[testName].Contains(ProfileToString(bp)));
                    //Console.WriteLine("==>" + listOfProfilesToExclude[testName].Count);
                    //Console.WriteLine("==>" + ProfileToString(bp));
                    Console.WriteLine("RemoveAllExcludedProfilesFromProfileList - If found : " + listOfProfilesToExclude[testName].Contains(ProfileToString(bp)));
                    if (listOfProfilesToExclude[testName].Contains(ProfileToString(bp)))
                    {
                        Console.WriteLine("**************removing profile");
                        count++;
                        profiles.Remove(bp);
                    }
                }
                Console.WriteLine("Profile list count after remove elements  is : " + profiles.Count + "  ==>" + count);
            }

        }

        public static void CheckIfNewTestRun(int countOfProfiles, string testName)
        {
            try
            {
                List<string> listOfTestProfiles;
                if (listOfProfilesToExclude != null && listOfProfilesToExclude.Count > 0)
                {
                    listOfProfilesToExclude.TryGetValue(testName, out listOfTestProfiles);
                    if (listOfTestProfiles.Count == countOfProfiles) 
                    {
                        listOfProfilesToExclude.Clear();
                        file.Delete();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.AppendException(e.Message);
            }
        }

         

    }
}
