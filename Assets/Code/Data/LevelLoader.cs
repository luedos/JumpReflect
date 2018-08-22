using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameProccess;

namespace DataManipulators
{
    public class LevelLoader : MonoBehaviour
    {

        private static string LevelFilePath;
        private static bool TestMode = false;

        public static void LoadLevel(string FilePath, bool inTestMode = false)
        {
            if (!System.IO.File.Exists(FilePath))
            {
                InfoMessage.ShowError("Level file not found");
                return;
            }

            TestMode = inTestMode;

            LevelFilePath = FilePath;
            SceneManager.LoadScene("SoftLevel");
        }

        public void OpenLevelInCreator()
        {
            if (!System.IO.File.Exists(LevelFilePath))
            {
                InfoMessage.ShowError("Level file not found");
                return;
            }

            LevelCreator.LevelCreatorManager.FileName = System.IO.Path.GetFileName(LevelFilePath);
            LevelCreator.LevelCreatorManager.SavePath = System.IO.Path.GetDirectoryName(LevelFilePath);

            SceneManager.LoadScene("Level_LevelCreator");
        }

        [Header("Level objects")]
        public GameObject PlayerStart;
        public GameObject MBStart;
        public GameObject OutWallsObj;
        public GameObject InWallsObj;
        public Camera MainCamera;
        public Camera BackgroundCamera;

        [Header("Level empties")]
        public GameManager myGameMan;
        public DataPool myDataPool;
        public SideBall SideBallSample;
        public SmallBall SmallBallSample;

        [Header("Gameplay objects")]
        public Player myPlayer;
        public MainBall MBSample;

        [Header("Game events")]
        public HUD_Emergency LeftHE;
        public HUD_Emergency RightHE;
        public HUD_Emergency TopHE;
        public HUD_Emergency BottomHE;
        [Space(5)]
        public HUD_Emergency TopLeftHE;
        public HUD_Emergency TopRightHE;
        public HUD_Emergency BottomLeftHE;
        public HUD_Emergency BottomRightHE;
        [Space(5)]
        public HUD_Emergency CircleHE;

        private Dictionary<int, Texture2D> myTextures = new Dictionary<int, Texture2D>();
        private Dictionary<int, Sprite> mySprites = new Dictionary<int, Sprite>();

        // Use this for initialization
        void Start()
        {

            if (!LoadMapFile(LevelFilePath))
            {
                return;
            }

            myGameMan.TestMode = TestMode;
            myGameMan.NewGame();

        }

        public bool LoadMapFile(string FilePath)
        {
            if (!System.IO.File.Exists(FilePath))
            {
                InfoMessage.ShowError("Level file not found");
                return false;
            }

            string[] stringArr = System.IO.File.ReadAllLines(FilePath);

            string localString = string.Concat(stringArr);

            localString = new string(localString.ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());

            stringArr = localString.Split('~');

            for (int i = stringArr.Length - 1; i >= 0; --i)
            {
                if (stringArr[i].Contains("Textures"))
                    if (LoadTextures(stringArr[i]))
                        break;
                    else
                    {
                        InfoMessage.ShowError("Load textures fail");
                        return false;
                    }

            }

            for (int i = stringArr.Length - 1; i >= 0; --i)
            {
                if (stringArr[i].Contains("Sprites"))
                    if (LoadSprites(stringArr[i]))
                        break;
                    else
                    {
                        InfoMessage.ShowError("Load sprites fail");
                        return false;
                    }
            }

            string[] localArr;

            for (int i = 0; i < stringArr.Length; ++i)
            {
                localArr = stringArr[i].Split(':');

                if (localArr.Length != 2)
                    continue;

                localArr = localArr[0].Split('(');

                switch (localArr[0])
                {
                    case "Camera":
                        if (!Change_Camera(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in camera read");
                            return false;
                        }
                        break;

                    case "Events":
                        if (!Change_Events(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in events read");
                            return false;
                        }
                        break;

                    case "MapSettings":
                        if (!Change_MapSettings(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in map settings read");
                            return false;
                        }
                        break;

                    case "MainPlayer":
                        if (!Change_Player(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in player read");
                            return false;
                        }
                        break;

                    case "MainBalls":
                        if (!Change_MainBalls(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in main balls read");
                            return false;
                        }
                        break;

                    case "SmallBall":
                        if (!Change_SmallBall(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in small ball read");
                            return false;
                        }
                        break;

                    case "SideBall":
                        if (!Change_SideBall(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in side ball read");
                            return false;
                        }
                        break;

                    case "OutWalls":
                        if (!Change_OutWalls(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in \"out walls\" read");
                            return false;
                        }
                        break;

                    case "InWalls":
                        if (!Change_InWalls(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in \"in walls\" read");
                            return false;
                        }
                        break;

                    default:
                        break;
                }
            }

            return true;
        }

        private bool LoadTextures(string inString)
        {
            string[] lines = inString.Split(':');
            if (lines.Length != 2)
                return false;

            myTextures.Clear();

            lines = lines[1].Split('[');

            for (int i = 0; i < lines.Length; ++i)
            {
                string[] localArr = lines[i].Split(']');

                if (localArr.Length != 2)
                    continue;

                int myNum;

                if (!int.TryParse(localArr[0], out myNum))
                    continue;

                if (localArr[1].IndexOf('(') >= localArr[1].IndexOf(')'))
                    continue;

                localArr[1] = localArr[1].Substring(localArr[1].IndexOf('(') + 1, localArr[1].IndexOf(')') - localArr[1].IndexOf('(') - 1);

                localArr = localArr[1].Split('=');

                if (localArr.Length != 2 || localArr[0] != "TextureName")
                    continue;

                Texture2D Tex2D;
                byte[] FileData;

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

                localArr[1] = System.IO.Path.GetDirectoryName(LevelFilePath) + @"\" + myBuilder.ToString();

                if (System.IO.File.Exists(localArr[1]))
                {
                    FileData = System.IO.File.ReadAllBytes(localArr[1]);
                    Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
                    if (!Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                        continue;            // If data = readable -> return texture
                }
                else
                    continue;

                myTextures.Add(myNum, Tex2D);

            }

            return true;

        }

        private bool LoadSprites(string inString)
        {
            string[] lines = inString.Split(':');
            if (lines.Length != 2)
                return false;

            mySprites.Clear();

            lines = lines[1].Split('[');



            for (int i = 0; i < lines.Length; ++i)
            {


                string[] localArr = lines[i].Split(']');

                if (localArr.Length != 2)
                    continue;

                int myNum;

                if (!int.TryParse(localArr[0], out myNum))
                    continue;

                if (localArr[1].IndexOf('(') >= localArr[1].IndexOf(')'))
                    continue;

                localArr[1] = localArr[1].Substring(localArr[1].IndexOf('(') + 1, localArr[1].IndexOf(')') - localArr[1].IndexOf('(') - 1);

                localArr = localArr[1].Split(',');


                if (localArr.Length == 0)
                    continue;

                //Sprite localSpite;

                Vector2 Pivot = new Vector2();
                float PixPerUnit = 100;
                Texture2D Tex2D = null;

                for (int ii = 0; ii < localArr.Length; ++ii)
                {
                    string[] propArr = localArr[ii].Split('=');

                    if (propArr.Length != 2)
                        continue;

                    switch (propArr[0])
                    {
                        case "Pivot":
                            propArr = propArr[1].Split('|');
                            if (propArr.Length != 2)
                                break;

                            if (!float.TryParse(propArr[0], out Pivot.x))
                                Pivot.x = 0.5f;
                            if (!float.TryParse(propArr[1], out Pivot.y))
                                Pivot.y = 0.5f;

                            break;

                        case "Texture":

                            if (propArr[1] == "\"SimpleBox\"")
                            {
                                Tex2D = Resources.Load<Texture2D>("S_SimpleBox");
                            }
                            else if (propArr[1] == "\"SimpleCircle\"")
                            {
                                Tex2D = Resources.Load<Texture2D>("S_SimpleCircle");
                            }
                            else
                            {
                                int localInt;
                                if (!int.TryParse(propArr[1], out localInt))
                                    break;

                                if (!myTextures.ContainsKey(localInt))
                                {
                                    print("texture not find (index : " + localInt.ToString() + ')');
                                    break;
                                }
                                Tex2D = myTextures[localInt];
                            }
                            break;

                        case "PixelsPerUnit":

                            if (!float.TryParse(propArr[1], out PixPerUnit))
                                PixPerUnit = 100;

                            break;

                    }

                }


                if (Tex2D == null)
                    continue;

                mySprites.Add(myNum, Sprite.Create(Tex2D, new Rect(0f, 0f, Tex2D.width, Tex2D.height), Pivot, PixPerUnit));

            }


            return true;
        }

        enum CollisionType
        {
            Circle, Box, Capsule
        }

        private bool Change_InWalls(string inString)
        {
            Color totalColor = Color.white;
            CollisionType totalColl = CollisionType.Box;
            Vector2 totalCollisionSize = new Vector2(-1f, -1f);
            Vector3 totalPos = new Vector3();
            float totalRot = 0f;
            Vector3 totalScale = Vector3.one;
            Sprite totalSprite = Resources.Load<Sprite>("S_SimpleBox");

            string[] localStrings = inString.Split(':');

            if (localStrings.Length != 2)
                return false;

            string[] localArr = new string[1];
            string[] propArr;

            float localF;
            int localI;

            if (localStrings[0].IndexOf('(') != -1 && localStrings[0].IndexOf('(') < localStrings[0].IndexOf(')'))
            {
                localArr[0] = localStrings[0].Substring(localStrings[0].IndexOf('(') + 1, localStrings[0].IndexOf(')') - localStrings[0].IndexOf('(') - 1);

                localArr = localArr[0].Split(',');

                for (int i = 0; i < localArr.Length; ++i)
                {
                    propArr = localArr[i].Split('=');

                    if (propArr.Length != 2)
                        continue;

                    switch (propArr[0])
                    {
                        case "Pos":
                            propArr = propArr[1].Split('|');

                            for (int ii = 0; ii < propArr.Length && ii < 3; ++ii)
                                switch (ii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalPos.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalPos.y = localF;
                                        break;
                                    case 2:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalPos.z = localF;
                                        break;
                                }

                            break;

                        case "Rot":

                            if (float.TryParse(propArr[1], out localF))
                                totalRot = localF;

                            break;

                        case "Scale":

                            propArr = propArr[1].Split('|');

                            for (int ii = 0; ii < propArr.Length && ii < 3; ++ii)
                                switch (ii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalScale.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalScale.y = localF;
                                        break;
                                }

                            break;

                        case "Sprite":

                            if (int.TryParse(propArr[1], out localI))
                            {
                                Sprite localOne = mySprites[localI];
                                if (localOne != null)
                                    totalSprite = localOne;
                            }

                            break;

                        case "Color":

                            propArr = propArr[1].Split('|');

                            for (int ii = 0; ii < propArr.Length && ii < 4; ++ii)
                                switch (ii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalColor.r = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalColor.g = localF;
                                        break;
                                    case 2:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalColor.b = localF;
                                        break;
                                    case 3:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalColor.a = localF;
                                        break;
                                }

                            break;

                        case "Collision":

                            switch (propArr[1])
                            {
                                case "Box":
                                    totalColl = CollisionType.Box;
                                    break;

                                case "Capsule":
                                    totalColl = CollisionType.Capsule;
                                    break;

                                case "Circle":
                                    totalColl = CollisionType.Circle;
                                    break;

                            }
                            break;

                        case "CollisionSize":

                            propArr = propArr[1].Split('|');

                            for (int ii = 0; ii < propArr.Length && ii < 2; ++ii)
                                switch (ii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalCollisionSize.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalCollisionSize.y = localF;
                                        break;

                                }

                            break;

                        default:
                            break;
                    }

                }


            }

            localStrings = localStrings[1].Split('[');

            for (int i = 0; i < localStrings.Length; ++i)
            {
                Color localColor = totalColor;
                CollisionType localColl = totalColl;
                Vector3 localPos = totalPos;
                float localRot = totalRot;
                Vector3 localScale = totalScale;
                Vector2 localCollisionSize = totalCollisionSize;
                Sprite localSprite = totalSprite;

                if (localStrings[i].IndexOf('(') == -1 || localStrings[i].IndexOf(')') - localStrings[i].IndexOf('(') < 2)
                    continue;

                localArr[0] = localStrings[i].Substring(localStrings[i].IndexOf('(') + 1, localStrings[i].IndexOf(')') - localStrings[i].IndexOf('(') - 1);

                localArr = localArr[0].Split(',');

                for (int ii = 0; ii < localArr.Length; ++ii)
                {
                    propArr = localArr[ii].Split('=');

                    if (propArr.Length != 2)
                        continue;

                    switch (propArr[0])
                    {
                        case "Pos":
                            propArr = propArr[1].Split('|');

                            for (int iii = 0; iii < propArr.Length && iii < 3; ++iii)
                                switch (iii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localPos.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localPos.y = localF;
                                        break;
                                    case 2:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localPos.z = localF;
                                        break;
                                }

                            break;

                        case "Rot":

                            if (float.TryParse(propArr[1], out localF))
                                localRot = localF;

                            break;

                        case "Scale":

                            propArr = propArr[1].Split('|');

                            for (int iii = 0; iii < propArr.Length && iii < 3; ++iii)
                                switch (iii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localScale.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localScale.y = localF;
                                        break;
                                }

                            break;

                        case "Sprite":

                            if (int.TryParse(propArr[1], out localI))
                            {
                                Sprite localOne = mySprites[localI];
                                if (localOne != null)
                                    localSprite = localOne;
                            }

                            break;

                        case "Color":

                            propArr = propArr[1].Split('|');

                            for (int iii = 0; iii < propArr.Length && iii < 4; ++iii)
                                switch (iii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localColor.r = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localColor.g = localF;
                                        break;
                                    case 2:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localColor.b = localF;
                                        break;
                                    case 3:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localColor.a = localF;
                                        break;
                                }

                            break;

                        case "Collision":

                            switch (propArr[1])
                            {
                                case "Box":
                                    localColl = CollisionType.Box;
                                    break;

                                case "Capsule":
                                    localColl = CollisionType.Capsule;
                                    break;

                                case "Circle":
                                    localColl = CollisionType.Circle;
                                    break;

                            }
                            break;

                        case "CollisionSize":

                            propArr = propArr[1].Split('|');

                            for (int iii = 0; iii < propArr.Length && iii < 2; ++iii)
                                switch (iii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localCollisionSize.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localCollisionSize.y = localF;
                                        break;

                                }

                            break;

                        default:
                            break;
                    }


                }

                GameObject localWall = new GameObject();

                localWall.layer = 9;

                localWall.transform.position = localPos;
                localWall.transform.rotation = Quaternion.Euler(0, 0, localRot);
                localWall.transform.localScale = localScale;

                SpriteRenderer lSR = localWall.AddComponent<SpriteRenderer>();
                lSR.sprite = localSprite;
                lSR.color = localColor;

                switch (localColl)
                {
                    case CollisionType.Circle:
                        CircleCollider2D localColliderCircle = localWall.AddComponent<CircleCollider2D>();
                        if (localCollisionSize.x > 0)
                            localColliderCircle.radius = localCollisionSize.x;


                        break;
                    case CollisionType.Box:
                        BoxCollider2D localColliderBox = localWall.AddComponent<BoxCollider2D>();

                        Vector2 localSize = localColliderBox.size;
                        if (localCollisionSize.x > 0)
                            localSize.x = localCollisionSize.x;

                        if (localCollisionSize.y > 0)
                            localSize.y = localCollisionSize.y;

                        localColliderBox.size = localSize;

                        break;
                    case CollisionType.Capsule:
                        localWall.AddComponent<CapsuleCollider2D>();
                        break;
                    default:
                        break;
                }

                localWall.transform.parent = InWallsObj.transform;
            }

            return true;
        }

        private bool Change_OutWalls(string inString)
        {
            Color totalColor = Color.white;
            CollisionType totalColl = CollisionType.Box;
            Vector2 totalCollisionSize = new Vector2(-1f, -1f);
            Vector3 totalPos = new Vector3();
            float totalRot = 0f;
            Vector3 totalScale = Vector3.one;
            Sprite totalSprite = Resources.Load<Sprite>("S_SimpleBox");

            string[] localStrings = inString.Split(':');

            if (localStrings.Length != 2)
                return false;

            string[] localArr = new string[1];
            string[] propArr;

            float localF;
            int localI;

            if (localStrings[0].IndexOf('(') != -1 && localStrings[0].IndexOf('(') < localStrings[0].IndexOf(')'))
            {
                localArr[0] = localStrings[0].Substring(localStrings[0].IndexOf('(') + 1, localStrings[0].IndexOf(')') - localStrings[0].IndexOf('(') - 1);

                localArr = localArr[0].Split(',');

                for (int i = 0; i < localArr.Length; ++i)
                {
                    propArr = localArr[i].Split('=');

                    if (propArr.Length != 2)
                        continue;

                    switch (propArr[0])
                    {
                        case "Pos":
                            propArr = propArr[1].Split('|');

                            for (int ii = 0; ii < propArr.Length && ii < 3; ++ii)
                                switch (ii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalPos.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalPos.y = localF;
                                        break;
                                    case 2:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalPos.z = localF;
                                        break;
                                }

                            break;

                        case "Rot":

                            if (float.TryParse(propArr[1], out localF))
                                totalRot = localF;

                            break;

                        case "Scale":

                            propArr = propArr[1].Split('|');

                            for (int ii = 0; ii < propArr.Length && ii < 3; ++ii)
                                switch (ii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalScale.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalScale.y = localF;
                                        break;
                                }

                            break;

                        case "Sprite":

                            if (int.TryParse(propArr[1], out localI))
                            {
                                Sprite localOne = mySprites[localI];
                                if (localOne != null)
                                    totalSprite = localOne;
                            }

                            break;

                        case "Color":

                            propArr = propArr[1].Split('|');

                            for (int ii = 0; ii < propArr.Length && ii < 4; ++ii)
                                switch (ii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalColor.r = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalColor.g = localF;
                                        break;
                                    case 2:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalColor.b = localF;
                                        break;
                                    case 3:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalColor.a = localF;
                                        break;
                                }

                            break;

                        case "Collision":

                            switch (propArr[1])
                            {
                                case "Box":
                                    totalColl = CollisionType.Box;
                                    break;

                                case "Capsule":
                                    totalColl = CollisionType.Capsule;
                                    break;

                                case "Circle":
                                    totalColl = CollisionType.Circle;
                                    break;

                            }
                            break;

                        case "CollisionSize":

                            propArr = propArr[1].Split('|');

                            for (int ii = 0; ii < propArr.Length && ii < 2; ++ii)
                                switch (ii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalCollisionSize.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[ii], out localF))
                                            totalCollisionSize.y = localF;
                                        break;

                                }

                            break;

                        default:
                            break;
                    }

                }


            }

            localStrings = localStrings[1].Split('[');

            for (int i = 0; i < localStrings.Length; ++i)
            {
                Color localColor = totalColor;
                CollisionType localColl = totalColl;
                Vector2 localCollisionSize = totalCollisionSize;
                Vector3 localPos = totalPos;
                float localRot = totalRot;
                Vector3 localScale = totalScale;
                Sprite localSprite = totalSprite;

                if (localStrings[i].IndexOf('(') == -1 || localStrings[i].IndexOf(')') - localStrings[i].IndexOf('(') < 2)
                    continue;

                localArr[0] = localStrings[i].Substring(localStrings[i].IndexOf('(') + 1, localStrings[i].IndexOf(')') - localStrings[i].IndexOf('(') - 1);

                localArr = localArr[0].Split(',');

                for (int ii = 0; ii < localArr.Length; ++ii)
                {
                    propArr = localArr[ii].Split('=');

                    if (propArr.Length != 2)
                        continue;

                    switch (propArr[0])
                    {
                        case "Pos":
                            propArr = propArr[1].Split('|');

                            for (int iii = 0; iii < propArr.Length && iii < 3; ++iii)
                                switch (iii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localPos.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localPos.y = localF;
                                        break;
                                    case 2:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localPos.z = localF;
                                        break;
                                }

                            break;

                        case "Rot":

                            if (float.TryParse(propArr[1], out localF))
                                localRot = localF;

                            break;

                        case "Scale":

                            propArr = propArr[1].Split('|');

                            for (int iii = 0; iii < propArr.Length && iii < 3; ++iii)
                                switch (iii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localScale.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localScale.y = localF;
                                        break;
                                }

                            break;

                        case "Sprite":

                            if (int.TryParse(propArr[1], out localI))
                            {
                                Sprite localOne = mySprites[localI];
                                if (localOne != null)
                                    localSprite = localOne;
                            }

                            break;

                        case "Color":

                            propArr = propArr[1].Split('|');

                            for (int iii = 0; iii < propArr.Length && iii < 4; ++iii)
                                switch (iii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localColor.r = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localColor.g = localF;
                                        break;
                                    case 2:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localColor.b = localF;
                                        break;
                                    case 3:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localColor.a = localF;
                                        break;
                                }

                            break;

                        case "Collision":

                            switch (propArr[1])
                            {
                                case "Box":
                                    localColl = CollisionType.Box;
                                    break;

                                case "Capsule":
                                    localColl = CollisionType.Capsule;
                                    break;

                                case "Circle":
                                    localColl = CollisionType.Circle;
                                    break;

                            }
                            break;

                        case "CollisionSize":

                            propArr = propArr[1].Split('|');

                            for (int iii = 0; iii < propArr.Length && iii < 2; ++iii)
                                switch (iii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localCollisionSize.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localCollisionSize.y = localF;
                                        break;

                                }
                            break;
                        default:
                            break;
                    }


                }

                GameObject localWall = new GameObject();

                localWall.layer = 8;

                localWall.transform.position = localPos;
                localWall.transform.rotation = Quaternion.Euler(0, 0, localRot);
                localWall.transform.localScale = localScale;

                SpriteRenderer lSR = localWall.AddComponent<SpriteRenderer>();
                lSR.sprite = localSprite;
                lSR.color = localColor;

                switch (localColl)
                {
                    case CollisionType.Circle:
                        CircleCollider2D localColliderCircle = localWall.AddComponent<CircleCollider2D>();
                        if (localCollisionSize.x > 0)
                            localColliderCircle.radius = localCollisionSize.x;


                        break;
                    case CollisionType.Box:
                        BoxCollider2D localColliderBox = localWall.AddComponent<BoxCollider2D>();

                        Vector2 localSize = localColliderBox.size;
                        if (localCollisionSize.x > 0)
                            localSize.x = localCollisionSize.x;

                        if (localCollisionSize.y > 0)
                            localSize.y = localCollisionSize.y;

                        localColliderBox.size = localSize;

                        break;
                    case CollisionType.Capsule:
                        localWall.AddComponent<CapsuleCollider2D>();
                        break;
                    default:
                        break;
                }

                localWall.transform.parent = OutWallsObj.transform;
            }

            return true;

        }

        private bool Change_SideBall(string inString)
        {
            string[] localStrings = inString.Split(':');

            if (localStrings.Length != 2)
                return false;

            if (localStrings[1].IndexOf('(') == -1 || localStrings[1].IndexOf(')') <= localStrings[1].IndexOf('('))
                return false;

            localStrings[1] = localStrings[1].Substring(localStrings[1].IndexOf('(') + 1, localStrings[1].IndexOf(')') - localStrings[1].IndexOf('(') - 1);

            localStrings = localStrings[1].Split(',');

            string[] propArr;

            for (int i = 0; i < localStrings.Length; ++i)
            {
                propArr = localStrings[i].Split('=');

                if (propArr.Length != 2)
                    continue;

                float localF;
                int localI;
                SpriteRenderer localSR;

                switch (propArr[0])
                {
                    case "Color":
                        localSR = SideBallSample.GetComponent<SpriteRenderer>();
                        if (localSR == null)
                            break;

                        Color localColor = localSR.color;

                        propArr = propArr[1].Split('|');

                        for (int ii = 0; ii < propArr.Length && ii < 4; ++ii)
                            switch (ii)
                            {
                                case 0:
                                    if (float.TryParse(propArr[ii], out localF))
                                        localColor.r = localF;
                                    break;
                                case 1:
                                    if (float.TryParse(propArr[ii], out localF))
                                        localColor.g = localF;
                                    break;
                                case 2:
                                    if (float.TryParse(propArr[ii], out localF))
                                        localColor.b = localF;
                                    break;
                                case 3:
                                    if (float.TryParse(propArr[ii], out localF))
                                        localColor.a = localF;
                                    break;
                            }

                        localSR.color = localColor;

                        break;

                    case "StartSpeed":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        SideBallSample.Speed = localF;

                        break;

                    case "Sprite":

                        localSR = SideBallSample.GetComponent<SpriteRenderer>();
                        if (localSR == null)
                            break;

                        if (!int.TryParse(propArr[1], out localI))
                            break;

                        Sprite localS = mySprites[localI];
                        if (localS != null)
                        {
                            localSR.sprite = localS;
                        }

                        break;

                    default:
                        break;
                }

            }

            return true;
        }

        private bool Change_SmallBall(string inString)
        {
            string[] localStrings = inString.Split(':');

            if (localStrings.Length != 2)
                return false;

            if (localStrings[1].IndexOf('(') == -1 || localStrings[1].IndexOf(')') <= localStrings[1].IndexOf('('))
                return false;

            localStrings[1] = localStrings[1].Substring(localStrings[1].IndexOf('(') + 1, localStrings[1].IndexOf(')') - localStrings[1].IndexOf('(') - 1);

            localStrings = localStrings[1].Split(',');

            string[] propArr;

            for (int i = 0; i < localStrings.Length; ++i)
            {
                propArr = localStrings[i].Split('=');

                if (propArr.Length != 2)
                    continue;

                float localF;
                int localI;
                SpriteRenderer localSR;

                switch (propArr[0])
                {
                    case "Color":
                        localSR = SmallBallSample.GetComponent<SpriteRenderer>();
                        if (localSR == null)
                            break;

                        Color localColor = localSR.color;

                        propArr = propArr[1].Split('|');

                        for (int ii = 0; ii < propArr.Length && ii < 4; ++ii)
                            switch (ii)
                            {
                                case 0:
                                    if (float.TryParse(propArr[ii], out localF))
                                        localColor.r = localF;
                                    break;
                                case 1:
                                    if (float.TryParse(propArr[ii], out localF))
                                        localColor.g = localF;
                                    break;
                                case 2:
                                    if (float.TryParse(propArr[ii], out localF))
                                        localColor.b = localF;
                                    break;
                                case 3:
                                    if (float.TryParse(propArr[ii], out localF))
                                        localColor.a = localF;
                                    break;
                            }

                        localSR.color = localColor;

                        break;

                    case "StartSpeed":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        SmallBallSample.Speed = localF;

                        break;

                    case "Sprite":

                        localSR = SmallBallSample.GetComponent<SpriteRenderer>();
                        if (localSR == null)
                            break;

                        if (!int.TryParse(propArr[1], out localI))
                            break;

                        Sprite localS = mySprites[localI];
                        if (localS != null)
                        {
                            localSR.sprite = localS;
                        }

                        break;

                    case "SpeedHitChange":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        SmallBallSample.SpeedChanging = localF;

                        break;

                    case "Reflects":

                        if (!int.TryParse(propArr[1], out localI))
                            break;

                        SmallBallSample.MaxTouches = localI;

                        break;

                    default:
                        break;
                }

            }

            return true;
        }

        private bool Change_MainBalls(string inString)
        {

            string[] localStrings = inString.Split(':');

            if (localStrings.Length != 2)
                return false;

            string[] localArr = new string[1];
            string[] propArr;

            float localF;
            int localI;
            Vector3 localVector;
            Color localColor;

            SpriteRenderer localSR = MBSample.GetComponent<SpriteRenderer>();

            if (localStrings[0].IndexOf('(') != -1 && localStrings[1].IndexOf('(') < localStrings[1].IndexOf(')'))
            {
                localArr[0] = localStrings[1].Substring(localStrings[1].IndexOf('(') + 1, localStrings[1].IndexOf(')') - localStrings[1].IndexOf('(') - 1);

                localArr = localArr[0].Split(',');

                for (int i = 0; i < localArr.Length; ++i)
                {
                    propArr = localArr[i].Split('=');

                    if (propArr.Length != 2)
                        continue;

                    switch (propArr[0])
                    {
                        case "SpawnPoint":
                            propArr = propArr[1].Split('|');

                            localVector = MBStart.transform.position;

                            for (int ii = 0; ii < propArr.Length && ii < 3; ++ii)
                                switch (ii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[ii], out localF))
                                            localVector.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[ii], out localF))
                                            localVector.y = localF;
                                        break;
                                    case 2:
                                        if (float.TryParse(propArr[ii], out localF))
                                            localVector.z = localF;
                                        break;
                                }

                            MBStart.transform.position = localVector;

                            break;

                        case "StartRot":

                            if (float.TryParse(propArr[1], out localF))
                                MBStart.transform.rotation = Quaternion.Euler(0, 0, localF);

                            break;

                        case "Scale":

                            localVector = MBSample.transform.localScale;

                            propArr = propArr[1].Split('|');

                            for (int ii = 0; ii < propArr.Length && ii < 3; ++ii)
                                switch (ii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[ii], out localF))
                                            localVector.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[ii], out localF))
                                            localVector.y = localF;
                                        break;
                                }

                            MBSample.transform.localScale = localVector;

                            break;

                        case "Sprite":

                            if (int.TryParse(propArr[1], out localI))
                            {
                                Sprite localOne = mySprites[localI];
                                if (localOne != null && localSR != null)
                                    localSR.sprite = localOne;
                            }

                            break;

                        case "Color":

                            if (localSR == null)
                                break;

                            localColor = localSR.color;

                            propArr = propArr[1].Split('|');

                            for (int ii = 0; ii < propArr.Length && ii < 4; ++ii)
                                switch (ii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[ii], out localF))
                                            localColor.r = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[ii], out localF))
                                            localColor.g = localF;
                                        break;
                                    case 2:
                                        if (float.TryParse(propArr[ii], out localF))
                                            localColor.b = localF;
                                        break;
                                    case 3:
                                        if (float.TryParse(propArr[ii], out localF))
                                            localColor.a = localF;
                                        break;
                                }

                            localSR.color = localColor;

                            break;

                        case "StartSpeed":

                            if (float.TryParse(propArr[1], out localF))
                                MBSample.Speed = localF;
                            break;

                        case "SpeedHitChange":

                            if (float.TryParse(propArr[1], out localF))
                                MBSample.SpeedChanging = localF;
                            break;

                        case "HitsAvarage":

                            if (int.TryParse(propArr[1], out localI))
                                MBSample.AvarageHitNumber = localI;
                            break;

                        case "HitsDelta":

                            if (int.TryParse(propArr[1], out localI))
                                MBSample.RandomRange = localI;
                            break;

                        case "SmallBalls":

                            if (int.TryParse(propArr[1], out localI))
                                MBSample.NumberOfSB = localI;
                            break;

                        case "ExpAngle":

                            if (float.TryParse(propArr[1], out localF))
                                MBSample.ExploadAngle = localF;
                            break;

                        case "CollisionRadius":

                            if (float.TryParse(propArr[1], out localF) && localF > 0)
                                MBSample.ChangeCollisionRadius(localF);

                            break;

                        default:
                            break;
                    }



                }


            }

            localStrings = localStrings[1].Split('[');

            for (int i = 0; i < localStrings.Length; ++i)
            {

                if (localStrings[i].IndexOf('(') == -1 || localStrings[i].IndexOf(')') - localStrings[i].IndexOf('(') < 2)
                    continue;

                localArr[0] = localStrings[i].Substring(localStrings[i].IndexOf('(') + 1, localStrings[i].IndexOf(')') - localStrings[i].IndexOf('(') - 1);

                localArr = localArr[0].Split(',');

                GameObject localStartPoint = new GameObject();
                localStartPoint.transform.position = MBStart.transform.position;
                localStartPoint.transform.rotation = MBStart.transform.rotation;

                MainBall localMB = Instantiate<MainBall>(MBSample);

                localSR = localMB.GetComponent<SpriteRenderer>();

                localMB.StartPos = localStartPoint.transform;

                for (int ii = 0; ii < localArr.Length; ++ii)
                {
                    propArr = localArr[ii].Split('=');

                    if (propArr.Length != 2)
                        continue;

                    switch (propArr[0])
                    {
                        case "SpawnPoint":
                            propArr = propArr[1].Split('|');

                            localVector = localStartPoint.transform.position;

                            for (int iii = 0; iii < propArr.Length && iii < 3; ++iii)
                                switch (iii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localVector.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localVector.y = localF;
                                        break;
                                    case 2:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localVector.z = localF;
                                        break;
                                }

                            localStartPoint.transform.position = localVector;

                            break;

                        case "StartRot":

                            if (float.TryParse(propArr[1], out localF))
                                localStartPoint.transform.rotation = Quaternion.Euler(0, 0, localF);

                            break;

                        case "Scale":

                            localVector = localMB.transform.localScale;

                            propArr = propArr[1].Split('|');

                            for (int iii = 0; iii < propArr.Length && iii < 3; ++iii)
                                switch (iii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localVector.x = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localVector.y = localF;
                                        break;
                                }

                            localMB.transform.localScale = localVector;

                            break;

                        case "Sprite":

                            if (int.TryParse(propArr[1], out localI))
                            {
                                print("before problem : " + mySprites.Count);
                                Sprite localOne = mySprites[localI];
                                if (localOne != null && localSR != null)
                                    localSR.sprite = localOne;

                                print("After problem");
                            }

                            break;

                        case "Color":

                            if (localSR == null)
                                break;

                            localColor = localSR.color;

                            propArr = propArr[1].Split('|');

                            for (int iii = 0; iii < propArr.Length && iii < 4; ++iii)
                                switch (iii)
                                {
                                    case 0:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localColor.r = localF;
                                        break;
                                    case 1:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localColor.g = localF;
                                        break;
                                    case 2:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localColor.b = localF;
                                        break;
                                    case 3:
                                        if (float.TryParse(propArr[iii], out localF))
                                            localColor.a = localF;
                                        break;
                                }

                            localSR.color = localColor;

                            break;

                        case "StartSpeed":

                            if (float.TryParse(propArr[1], out localF))
                                localMB.Speed = localF;
                            break;

                        case "SpeedHitChange":

                            if (float.TryParse(propArr[1], out localF))
                                localMB.SpeedChanging = localF;
                            break;

                        case "HitsAvarage":

                            if (int.TryParse(propArr[1], out localI))
                                localMB.AvarageHitNumber = localI;
                            break;

                        case "HitsDelta":

                            if (int.TryParse(propArr[1], out localI))
                                localMB.RandomRange = localI;
                            break;

                        case "SmallBalls":

                            if (int.TryParse(propArr[1], out localI))
                                localMB.NumberOfSB = localI;
                            break;

                        case "ExpAngle":

                            if (float.TryParse(propArr[1], out localF))
                                localMB.ExploadAngle = localF;
                            break;

                        case "CollisionRadius":

                            if (float.TryParse(propArr[1], out localF) && localF > 0)
                                localMB.ChangeCollisionRadius(localF);

                            break;

                        default:
                            break;
                    }

                }
                localMB.gameObject.SetActive(true);

            }

            return true;

        }

        private bool Change_Player(string inString)
        {
            string[] localStrings = inString.Split(':');

            if (localStrings.Length != 2)
                return false;

            if (localStrings[1].IndexOf('(') == -1 || localStrings[1].IndexOf(')') <= localStrings[1].IndexOf('('))
                return false;

            localStrings[1] = localStrings[1].Substring(localStrings[1].IndexOf('(') + 1, localStrings[1].IndexOf(')') - localStrings[1].IndexOf('(') - 1);

            localStrings = localStrings[1].Split(',');

            string[] propArr;

            for (int i = 0; i < localStrings.Length; ++i)
            {
                propArr = localStrings[i].Split('=');

                if (propArr.Length != 2)
                    continue;

                float localF;
                int localI;

                switch (propArr[0])
                {
                    case "SpawnPoint":
                        Vector2 localVector = PlayerStart.transform.position;

                        propArr = propArr[1].Split('|');

                        for (int ii = 0; ii < propArr.Length && ii < 2; ++ii)
                            switch (ii)
                            {
                                case 0:
                                    if (float.TryParse(propArr[ii], out localF))
                                        localVector.x = localF;
                                    break;
                                case 1:
                                    if (float.TryParse(propArr[ii], out localF))
                                        localVector.y = localF;
                                    break;

                            }
                        PlayerStart.transform.position = localVector;

                        break;

                    case "HP":

                        if (!int.TryParse(propArr[1], out localI))
                            break;

                        myPlayer.StartHP = localI;

                        break;

                    case "InvulDamageTime":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        myPlayer.InvulTimeOnDamage = localF;

                        break;

                    case "InvulHitTime":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        myPlayer.InvulTimeAfterHit = localF;

                        break;

                    case "HitReloadTime":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        myPlayer.HitReloadTime = localF;

                        break;

                    case "HitRadius":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        myPlayer.HitRadius = localF;

                        break;

                    case "Sprite":

                        break;





                    default:
                        break;
                }

            }

            return true;
        }

        private bool Change_MapSettings(string inString)
        {
            string[] localStrings = inString.Split(':');

            if (localStrings.Length != 2)
                return false;

            if (localStrings[1].IndexOf('(') == -1 || localStrings[1].IndexOf(')') <= localStrings[1].IndexOf('('))
                return false;

            localStrings[1] = localStrings[1].Substring(localStrings[1].IndexOf('(') + 1, localStrings[1].IndexOf(')') - localStrings[1].IndexOf('(') - 1);

            localStrings = localStrings[1].Split(',');

            string[] propArr;

            for (int i = 0; i < localStrings.Length; ++i)
            {
                propArr = localStrings[i].Split('=');

                if (propArr.Length != 2)
                    continue;

                float localF;
                int localI;

                switch (propArr[0])
                {
                    case "BackgroundColor":

                        Color localColor = BackgroundCamera.backgroundColor;

                        propArr = propArr[1].Split('|');

                        for (int ii = 0; ii < propArr.Length && ii < 4; ++ii)
                            switch (ii)
                            {
                                case 0:
                                    if (float.TryParse(propArr[ii], out localF))
                                        localColor.r = localF;
                                    break;
                                case 1:
                                    if (float.TryParse(propArr[ii], out localF))
                                        localColor.g = localF;
                                    break;
                                case 2:
                                    if (float.TryParse(propArr[ii], out localF))
                                        localColor.b = localF;
                                    break;
                                case 3:
                                    if (float.TryParse(propArr[ii], out localF))
                                        localColor.a = localF;
                                    break;
                            }

                        BackgroundCamera.backgroundColor = localColor;

                        break;

                    case "PointsMultiplyer":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        myGameMan.ScoreMultiplyer = localF;

                        break;

                    case "TimeSpeed":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        myGameMan.TimeSpeed = localF;

                        break;

                    case "SideBallStartSpawn":

                        if (!int.TryParse(propArr[1], out localI))
                            break;

                        for (int ii = 0; ii < myDataPool.ObjectSamples.Length; ++ii)
                            if (myDataPool.ObjectSamples[ii].ObjType == DataPool.E_ObjectType.SideBall)
                            {
                                myDataPool.ObjectSamples[ii].SpawnOnStart = localI;
                                break;
                            }

                        break;

                    case "SmallBallStartSpawn":

                        if (!int.TryParse(propArr[1], out localI))
                            break;

                        for (int ii = 0; ii < myDataPool.ObjectSamples.Length; ++ii)
                            if (myDataPool.ObjectSamples[ii].ObjType == DataPool.E_ObjectType.SmallBall)
                            {
                                myDataPool.ObjectSamples[ii].SpawnOnStart = localI;
                                break;
                            }

                        break;

                    default:
                        break;
                }

            }

            return true;
        }

        private bool Change_Camera(string inString)
        {
            string[] localStrings = inString.Split(':');

            if (localStrings.Length != 2)
                return false;

            if (localStrings[1].IndexOf('(') == -1 || localStrings[1].IndexOf(')') <= localStrings[1].IndexOf('('))
                return false;

            localStrings[1] = localStrings[1].Substring(localStrings[1].IndexOf('(') + 1, localStrings[1].IndexOf(')') - localStrings[1].IndexOf('(') - 1);

            localStrings = localStrings[1].Split(',');

            string[] propArr;

            for (int i = 0; i < localStrings.Length; ++i)
            {
                propArr = localStrings[i].Split('=');

                if (propArr.Length != 2)
                    continue;

                float localF;
                bool localB;

                switch (propArr[0])
                {


                    case "Size":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        MainCamera.orthographicSize = localF;

                        break;

                    case "SnapToPlayer":

                        if (!bool.TryParse(propArr[1], out localB))
                            break;

                        if (localB)
                        {
                            MainCamera.transform.position = new Vector3(myPlayer.transform.position.x, myPlayer.transform.position.y, -10);
                            MainCamera.transform.parent = myPlayer.transform;
                        }
                        else
                        {
                            MainCamera.transform.parent = null;
                            MainCamera.transform.position = new Vector3(0, 0, -10);
                        }


                        break;


                    default:
                        break;
                }

            }

            return true;
        }

        private bool Change_Events(string inString)
        {
            string[] stringLines = inString.Split(':');

            if (stringLines.Length != 2)
                return false;

            string[] localArr;

            string[] propArr;

            float localF;
            int localI;
            if (stringLines[0].IndexOf('(') != -1 && stringLines[1].IndexOf('(') < stringLines[1].IndexOf(')'))
            {
                stringLines[0] = stringLines[0].Substring(stringLines[0].IndexOf('(') + 1, stringLines[0].IndexOf(')') - stringLines[0].IndexOf('(') - 1);

                localArr = stringLines[0].Split(',');

                for (int i = 0; i < localArr.Length; ++i)
                {
                    propArr = localArr[i].Split('=');

                    if (propArr.Length != 2)
                        continue;

                    switch (propArr[0])
                    {
                        case "AverageTime":
                            if (float.TryParse(propArr[1], out localF))
                                myGameMan.AvarageEventTime = localF;
                            break;

                        case "DeltaTime":
                            if (float.TryParse(propArr[1], out localF))
                                myGameMan.DeltaEventTime = localF;
                            break;


                        default:
                            break;
                    }
                }
            }

            stringLines = stringLines[1].Split('[');

            List<GameEvent> localGEList = new List<GameEvent>(stringLines.Length);

            for (int i = 0; i < stringLines.Length; ++i)
            {
                if (stringLines[i].IndexOf('(') == -1 && stringLines[i].IndexOf('(') >= stringLines[i].IndexOf(')'))
                    continue;

                stringLines[i] = stringLines[i].Substring(stringLines[i].IndexOf('(') + 1, stringLines[i].IndexOf(')') - stringLines[i].IndexOf('(') - 1);

                localArr = stringLines[i].Split(',');

                for (int ii = 0; ii < localArr.Length; ++ii)
                {
                    propArr = localArr[ii].Split('=');
                    if (propArr.Length != 2)
                        continue;

                    if (propArr[0] == "EventType")
                    {
                        switch (propArr[1])
                        {
                            case "Line":
                                GameEvent_Line localE_line = myGameMan.gameObject.AddComponent<GameEvent_Line>();

                                localE_line.HUD_TopLeft = TopLeftHE;
                                localE_line.HUD_BottomLeft = BottomLeftHE;
                                localE_line.HUD_TopRight = TopRightHE;
                                localE_line.HUD_BottomRight = BottomRightHE;

                                for (int iii = 0; iii < localArr.Length; ++iii)
                                {
                                    propArr = localArr[iii].Split('=');
                                    if (propArr.Length != 2)
                                        continue;

                                    switch (propArr[0])
                                    {

                                        case "Num":

                                            if (int.TryParse(propArr[1], out localI))
                                                localE_line.NumOfBalls = localI;

                                            break;

                                        case "a":

                                            if (float.TryParse(propArr[1], out localF))
                                                localE_line.aLength = localF;

                                            break;

                                        case "b":

                                            if (float.TryParse(propArr[1], out localF))
                                                localE_line.bLength = localF;

                                            break;

                                        default:
                                            break;
                                    }
                                }

                                localGEList.Add(localE_line);

                                break;

                            case "Slide":

                                GameEvent_Slide localE_slide = myGameMan.gameObject.AddComponent<GameEvent_Slide>();

                                localE_slide.HUD_Left = LeftHE;
                                localE_slide.HUD_Right = RightHE;
                                localE_slide.HUD_Top = TopHE;
                                localE_slide.HUD_Bottom = BottomHE;

                                for (int iii = 0; iii < localArr.Length; ++iii)
                                {
                                    propArr = localArr[iii].Split('=');
                                    if (propArr.Length != 2)
                                        continue;

                                    switch (propArr[0])
                                    {

                                        case "Num":

                                            if (int.TryParse(propArr[1], out localI))
                                                localE_slide.NumOfBalls = localI;

                                            break;

                                        case "a":

                                            if (float.TryParse(propArr[1], out localF))
                                                localE_slide.aLength = localF;

                                            break;

                                        case "b":

                                            if (float.TryParse(propArr[1], out localF))
                                                localE_slide.bLength = localF;

                                            break;

                                        case "c":

                                            if (float.TryParse(propArr[1], out localF))
                                                localE_slide.cLength = localF;

                                            break;

                                        default:
                                            break;
                                    }
                                }

                                localGEList.Add(localE_slide);

                                break;

                            case "Circle":

                                GameEvent_Circle localE_circle = myGameMan.gameObject.AddComponent<GameEvent_Circle>();

                                localE_circle.HUD_Element = CircleHE;

                                for (int iii = 0; iii < localArr.Length; ++iii)
                                {
                                    propArr = localArr[iii].Split('=');
                                    if (propArr.Length != 2)
                                        continue;

                                    switch (propArr[0])
                                    {

                                        case "Num":

                                            if (int.TryParse(propArr[1], out localI))
                                                localE_circle.NumOfBalls = localI;

                                            break;

                                        case "a":

                                            if (float.TryParse(propArr[1], out localF))
                                                localE_circle.aLength = localF;

                                            break;

                                        default:
                                            break;
                                    }
                                }

                                localGEList.Add(localE_circle);

                                break;

                            default:
                                break;
                        }


                        break;
                    }


                }


            }

            myGameMan.MyEvents = localGEList.ToArray();

            return true;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}