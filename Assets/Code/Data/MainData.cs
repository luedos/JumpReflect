using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DataManipulators
{
    public sealed class MainData : MonoBehaviour
    {


        class LevelData
        {
            public int Score;
            public string LevelName;
            public string LevelHash;
        }

        class PlayerData
        {
            public string Name;
            public List<LevelData> LevelData;

            public LevelData MaxScoreLevel = null;

            // change the way player data trate levels

            private PlayerData() { }

            public PlayerData(string inName)
            {
                Name = inName;
                LevelData = new List<LevelData>();
            }

            public void ChangeScore(string inLevel, int inScore, string LevelHash, bool ChangeIfSmoller = false)
            {
                if (inScore < 0)
                    return;

                for (int i = 0; i < LevelData.Count; ++i)
                    if (LevelData[i].LevelName == inLevel && LevelData[i].LevelHash == LevelHash)
                    {
                        if (LevelData[i].Score < inScore || ChangeIfSmoller)
                        {
                            LevelData[i].Score = inScore;

                            if (MaxScoreLevel.Score < inScore && MaxScoreLevel.LevelName != LevelData[i].LevelName)
                                MaxScoreLevel = LevelData[i];
                        }
                        return;
                    }

                LevelData local = new LevelData();

                local.LevelName = inLevel;
                local.Score = inScore;
                local.LevelHash = LevelHash;

                if (MaxScoreLevel == null || MaxScoreLevel.Score < inScore)
                    MaxScoreLevel = local;

                LevelData.Add(local);
            }

            public int GetScore(string inLevel, string inLevelHash)
            {
                for (int i = 0; i < LevelData.Count; ++i)
                    if (LevelData[i].LevelName == inLevel && LevelData[i].LevelHash == inLevelHash)
                        return LevelData[i].Score;

                return 0;
            }

        }

        public struct LevelProps
        {
            public Sprite LevelImage;
            public string LevelName;
            public string LevelPath;
            public string LevelHash;

            public bool inGameLevel;
        }

        public Dictionary<int, LevelProps> myLevelData = new Dictionary<int, LevelProps>();
        int currentLevel = -1;

        public void LoadLevel(int LevelIndex)
        {
            if (!myLevelData.ContainsKey(LevelIndex))
            {
                InfoMessage.ShowError("Level not found");
                return;
            }

            currentLevel = LevelIndex;

            LevelProps localLP = myLevelData[LevelIndex];

            if (localLP.inGameLevel)
                SceneManager.LoadScene(localLP.LevelPath);
            else
                LevelLoader.LoadLevel(localLP.LevelPath);
        }

        public LevelProps GetCurrentLevel()
        {
            if (currentLevel == -1 || !myLevelData.ContainsKey(currentLevel) || SceneManager.GetActiveScene().name == "Level_MainMenu")
            {
                LevelProps localLD = new LevelProps();
                localLD.LevelName = "Main menu";
                return localLD;
            }

            return myLevelData[currentLevel];

        }

        List<PlayerData> PlayerList = new List<PlayerData>();

        PlayerData MainPlayer;

        private static MainData instance;

        public static MainData GetInstance()
        {
            if (instance != null)
                return instance;

            instance = new GameObject().AddComponent<MainData>();
            DontDestroyOnLoad(instance);
            instance.ReadPlayerData();
            instance.ReadLevelData();

            return instance;
        }

        public void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
                ReadPlayerData();
                ReadLevelData();
                return;
            }

            if (instance == this)
                return;

            Destroy(this);

        }

        private void Update()
        {
            if (Input.GetButtonDown("LoadSaves") && Input.GetKey(KeyCode.LeftControl))
            {
                MainMenues.MainMenu myMM = FindObjectOfType<MainMenues.MainMenu>();

                if (!myMM)
                    return;

                if (ReadPlayerData())
                    InfoMessage.ShowMessage("Saves readed successfully");
                else
                    InfoMessage.ShowError("Fail in reading saves");

                myMM.UpdateHUD();
            }

            if (Input.GetButtonDown("LoadLevels") && Input.GetKey(KeyCode.LeftControl))
            {
                MainMenues.MainMenu myMM = FindObjectOfType<MainMenues.MainMenu>();

                if (!myMM)
                    return;

                ReadLevelData();

                InfoMessage.ShowMessage("Levels loaded");

                myMM.UpdateHUD();
            }

            if (Input.GetButtonDown("WriteSaves") && Input.GetKey(KeyCode.LeftControl))
            {

                InfoMessage.ShowMessage("Writing saves");
                WritePlayerData();
            }


        }

        public bool AddPlayer(string inName)
        {
            for (int i = 0; i < PlayerList.Count; ++i)
                if (PlayerList[i].Name == inName)
                    return false;

            PlayerData localPlayer = new PlayerData(inName);

            PlayerList.Add(localPlayer);
            //if (PlayerList.Count == 1)
            //    MainPlayer = PlayerList[0];

            MainPlayer = localPlayer;

            return true;
        }

        private void AddPlayer(PlayerData inPD)
        {
            if (MainPlayer == null)
                MainPlayer = inPD;

            for (int i = 0; i < PlayerList.Count; ++i)
                if (PlayerList[i].Name == inPD.Name)
                {
                    PlayerList.RemoveAt(i);

                }

            if (inPD.MaxScoreLevel != null)
                for (int i = 0; i < PlayerList.Count; ++i)
                    if (PlayerList[i].MaxScoreLevel == null)
                        break;
                    else if (inPD.MaxScoreLevel.Score > PlayerList[i].MaxScoreLevel.Score)
                    {
                        PlayerList.Insert(i, inPD);
                        return;
                    }

            PlayerList.Add(inPD);
        }

        public bool DeletePlayer(string inName)
        {
            for (int i = 0; i < PlayerList.Count; ++i)
                if (PlayerList[i].Name == inName)
                {
                    if (PlayerList[i].Name == MainPlayer.Name)
                        MainPlayer = null;

                    PlayerList.RemoveAt(i);

                    return true;
                }

            return false;
        }

        public string GetMainPlayer()
        {
            if (MainPlayer == null)
                return null;

            return MainPlayer.Name;
        }

        public string[] GetPlayers(bool Sorted = true)
        {


            string[] returnStrings = new string[PlayerList.Count];

            for (int i = 0; i < PlayerList.Count; ++i)
                returnStrings[i] = PlayerList[i].Name;

            return returnStrings;
        }

        public bool ChangeMainPlayer(string inName, bool AddIfNotFind = false)
        {
            for (int i = 0; i < PlayerList.Count; ++i)
            {
                if (PlayerList[i].Name == inName)
                {
                    MainPlayer = PlayerList[i];
                    return true;
                }
            }

            if (AddIfNotFind)
            {
                AddPlayer(inName);

                MainPlayer = PlayerList[PlayerList.Count - 1];

                return true;
            }

            return false;
        }

        public bool isPlayerExist(string inName)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList[i].Name == inName)
                    return true;
            }

            return false;
        }

        public bool ReadPlayerData()
        {

            if (!System.IO.File.Exists(@"Saves\Save.jrs"))
                return false;

            string[] MainLines = System.IO.File.ReadAllLines(@"Saves\Save.jrs");

            int localI;
            int totalI;

            if (MainLines.Length < 1)
                return false;

            CountSystem Line = new CountSystem(26);

            Line.SetValue(MainLines[0], E_Language.English, true);

            Line.MyOrder = 256;

            string lineString = Line.GetString(E_Language.UTF8, false);

            string[] stringArr = lineString.Split('.');

            if (stringArr.Length != 3 || stringArr[0] != "ИгроваяИнформация")
                return false;

            Line.SetValue(stringArr[2], E_Language.Chars);

            int LengthTest = 0;

            for (int i = 1; i < MainLines.Length; ++i)
                LengthTest += MainLines[i].Length;

            if (LengthTest != Line.GetHashCode())
                return false;

            if (!int.TryParse(stringArr[1], out totalI))
                return false;

            Dictionary<int, string> localHash = new Dictionary<int, string>(totalI);

            for (int i = 1; i < totalI + 1; ++i)
            {
                Line.SetValue(0);
                Line.MyOrder = 26;

                Line.SetValue(MainLines[i], E_Language.English, true);

                Line.MyOrder = 256;

                stringArr = Line.GetString(E_Language.UTF8).Split('|');

                if (stringArr.Length != 2 || !int.TryParse(stringArr[0], out localI))
                    continue;

                localHash.Add(localI, stringArr[1]);

            }

            PlayerList.Clear();

            for (int ip = totalI + 1; ip < MainLines.Length; ++ip)
            {
                Line.SetValue(0);
                Line.MyOrder = 26;
                Line.SetValue(MainLines[ip], E_Language.English, true);

                Line.MyOrder = 256;

                lineString = Line.GetString(E_Language.UTF8, false);

                stringArr = lineString.Split('|');

                if (stringArr.Length < 2)
                    continue;

                string[] localArr = stringArr[stringArr.Length - 1].Split('.');

                if (localArr.Length != 2 || localArr[0] != "PD")
                    continue;

                Line.SetValue(localArr[1], E_Language.Chars);

                int localTest = 0;

                for (int i = 0; i < stringArr.Length - 1; ++i)
                    localTest += stringArr[i].Length;

                if (Line.GetHashCode() != localTest)
                    continue;

                PlayerData linePD = new PlayerData(stringArr[0]);

                for (int il = 1; il < stringArr.Length - 1; ++il)
                {
                    localArr = stringArr[il].Split('.');

                    if (localArr.Length != 5 || localArr[3] != "LD")
                        break;

                    Line.SetValue(localArr[4], E_Language.Chars);

                    if (Line.GetHashCode() != localArr[0].Length + localArr[1].Length)
                        break;


                    Line.SetValue(localArr[1], E_Language.Chars);

                    int intScore = Line.GetHashCode();

                    if (intScore < 0)
                        break;


                    if (!int.TryParse(localArr[2], out localI))
                        continue;

                    if (!localHash.ContainsKey(localI))
                        continue;

                    linePD.ChangeScore(localArr[0], intScore, localHash[localI]);
                }

                if (stringArr.Length - 2 == linePD.LevelData.Count)
                    AddPlayer(linePD);
            }

            if (MainLines.Length > 1 && PlayerList.Count == 0)
                return false;

            return true;
        }

        public void WritePlayerData()
        {
            string[] PlayerLines = new String[PlayerList.Count];
            Dictionary<string, int> localHash = new Dictionary<string, int>();
            List<string> localKeys = new List<string>();

            List<string> LineList = new List<string>();

            CountSystem tempCS = new CountSystem(256);

            string tempString;

            for (int ip = 0; ip < PlayerList.Count; ++ip)
            {
                tempCS.SetValue(0);
                tempCS.MyOrder = 256;

                LineList.Clear();
                LineList.Add(PlayerList[ip].Name);

                int totalSum = 0;

                for (int il = 0; il < PlayerList[ip].LevelData.Count; ++il)
                {
                    tempCS.SetValue(PlayerList[ip].LevelData[il].Score);

                    tempString = tempCS.GetString(E_Language.Chars);

                    tempCS.SetValue(PlayerList[ip].LevelData[il].LevelName.Length + tempString.Length);

                    if (!localHash.ContainsKey(PlayerList[ip].LevelData[il].LevelHash))
                    {
                        localHash.Add(PlayerList[ip].LevelData[il].LevelHash, localHash.Count);
                        localKeys.Add(PlayerList[ip].LevelData[il].LevelHash);
                    }
                    LineList.Add(String.Join(".", new string[] { PlayerList[ip].LevelData[il].LevelName,
                                                                tempString,
                                                                localHash[PlayerList[ip].LevelData[il].LevelHash].ToString(),
                                                                "LD",
                                                                tempCS.GetString(E_Language.Chars) }));


                }

                for (int i = 0; i < LineList.Count; ++i)
                    totalSum += LineList[i].Length;

                tempCS.SetValue(totalSum);

                LineList.Add("PD." + tempCS.GetString(E_Language.Chars));

                tempCS.SetValue(String.Join("|", LineList.ToArray()), E_Language.UTF8);

                tempCS.MyOrder = 26;

                PlayerLines[ip] = tempCS.GetString(E_Language.English, true);

            }

            LineList.Clear();

            for (int i = 0; i < localKeys.Count; ++i)
            {
                tempCS.SetValue(0);
                tempCS.MyOrder = 256;
                tempCS.SetValue(localHash[localKeys[i]].ToString() + '|' + localKeys[i], E_Language.UTF8);

                tempCS.MyOrder = 26;

                LineList.Add(tempCS.GetString(E_Language.English, true));
            }
            int total = 0;

            for (int i = 0; i < PlayerLines.Length; ++i)
                total += PlayerLines[i].Length;

            for (int i = 0; i < LineList.Count; ++i)
                total += LineList[i].Length;

            tempCS.SetValue(0);
            tempCS.MyOrder = 256;
            tempCS.SetValue(total);

            tempString = "ИгроваяИнформация." + LineList.Count.ToString() + '.' + tempCS.GetString(E_Language.Chars);

            tempCS.SetValue(tempString, E_Language.UTF8);

            tempCS.MyOrder = 26;

            tempString = tempCS.GetString(E_Language.English, true);

            string[] MainLines = new string[1 + PlayerLines.Length + LineList.Count];

            MainLines[0] = tempString;



            for (int i = 0; i < LineList.Count; ++i)
                MainLines[i + 1] = LineList[i];

            for (int i = 0; i < PlayerLines.Length; ++i)
                MainLines[i + 1 + LineList.Count] = PlayerLines[i];

            System.IO.Directory.CreateDirectory("Saves");

            System.IO.File.WriteAllLines(@"Saves\Save.jrs", MainLines);


        }

        public bool GetLevelHash(string FilePath, out string OutString)
        {


            OutString = null;

            if (!System.IO.File.Exists(FilePath))
                return false;

            string[] lines = System.IO.File.ReadAllLines(FilePath);

            if (lines.Length == 0)
                return false;

            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] pathBytes;
            byte[] contentBytes;

            lines[0] = string.Concat(lines);

            lines[0] = new string(lines[0].ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());

            lines = lines[0].Split('~');

            bool TestBool = false;

            for (int i = 0; i < lines.Length; ++i)
            {
                if (lines[i].Contains("Textures"))
                {


                    lines = lines[i].Split(':');

                    if (lines.Length != 2)
                        break;





                    lines = lines[1].Split('[');

                    string[] localArr;



                    for (int ii = 0; ii < lines.Length; ++ii)
                    {

                        if (String.IsNullOrEmpty(lines[ii]))
                            continue;

                        if (lines[ii].IndexOf('(') < 0 || lines[ii].IndexOf(')') <= lines[ii].IndexOf('('))
                            continue;

                        lines[ii] = lines[ii].Substring(lines[ii].IndexOf('(') + 1, lines[ii].IndexOf(')') - lines[ii].IndexOf('(') - 1);

                        localArr = lines[ii].Split('=');

                        if (localArr.Length != 2 || localArr[0] != "TextureName")
                            continue;

                        System.Text.StringBuilder myBuilder = new System.Text.StringBuilder(localArr[1]);

                        myBuilder.Replace("\"", "");

                        myBuilder.Replace("%11", "(");
                        myBuilder.Replace("%12", ")");
                        myBuilder.Replace("%13", ",");
                        myBuilder.Replace("%14", "=");
                        myBuilder.Replace("%15", "[");
                        myBuilder.Replace("%16", "]");
                        myBuilder.Replace("%17", "~");
                        myBuilder.Replace("%18", ":");
                        myBuilder.Replace("%19", "\"");
                        myBuilder.Replace("%20", " ");

                        myBuilder.Replace("%10", "%");

                        localArr[1] = System.IO.Path.GetDirectoryName(FilePath) + @"\" + myBuilder.ToString();

                        if (!System.IO.File.Exists(localArr[1]))
                            return false;


                        if (!TestBool)
                        {
                            pathBytes = System.Text.Encoding.UTF8.GetBytes(FilePath.ToLower());

                            md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                            contentBytes = System.IO.File.ReadAllBytes(FilePath);

                            md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);

                            TestBool = true;
                        }



                        pathBytes = System.Text.Encoding.UTF8.GetBytes(localArr[1].ToLower());

                        md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                        contentBytes = System.IO.File.ReadAllBytes(localArr[1]);

                        if (ii == lines.Length - 1)
                            md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                        else
                            md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);

                    }


                    if (TestBool)
                    {
                        OutString = BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();


                        return true;
                    }
                }
            }

            if (!TestBool)
            {
                pathBytes = System.Text.Encoding.UTF8.GetBytes(FilePath.ToLower());

                md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                contentBytes = System.IO.File.ReadAllBytes(FilePath);

                md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);

                OutString = BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();

                return true;
            }


            return false;
        }



        public void ReadLevelData()
        {
            if (!System.IO.Directory.Exists("Levels"))
                System.IO.Directory.CreateDirectory("Levels");

            myLevelData.Clear();

            LevelProps localLP = new LevelProps();

            localLP.inGameLevel = true;
            localLP.LevelImage = Resources.Load<Sprite>("Level_0_img");
            localLP.LevelName = "First Level";
            localLP.LevelPath = "Level_0";
            localLP.LevelHash = "0";

            myLevelData.Add(1, localLP);

            localLP.LevelImage = Resources.Load<Sprite>("Level_1_img");
            localLP.LevelName = "Second Level";
            localLP.LevelPath = "Level_1";
            localLP.LevelHash = "1";

            myLevelData.Add(2, localLP);

            if (!System.IO.Directory.Exists("Levels"))
                return;

            string[] files = System.IO.Directory.GetFiles("Levels", "*.jrl", System.IO.SearchOption.AllDirectories);


            System.Text.StringBuilder myBuilder;
            string[] lines;
            string lLevelProps;
            string lTexture;

            string[] localArr;
            string[] propArr;
            string localString;

            int localI;

            for (int i = 0; i < files.Length; ++i)
            {
                lLevelProps = null;
                lTexture = null;

                lines = System.IO.File.ReadAllLines(files[i]);

                localString = string.Concat(lines);

                localString = new string(localString.ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());

                lines = localString.Split('~');

                for (int ii = 0; ii < lines.Length; ++ii)
                {
                    localArr = lines[ii].Split(':');
                    if (localArr.Length != 2)
                        continue;

                    localArr = localArr[0].Split('(');

                    switch (localArr[0])
                    {
                        case "LevelProp":
                            lLevelProps = lines[ii];
                            break;

                        case "Textures":
                            lTexture = lines[ii];
                            break;

                        default:
                            break;
                    }
                }

                if (lTexture == null || lLevelProps == null)
                    continue;

                localLP = new LevelProps();


                if (!GetLevelHash(files[i], out localString))
                    continue;

                localArr = lLevelProps.Split(':');

                if (localArr[1].IndexOf('(') >= localArr[1].IndexOf(')'))
                    continue;

                localArr[1] = localArr[1].Substring(localArr[1].IndexOf('(') + 1, localArr[1].IndexOf(')') - localArr[1].IndexOf('(') - 1);

                localArr = localArr[1].Split(',');

                for (int ii = 0; ii < localArr.Length; ++ii)
                {
                    propArr = localArr[ii].Split('=');

                    if (propArr.Length != 2)
                        continue;

                    switch (propArr[0])
                    {
                        case "Name":
                            myBuilder = new System.Text.StringBuilder(propArr[1]);

                            myBuilder.Replace("\"", "");

                            myBuilder.Replace("%11", "(");
                            myBuilder.Replace("%12", ")");
                            myBuilder.Replace("%13", ",");
                            myBuilder.Replace("%14", "=");
                            myBuilder.Replace("%15", "[");
                            myBuilder.Replace("%16", "]");
                            myBuilder.Replace("%17", "~");
                            myBuilder.Replace("%18", ":");
                            myBuilder.Replace("%19", "\"");
                            myBuilder.Replace("%20", " ");

                            myBuilder.Replace("%10", "%");

                            localLP.LevelName = myBuilder.ToString();

                            break;

                        case "LevelTexture":

                            Sprite mySprite = null;
                            Texture2D Tex2D;

                            if (propArr[1] == "\"SimpleBox\"")
                            {
                                Tex2D = Resources.Load<Texture2D>("S_SimpleBox");
                                mySprite = Sprite.Create(Tex2D, new Rect(0f, 0f, Tex2D.width, Tex2D.height), new Vector2(0.5f, 0.5f));

                            }
                            else if (propArr[1] == "\"SimpleCircle\"")
                            {
                                Tex2D = Resources.Load<Texture2D>("S_SimpleCircle");
                                mySprite = Sprite.Create(Tex2D, new Rect(0f, 0f, Tex2D.width, Tex2D.height), new Vector2(0.5f, 0.5f));
                            }
                            else
                            {

                                if (!int.TryParse(propArr[1], out localI))
                                    break;

                                string[] localLines = lTexture.Split('[');



                                for (int iii = 1; iii < localLines.Length; ++iii)
                                {
                                    propArr = localLines[iii].Split(']');

                                    if (propArr.Length != 2)
                                        continue;

                                    int testI;

                                    if (!int.TryParse(propArr[0], out testI))
                                        continue;


                                    if (testI == localI)
                                    {

                                        byte[] FileData;

                                        if (localLines[iii].IndexOf('(') < 0 || localLines[iii].IndexOf('(') >= localLines[iii].IndexOf(')'))
                                            break;

                                        localLines[iii] = localLines[iii].Substring(localLines[iii].IndexOf('(') + 1, localLines[iii].IndexOf(')') - localLines[iii].IndexOf('(') - 1);

                                        propArr = localLines[iii].Split('=');

                                        if (propArr.Length != 2 || propArr[0] != "TextureName")
                                            break;

                                        myBuilder = new System.Text.StringBuilder(propArr[1]);

                                        myBuilder.Replace("\"", "");

                                        myBuilder.Replace("%11", "(");
                                        myBuilder.Replace("%12", ")");
                                        myBuilder.Replace("%13", ",");
                                        myBuilder.Replace("%14", "=");
                                        myBuilder.Replace("%15", "[");
                                        myBuilder.Replace("%16", "]");
                                        myBuilder.Replace("%17", "~");
                                        myBuilder.Replace("%18", ":");
                                        myBuilder.Replace("%19", "\"");
                                        myBuilder.Replace("%20", " ");

                                        myBuilder.Replace("%10", "%");

                                        propArr[1] = System.IO.Path.GetDirectoryName(files[i]) + @"\" + myBuilder.ToString();

                                        if (System.IO.File.Exists(propArr[1]))
                                        {
                                            FileData = System.IO.File.ReadAllBytes(propArr[1]);
                                            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
                                            if (!Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                                                break;            // If data = readable -> return texture
                                        }
                                        else
                                            break;

                                        mySprite = Sprite.Create(Tex2D, new Rect(0f, 0f, Tex2D.width, Tex2D.height), new Vector2(0.5f, 0.5f));

                                        break;
                                    }



                                }
                            }

                            localLP.LevelImage = mySprite;

                            break;

                        default:
                            break;
                    }
                }



                localLP.LevelPath = files[i];
                localLP.inGameLevel = false;
                localLP.LevelHash = localString;

                myLevelData.Add(i + 3, localLP);

            }
        }

        public int GetMaxScore(string PlayerName, string MapName, string MapHash)
        {
            for (int i = 0; i < PlayerList.Count; ++i)
            {
                if (PlayerList[i].Name == PlayerName)
                {
                    for (int ii = 0; ii < PlayerList[i].LevelData.Count; ++ii)
                        if (PlayerList[i].LevelData[ii].LevelName == MapName && PlayerList[i].LevelData[ii].LevelHash == MapHash)
                            return PlayerList[i].LevelData[ii].Score;

                    break;
                }
            }

            return 0;
        }

        public int GetMaxScore(string PlayerName, out string MapName)
        {
            MapName = null;

            for (int i = 0; i < PlayerList.Count; ++i)
            {
                if (PlayerList[i].Name == PlayerName)
                {
                    if (PlayerList[i].MaxScoreLevel == null)
                        return 0;

                    MapName = PlayerList[i].MaxScoreLevel.LevelName;
                    return PlayerList[i].MaxScoreLevel.Score;

                }
            }

            return 0;
        }

        public void SetMaxScore(int inScore, string LevelName, string LevelHash, bool RewriteScoreIfSmoller = false)
        {
            if (MainPlayer != null)
            {
                MainPlayer.ChangeScore(LevelName, inScore, LevelHash, RewriteScoreIfSmoller);
            }
        }

        private void OnApplicationQuit()
        {
            WritePlayerData();
        }

        public static bool isTestingMode = false;

    }
}
public class CountSystem
{
    List<int> MyValue = new List<int>();
    int myOrder;

    public int MyOrder
    {
        get { return myOrder; }

        set
        {
            int inOrder = value;
            if (inOrder > 45000)
                inOrder = 45000;
            if (inOrder < 2)
                inOrder = 2;

            CountSystem newOne = new CountSystem(inOrder);

            CountSystem retCS = new CountSystem(inOrder);

            for (int i = 0; i < MyValue.Count; ++i)
            {
                newOne.MyValue.Clear();

                newOne.MyValue.Add(MyValue[i]);

                for (int ii = 0; ii < i; ++ii)
                    newOne.MultByInt(myOrder);

                retCS.SelfSum(newOne);

            }

            MyValue = retCS.MyValue;

            myOrder = inOrder;
        }
    }

    

    private CountSystem()
    {
        myOrder = 10;
    }

    public CountSystem(int inMyOrder)
    {

        myOrder = inMyOrder;
        if (myOrder < 2)
            myOrder = 2;
        if (myOrder > 45000)
            myOrder = 45000;


    }

    public CountSystem(int inMyOrder, int inTenCS)
    {
        myOrder = inMyOrder;

        if (myOrder < 2)
            myOrder = 2;
        if (myOrder > 45000)
            myOrder = 45000;

        MyValue.Add(inTenCS);
        Normalize();

    }

    public CountSystem(int inMyOrder, List<int> inList)
    {
        myOrder = inMyOrder;

        if (myOrder < 2)
            myOrder = 2;
        if (myOrder > 45000)
            myOrder = 45000;

        SetValue(inList);
    }

    public CountSystem(CountSystem inCS)
    {
        myOrder = inCS.myOrder;
        SetValue(inCS.MyValue);
    }

    public void SetValue(CountSystem inCS)
    {
        CountSystem localCS = inCS;
        if (myOrder != inCS.myOrder)
            localCS = GetNewOrder(myOrder, inCS);

        MyValue.Clear();

        if (MyValue.Capacity < localCS.MyValue.Count)
            MyValue.Capacity = localCS.MyValue.Count;

        for (int i = 0; i < localCS.MyValue.Count; ++i)
            MyValue.Add(localCS.MyValue[i]);
    }

    private void SetValue(List<int> inList)
    {
        MyValue.Clear();

        if (MyValue.Capacity < inList.Count)
            MyValue.Capacity = inList.Count;

        for (int i = 0; i < inList.Count; ++i)
            MyValue.Add(inList[i]);

        Normalize();
    }


    //-------------------adding stuff-------------------
    public void SetValue(int inTenth)
    {
        MyValue.Clear();
        MyValue.Add(inTenth);
        Normalize();
    }

    public bool SetValue(String inString, E_Language byLang, bool reverse = false)
    {

        List<int> TestList = new List<int>();

        TestList.Capacity = inString.Length;

        if (byLang == E_Language.UTF8)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(inString);

            if (reverse)
                Array.Reverse(bytes);

            for (int i = 0; i < bytes.Length; ++i)
                TestList.Add(bytes[i]);
        }
        else
            for (int i = 0; i < inString.Length; ++i)
            {
                int Test;
                if (reverse)
                    Test = Alphabet.GetNumFromChar(inString[inString.Length - 1 - i], byLang);
                else
                    Test = Alphabet.GetNumFromChar(inString[i], byLang);

                if (Test == -1)
                    return false;

                TestList.Add(Test);
            }



        MyValue = TestList;

        Normalize();

        return true;
    }

    public void AddSystem(CountSystem inCS)
    {
        CountSystem localCS = inCS;
        if (myOrder != inCS.myOrder)
            localCS = GetNewOrder(myOrder, inCS);

        if (MyValue.Capacity < MyValue.Count + localCS.MyValue.Count)
            MyValue.Capacity = MyValue.Count + localCS.MyValue.Count;

        for (int i = 0; i < localCS.MyValue.Count; ++i)
            MyValue.Add(localCS.MyValue[i]);



    }

    public int GetValueCount()
    { return MyValue.Count; }

    public string GetString(E_Language byLang, bool reverse = false)
    {
        if (byLang != E_Language.UTF8)
        {
            char[] charArr = new char[MyValue.Count];

            if (reverse)
                for (int i = 0; i < MyValue.Count; ++i)
                {
                    char testChar = Alphabet.GetCharFromNum(MyValue[i], byLang);

                    if (testChar == '\0')
                        return string.Empty;

                    charArr[charArr.Length - 1 - i] = testChar;
                }
            else
                for (int i = 0; i < MyValue.Count; ++i)
                {
                    char testChar = Alphabet.GetCharFromNum(MyValue[i], byLang);

                    if (testChar == '\0')
                        return string.Empty;

                    charArr[i] = testChar;
                }

            

            return new string(charArr);
        }

        byte[] byteArr = new byte[MyValue.Count];

        for (int i = 0; i < MyValue.Count; ++i)
        {
            if (MyValue[i] > 255)
                return string.Empty;

            byteArr[i] = (byte)MyValue[i];
        }

        

        char[] CharArr = System.Text.Encoding.UTF8.GetChars(byteArr);

        //string retStr = new string(System.Text.Encoding.UTF8.GetChars(byteArr));
        

        
        if(reverse && CharArr.Length > 1)
            Array.Reverse(CharArr);

        return new string(CharArr);
    }

    //--------------------------------------------------

    public void Normalize()
    {


        for (int i = 0; i < MyValue.Count; ++i)
        {
            if (MyValue[i] >= myOrder || MyValue[i] <= -myOrder)
            {
                int SomeInt = MyValue[i] / myOrder;
                MyValue[i] -= myOrder * SomeInt;

                if (i < MyValue.Count - 1)
                    MyValue[i + 1] += SomeInt;
                else
                    MyValue.Add(SomeInt);
            }
        }

        for (int i = MyValue.Count - 1; i >= 0; --i)
            if (MyValue[i] == 0)
                MyValue.RemoveAt(i);
            else
                break;

        int count = MyValue.Count;

        if (MyValue.Count > 1)
        {
            if (MyValue[count - 1] < 0)
                for (int i = count - 2; i >= 0; --i)
                    if (MyValue[i] > 0)
                    {
                        MyValue[i] -= myOrder;
                        ++MyValue[i + 1];
                    }

            if (MyValue[count - 1] > 0)
                for (int i = count - 2; i >= 0; --i)
                    if (MyValue[i] < 0)
                    {
                        MyValue[i] += myOrder;
                        --MyValue[i + 1];
                    }
        }

        for (int i = count - 1; i > 0; --i)
            if (MyValue[i] == 0)
                MyValue.RemoveAt(i);
            else
                break;
    }

    public static void NormalizeList(List<int> inList, int inOrder)
    {
        int count = inList.Count;

        for (int i = 0; i < count; ++i)
        {
            if (inList[i] >= inOrder)
            {
                int SomeInt = inList[i] / inOrder;
                inList[i] -= inOrder * SomeInt;

                if (i < count - 1)
                    inList[i + 1] += SomeInt;
                else
                    inList.Add(SomeInt);
            }
        }

        for (int i = count - 1; i > 0; --i)
            if (inList[i] == 0)
                inList.RemoveAt(i);
            else
                break;

        count = inList.Count;

        if (inList.Count > 1)
        {
            if (inList[count - 1] < 0)
                for (int i = count - 2; i >= 0; --i)
                    if (inList[i] > 0)
                    {
                        inList[i] -= inOrder;
                        ++inList[i + 1];
                    }

            if (inList[count - 1] > 0)
                for (int i = count - 2; i >= 0; --i)
                    if (inList[i] < 0)
                    {
                        inList[i] += inOrder;
                        --inList[i + 1];
                    }
        }

        for (int i = count - 1; i > 0; --i)
            if (inList[i] == 0)
                inList.RemoveAt(i);
            else
                break;
    }

    private static void ListTenPow(List<int> inList, int TenthOrder)
    {
        if (TenthOrder <= 0)
            return;

        int count = inList.Count;

        if (inList.Capacity < count + TenthOrder)
            inList.Capacity = count + TenthOrder;


        for (int i = 0; i < TenthOrder; ++i)
            inList.Add(0);

        for (int i = count - 1; i >= 0; --i)
            inList[i + TenthOrder] = inList[i];

        for (int i = 0; i < TenthOrder; ++i)
            inList[i] = 0;

    }

    public void SelfTenPow(int TenthOrder)
    {
        if (TenthOrder <= 0)
            return;

        int count = MyValue.Count;

        if (MyValue.Capacity < count + TenthOrder)
            MyValue.Capacity = count + TenthOrder;

        for (int i = 0; i < TenthOrder; ++i)
            MyValue.Add(0);

        for (int i = count - 1; i >= 0; --i)
            MyValue[i + TenthOrder] = MyValue[i];

        for (int i = 0; i < TenthOrder; ++i)
            MyValue[i] = 0;
    }

    public int this[int inIndex]
    {
        get { return MyValue[inIndex]; }
        set
        {
            if (inIndex < 0)
                return;

            if (inIndex >= MyValue.Count)
            {
                if (value == 0)
                    return;

                for (int i = MyValue.Count; i <= inIndex; ++i)
                    MyValue.Add(0);

            }

            MyValue[inIndex] = value;

            if (value >= myOrder || value <= -myOrder)
                Normalize();
        }
    }

    public static CountSystem GetNewOrder(int inOrder, CountSystem inCS)
    {
        if (inOrder > 45000)
            inOrder = 45000;
        if (inOrder < 2)
            inOrder = 2;

        CountSystem newOne = new CountSystem(inOrder);

        CountSystem retCS = new CountSystem(inOrder);

        for (int i = 0; i < inCS.MyValue.Count; ++i)
        {
            newOne.MyValue.Clear();

            newOne.MyValue.Add(inCS.MyValue[i]);

            for (int ii = 0; ii < i; ++ii)
                newOne.MultByInt(inCS.myOrder);

            retCS.SelfSum(newOne);

        }

        return retCS;
    }


    /// <summary>
    /// Multiply Count System by integer
    /// </summary>
    /// <param name="inInt"> 
    /// Abs must be less then 45000! if not function will consider this number as +-45000 
    /// </param>
    public void MultByInt(int inInt)
    {
        if (inInt > 45000)
            inInt = 45000;
        if (inInt < -45000)
            inInt = -45000;

        for (int i = 0; i < MyValue.Count; ++i)
            MyValue[i] *= inInt;

        Normalize();
    }


    public static CountSystem operator +(CountSystem inCSfirst, CountSystem inCSsecond)
    {
        CountSystem retCS = new CountSystem(inCSfirst.myOrder);
        retCS.MyValue.Clear();

        CountSystem inSecond = inCSsecond;

        if (inCSfirst.myOrder != inCSsecond.myOrder)
            inSecond = GetNewOrder(inCSfirst.myOrder, inCSsecond);

        for (int i = 0; i < inCSfirst.MyValue.Count && i < inSecond.MyValue.Count; ++i)
            retCS.MyValue.Add(inCSfirst.MyValue[i] + inSecond.MyValue[i]);

        if (inCSfirst.MyValue.Count > inSecond.MyValue.Count)
            for (int i = inSecond.MyValue.Count; i < inCSfirst.MyValue.Count; ++i)
                retCS.MyValue.Add(inCSfirst.MyValue[i]);

        if (inCSfirst.MyValue.Count < inSecond.MyValue.Count)
            for (int i = inCSfirst.MyValue.Count; i < inSecond.MyValue.Count; ++i)
                retCS.MyValue.Add(inSecond.MyValue[i]);

        retCS.Normalize();

        return retCS;
    }

    public static CountSystem operator *(CountSystem inCSfirst, CountSystem inCSsecond)
    {

        CountSystem TestCS = new CountSystem(inCSfirst.myOrder);


        CountSystem retCS = new CountSystem(inCSfirst.myOrder);

        CountSystem inSecond = inCSsecond;
        if (inCSfirst.myOrder != inCSsecond.myOrder)
            inSecond = GetNewOrder(inCSfirst.myOrder, inCSsecond);

        for (int i = 0; i < inSecond.MyValue.Count; ++i)
        {
            TestCS.SetValue(inCSfirst);

            for (int ii = 0; ii < TestCS.MyValue.Count; ++ii)
                TestCS.MyValue[ii] *= inSecond.MyValue[i];

            TestCS.Normalize();

            TestCS.SelfTenPow(i);

            retCS.SelfSum(TestCS);



        }

        return retCS;
    }

    public static CountSystem operator -(CountSystem inCSfirst, CountSystem inCSsecond)
    {
        CountSystem retCS = new CountSystem(inCSfirst.myOrder);
        retCS.MyValue.Clear();

        CountSystem inSecond = inCSsecond;

        if (inCSfirst.myOrder != inCSsecond.myOrder)
            inSecond = GetNewOrder(inCSfirst.myOrder, inCSsecond);

        for (int i = 0; i < inCSfirst.MyValue.Count && i < inSecond.MyValue.Count; ++i)
            retCS.MyValue.Add(inCSfirst.MyValue[i] - inSecond.MyValue[i]);

        if (inCSfirst.MyValue.Count > inSecond.MyValue.Count)
            for (int i = inSecond.MyValue.Count; i < inCSfirst.MyValue.Count; ++i)
                retCS.MyValue.Add(inCSfirst.MyValue[i]);

        if (inCSfirst.MyValue.Count < inSecond.MyValue.Count)
            for (int i = inCSfirst.MyValue.Count; i < inSecond.MyValue.Count; ++i)
                retCS.MyValue.Add(-inSecond.MyValue[i]);

        retCS.Normalize();

        return retCS;


    }

    public static CountSystem operator /(CountSystem inCSfirst, CountSystem inCSsecond)
    {
        CountSystem inSecond = inCSsecond;

        if (inCSfirst.myOrder != inCSsecond.myOrder)
            inSecond = GetNewOrder(inCSfirst.myOrder, inCSsecond);

        CountSystem retCS = new CountSystem(inCSfirst.myOrder);

        CountSystem testCS = new CountSystem(inCSfirst.myOrder);

        CountSystem localCS = new CountSystem(inCSfirst.myOrder);


        for (int i = 1; i < inSecond.MyValue.Count; ++i)
            testCS.MyValue.Add(inCSfirst[inCSfirst.MyValue.Count - inSecond.MyValue.Count + i]);

        for (int i = inCSfirst.MyValue.Count - inSecond.MyValue.Count; i >= 0; --i)
        {
            testCS.MyValue.Insert(0, inCSfirst.MyValue[i]);

            if (testCS < inSecond)
            {
                retCS.MyValue.Add(0);

                continue;
            }
            else
            {
                int Min = 1;
                int Max = inCSfirst.myOrder;

                int Mid = (Max + Min) / 2;

                while (Max - Min > 1)
                {

                    Mid = (Max + Min) / 2;

                    localCS.SetValue(inSecond);
                    localCS.MultByInt(Mid);

                    if (localCS <= testCS)
                    {
                        Min = Mid;
                        if (localCS == testCS)
                            break;
                    }
                    else
                    {
                        Max = Mid;
                    }
                }

                retCS.MyValue.Add(Min);

                localCS.SetValue(inSecond);
                localCS.MultByInt(Min);

                testCS.SelfSub(localCS);
            }

        }

        retCS.MyValue.Reverse();

        retCS.Normalize();

        return retCS;

    }

    public void SelfSum(CountSystem inCS)
    {

        CountSystem localCS = inCS;

        if (myOrder != inCS.myOrder)
            localCS = GetNewOrder(myOrder, inCS);

        for (int i = 0; i < MyValue.Count && i < localCS.MyValue.Count; ++i)
            MyValue[i] += localCS.MyValue[i];

        if (MyValue.Count < localCS.MyValue.Count)
            for (int i = MyValue.Count; i < localCS.MyValue.Count; ++i)
                MyValue.Add(localCS.MyValue[i]);

        Normalize();
    }

    public void SelfMul(CountSystem inCS)
    {
        CountSystem TestCS = new CountSystem(myOrder);


        CountSystem retCS = new CountSystem(myOrder);

        CountSystem inSecond = inCS;
        if (myOrder != inCS.myOrder)
            inSecond = GetNewOrder(myOrder, inCS);

        for (int i = 0; i < inSecond.MyValue.Count; ++i)
        {
            TestCS.SetValue(this);

            for (int ii = 0; ii < TestCS.MyValue.Count; ++ii)
                TestCS.MyValue[ii] *= inSecond.MyValue[i];

            TestCS.Normalize();

            TestCS.SelfTenPow(i);

            retCS.SelfSum(TestCS);

        }

        MyValue = retCS.MyValue;
    }

    public void SelfSub(CountSystem inCS)
    {
        CountSystem localCS = inCS;

        if (myOrder != inCS.myOrder)
            localCS = GetNewOrder(myOrder, inCS);

        for (int i = 0; i < MyValue.Count && i < localCS.MyValue.Count; ++i)
            MyValue[i] -= localCS.MyValue[i];

        if (MyValue.Count < localCS.MyValue.Count)
            for (int i = MyValue.Count; i < localCS.MyValue.Count; ++i)
                MyValue.Add(-localCS.MyValue[i]);

        Normalize();
    }

    public void SelfDiv(CountSystem inCS)
    {
        CountSystem inSecond = inCS;

        if (myOrder != inCS.myOrder)
            inSecond = GetNewOrder(myOrder, inCS);

        CountSystem retCS = new CountSystem(myOrder);

        CountSystem testCS = new CountSystem(myOrder);

        CountSystem localCS = new CountSystem(myOrder);

        for (int i = 1; i < inSecond.MyValue.Count; ++i)
            testCS.MyValue.Add(MyValue[MyValue.Count - inSecond.MyValue.Count + i]);

        for (int i = MyValue.Count - inSecond.MyValue.Count; i >= 0; --i)
        {
            testCS.MyValue.Insert(0, MyValue[i]);

            if (testCS < inSecond)
            {
                retCS.MyValue.Add(0);

                continue;
            }
            else
            {
                int Min = 1;
                int Max = myOrder;

                while (Max - Min > 1)
                {
                    int Mid = (Max + Min) / 2;

                    localCS.SetValue(inSecond);
                    localCS.MultByInt(Mid);

                    if (localCS <= testCS)
                    {
                        Min = Mid;
                        if (localCS == testCS)
                            break;
                    }
                    else
                        Max = Mid;

                }


                localCS.SetValue(inSecond);
                localCS.MultByInt(Min);

                testCS.SelfSub(localCS);

                retCS.MyValue.Add(Min);
            }

        }
        retCS.MyValue.Reverse();

        retCS.Normalize();

        MyValue = retCS.MyValue;
    }

    public static bool operator >(CountSystem inCSfirst, CountSystem inCSsecond)
    {
        CountSystem inSecond = inCSsecond;

        if (inCSfirst.myOrder != inCSsecond.myOrder)
            inSecond = GetNewOrder(inCSfirst.myOrder, inCSsecond);

        if (inCSfirst.MyValue.Count > inSecond.MyValue.Count)
            return inCSfirst.MyValue[inCSfirst.MyValue.Count - 1] > 0;

        if (inCSfirst.MyValue.Count < inSecond.MyValue.Count)
            return inSecond.MyValue[inSecond.MyValue.Count - 1] < 0;


        for (int i = inCSfirst.MyValue.Count - 1; i >= 0; --i)
        {
            if (inCSfirst.MyValue[i] == inSecond.MyValue[i])
                continue;

            if (inCSfirst.MyValue[i] > inSecond.MyValue[i])
                return true;

            if (inCSfirst.MyValue[i] < inSecond.MyValue[i])
                return false;
        }

        return false;
    }

    public static bool operator <(CountSystem inCSfirst, CountSystem inCSsecond)
    {
        CountSystem inSecond = inCSsecond;

        if (inCSfirst.myOrder != inCSsecond.myOrder)
            inSecond = GetNewOrder(inCSfirst.myOrder, inCSsecond);

        if (inCSfirst.MyValue.Count > inSecond.MyValue.Count)
            return inCSfirst.MyValue[inCSfirst.MyValue.Count - 1] < 0;


        if (inCSfirst.MyValue.Count < inSecond.MyValue.Count)
            return inSecond.MyValue[inSecond.MyValue.Count - 1] > 0;


        for (int i = inCSfirst.MyValue.Count - 1; i >= 0; --i)
        {
            if (inCSfirst.MyValue[i] == inSecond.MyValue[i])
                continue;

            if (inCSfirst.MyValue[i] > inSecond.MyValue[i])
                return false;

            if (inCSfirst.MyValue[i] < inSecond.MyValue[i])
                return true;
        }

        return false;
    }

    public static bool operator >=(CountSystem inCSfirst, CountSystem inCSsecond)
    {
        CountSystem inSecond = inCSsecond;

        if (inCSfirst.myOrder != inCSsecond.myOrder)
            inSecond = GetNewOrder(inCSfirst.myOrder, inCSsecond);

        if (inCSfirst.MyValue.Count > inSecond.MyValue.Count)
            return inCSfirst.MyValue[inCSfirst.MyValue.Count - 1] > 0;


        if (inCSfirst.MyValue.Count < inSecond.MyValue.Count)
            return inSecond.MyValue[inSecond.MyValue.Count - 1] < 0;


        for (int i = inCSfirst.MyValue.Count - 1; i >= 0; --i)
        {
            if (inCSfirst.MyValue[i] == inSecond.MyValue[i])
                continue;

            if (inCSfirst.MyValue[i] > inSecond.MyValue[i])
                return true;

            if (inCSfirst.MyValue[i] < inSecond.MyValue[i])
                return false;
        }

        return true;
    }

    public static bool operator <=(CountSystem inCSfirst, CountSystem inCSsecond)
    {
        CountSystem inSecond = inCSsecond;

        if (inCSfirst.myOrder != inCSsecond.myOrder)
            inSecond = GetNewOrder(inCSfirst.myOrder, inCSsecond);

        if (inCSfirst.MyValue.Count > inSecond.MyValue.Count)
            return inCSfirst.MyValue[inCSfirst.MyValue.Count - 1] < 0;


        if (inCSfirst.MyValue.Count < inSecond.MyValue.Count)
            return inSecond.MyValue[inSecond.MyValue.Count - 1] > 0;


        for (int i = inCSfirst.MyValue.Count - 1; i >= 0; --i)
        {
            if (inCSfirst.MyValue[i] == inSecond.MyValue[i])
                continue;

            if (inCSfirst.MyValue[i] > inSecond.MyValue[i])
                return false;

            if (inCSfirst.MyValue[i] < inSecond.MyValue[i])
                return true;
        }

        return true;
    }



    public override int GetHashCode()
    {
        int TestInt = 0;
        int MiddleSum;
        unchecked
        {
            for (int i = 0; i < MyValue.Count; ++i)
            {
                MiddleSum = MyValue[i];
                for (int ii = 0; ii < i; ++ii)
                    MiddleSum *= myOrder;

                TestInt += MiddleSum;

            }
        }
        return TestInt;
    }

    public override bool Equals(object obj)
    {
        CountSystem TestCS = obj as CountSystem;

        if (TestCS == null)
            return false;

        CountSystem inSecond = TestCS;

        if (myOrder != inSecond.myOrder)
            inSecond = GetNewOrder(myOrder, TestCS);

        if (MyValue.Count != inSecond.MyValue.Count)
            return false;

        for (int i = 0; i < MyValue.Count; ++i)
            if (MyValue[i] != inSecond.MyValue[i])
                return false;

        return true;

    }

    public override string ToString()
    {
        if (MyValue.Count == 0)
            return String.Empty;

        System.Text.StringBuilder sb = new System.Text.StringBuilder(MyValue[MyValue.Count - 1].ToString());

        for (int i = MyValue.Count - 2; i >= 0; --i)
            sb.Append("." + MyValue[i].ToString());

        sb.Append("(" + myOrder.ToString() + ")");

        return sb.ToString();

    }

}

public enum E_Language
{
    English,
    Russian,
    EnglishFull,
    RussianFull,
    UTF8,
    Chars
}

class Alphabet
{
    static readonly char[] RusAlph = {'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и',
                                            'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т',
                                            'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь',
                                            'э', 'ю', 'я' };

    static readonly char[] EngAlph = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
                                            'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
                                            'u', 'v', 'w', 'x', 'y', 'z' };

    static readonly char[] RusFullAlph = {'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и',
                                                'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т',
                                                'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь',
                                                'э', 'ю', 'я',
                                                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                                                '.', ',', ' ', '[', ']', '|' };

    static readonly char[] EngFullAlph = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
                                                'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
                                                'u', 'v', 'w', 'x', 'y', 'z',
                                                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                                                '.', ',', ' ', '[', ']', '|' };

    public static char GetCharFromNum(int inNum, E_Language inLanguage)
    {
        if (inNum >= 0)
            switch (inLanguage)
            {
                case E_Language.English:
                    if (inNum < EngAlph.Length)
                        return EngAlph[inNum];

                    break;
                case E_Language.Russian:
                    if (inNum < RusAlph.Length)
                        return RusAlph[inNum];
                    break;
                case E_Language.EnglishFull:
                    if (inNum < EngFullAlph.Length)
                        return EngFullAlph[inNum];
                    break;
                case E_Language.RussianFull:
                    if (inNum < RusFullAlph.Length)
                        return RusFullAlph[inNum];
                    break;
                case E_Language.Chars:
                    return (char)inNum;

            }

        return '\0';
    }

    public static int GetNumFromChar(char inChar, E_Language inLanguage)
    {
        switch (inLanguage)
        {
            case E_Language.English:
                for (int i = 0; i < EngAlph.Length; ++i)
                    if (EngAlph[i] == inChar)
                        return i;

                break;
            case E_Language.Russian:
                for (int i = 0; i < RusAlph.Length; ++i)
                    if (RusAlph[i] == inChar)
                        return i;
                break;

            case E_Language.EnglishFull:
                for (int i = 0; i < EngFullAlph.Length; ++i)
                    if (EngFullAlph[i] == inChar)
                        return i;

                break;
            case E_Language.RussianFull:
                for (int i = 0; i < RusFullAlph.Length; ++i)
                    if (RusFullAlph[i] == inChar)
                        return i;
                break;

            case E_Language.Chars:
                return (int)inChar;

        }

        return -1;
    }

}

