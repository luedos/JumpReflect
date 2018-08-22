using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LevelCreator
{
    public enum E_LevelObjType
    {
        Wall,
        MainBall,
        Player
    }
    [System.Flags]
    public enum E_MenuType
    {
        BlanksMenu = 0x01,
        ObjectsMenu = 0x02,
        Textures = 0x04,
        OnFieldMenu = 0x08,
        All = BlanksMenu | ObjectsMenu | Textures | OnFieldMenu
    }

    [System.Serializable]
    public struct TextureData
    {
        public TextureData(Texture2D inTex, string inPath, string inHash)
        {
            myTexture = inTex;
            Path = inPath;
            Hash = inHash;
        }

        public Texture2D myTexture;
        public string Path;
        public string Hash;
    }



    class BlankCopyData
    {
        public Blank myBlank;
        public Vector3 pos;
    }
    [System.Serializable]
    public class EventsData
    {
        public enum E_EventType
        {
            Line,
            Slide,
            Circle
        }
        public E_EventType myType = E_EventType.Line;
        public float a = 15f;
        public float b = 5f;
        public float c = 10f;
        public int Num = 8;
    }

    public class LevelCreatorManager : MonoBehaviour, ITextureDataHolder
    {

        protected struct SpriteData
        {
            public int TextureIndex;
            public float PPU;
            public Vector2 Pivot;

            public SpriteData(int TextureID, float inPPU, Vector2 inPivot)
            {
                TextureIndex = TextureID;
                PPU = inPPU;
                Pivot = inPivot;
            }
        }

        public enum E_Answer
        {
            None,
            OK,
            Cansel
        }

        // level prop variables
        // Level props
        [HideInInspector] public string LevelName = "New level";
        [HideInInspector] public TextureData LevelTexture;
        // Map settings
        [HideInInspector] public float PointsMultiplyer = 1f;
        [HideInInspector] public float TimeSpeed = 1f;
        [HideInInspector] public int SideBallStartSpawn = 10;
        [HideInInspector] public int SmallBallStartSpawn = 3;
        // Camera
        public void ChangeCameraSize(float inSize) { CameraBorder.transform.localScale = new Vector3(inSize, inSize, 1); }
        [HideInInspector] public bool SnapCameraToPlayer = false;
        // Events
        [HideInInspector] public float AverageTime = 45f;
        [HideInInspector] public float DeltaTime = 15f;
        [HideInInspector] public List<EventsData> myEvents = new List<EventsData>();
        // Balls
        [HideInInspector] public float SmallBallStartSpeed = 8f;
        [HideInInspector] public float SmallBallSpeedHitChange = 1.1f;
        [HideInInspector] public int SmallBallReflects = 3;
        [HideInInspector] public float SideBallStartSpeed = 5f;


        public Blank_MB MainBallSample;
        public Blank_Wall WallSample;

        public Blank_Player myPlayer;

        public ObjectsMenu myObjectMenu;
        public WallMenu myWallMenu;
        public MBMenu myMBMenu;
        public MainPlayerMenu myMPMenu;
        public Assets.SimpleColorPicker.Scripts.ColorPicker myColorPicker;
        public BlanksMenu myBlanksMenu;

        List<Blank> ActiveBlanks = new List<Blank>();
        List<BlankCopyData> OnCopyList = new List<BlankCopyData>();


        public List<Blank_MB> MBList = new List<Blank_MB>();
        public List<Blank_Wall> WallList = new List<Blank_Wall>();
        //public Dictionary<string, Texture2D> myTexDic = new Dictionary<string, Texture2D>();
        public List<TextureData> myTextures = new List<TextureData>();


        public GameObject CameraBorder;
        public Rect cameraWorldRect;
        Camera myCamera;
        bool CameraDrag = false;
        Vector3 StartWorldCameraPos;
        Vector2 StartScreenMousePos;


        [Header("Saving staff")]
        public GameObject TextureNotFoundQuestion;
        public static string SavePath;
        public static string FileName;
        public E_Answer myAnswer = E_Answer.None;
        public FolderChooser myFC;

        UnityEngine.Events.UnityEvent EventAfterSave = new UnityEngine.Events.UnityEvent();

        private Dictionary<int, TextureData> LoadingTextures = new Dictionary<int, TextureData>();
        private Dictionary<int, SpriteData> LoadingSprites = new Dictionary<int, SpriteData>();

        public void AnswerQuestion(int inAnswer)
        {
            myAnswer = (E_Answer)inAnswer;
        }

        public static LevelCreatorManager Instance
        { get { return instance; } }

        static LevelCreatorManager instance;

        private void Start()
        {
            if (instance != null)
                if (instance == this)
                    return;
                else
                {
                    Destroy(this);
                    return;
                }



            instance = this;

            myCamera = Camera.main;

            if (!string.IsNullOrEmpty(FileName) && System.IO.Path.GetExtension(FileName) == ".jrl")
            {
                string localPath = (string.IsNullOrEmpty(SavePath) ? "" : SavePath + @"\") + FileName;
                if (System.IO.File.Exists(localPath))
                    DirectLoad();
            }           
            

        }

        private void ReloadAll()
        {

            ActiveBlanks.Clear();

            for (int i = 0; i < OnCopyList.Count; ++i)
            {
                if (WallList[i].gameObject.activeSelf)
                {
                    Destroy(OnCopyList[i].myBlank.gameObject);
                    OnCopyList.RemoveAt(i);
                    i--;
                }

            }

            // Walls
            for (int i = 0; i < WallList.Count; ++i)
            {
                if (WallList[i].gameObject.activeSelf)
                {
                    Destroy(WallList[i].gameObject);
                    WallList.RemoveAt(i);
                    i--;
                }

            }
            // MBs
            for (int i = 0; i < MBList.Count; ++i)
            {
                if (MBList[i].gameObject.activeSelf)
                {
                    Destroy(MBList[i].gameObject);
                    MBList.RemoveAt(i);
                    i--;
                }

            }

            // Player

            myPlayer.transform.position = Vector3.zero;

            myPlayer.PlayerHP = 3;
            myPlayer.InvulDamageTime = 1.5f;
            myPlayer.InvulHitTime = 0.7f;
            myPlayer.HitReloadTime = 0.8f;
            myPlayer.HitRadius = 0.5f;

            // Events

            myEvents.Clear();

            // Balls

            SmallBallStartSpeed = 8f;
            SmallBallSpeedHitChange = 1.1f;
            SmallBallReflects = 3;
            SideBallStartSpeed = 5f;

            myObjectMenu.SideBallBlank.SetColor(new Color(163f / 255f, 145f / 255f, 0f));

            myObjectMenu.SmallBallBlank.SetColor(new Color(163f / 255f, 145f / 255f, 0f));

            myObjectMenu.SideBallBlank.SetTextureData(new TextureData(Resources.Load<Texture2D>("S_SimpleCircle"), "", "2"));

            myObjectMenu.SmallBallBlank.SetTextureData(new TextureData(Resources.Load<Texture2D>("S_SimpleCircle"), "", "2"));

            // Map settings

            PointsMultiplyer = 1f;
            TimeSpeed = 1f;
            SideBallStartSpawn = 10;
            SmallBallStartSpawn = 3;

            myCamera.backgroundColor = new Color(0.8f,0.8f,0.8f);

            // Camera
            CameraBorder.transform.localScale = new Vector3(5.3f, 5.3f, 1f);

            SnapCameraToPlayer = false;


        }

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }
        

        public void LoadTestMap()
        {
            WriteSave(new UnityEngine.Events.UnityAction(DirectLoadTestMap));
        }

        public void DirectLoadTestMap()
        {
            string localPath = (string.IsNullOrEmpty(SavePath) ? "" : SavePath + @"\") + FileName;

            if (!System.IO.File.Exists(localPath))
            {
                InfoMessage.ShowError("File does not exist");
                return;
            }

            DataManipulators.LevelLoader.LoadLevel(localPath, true);
            
        }
        
        public Texture2D AddTexture(string TexPath)
        {
            Texture2D Tex2D;
            byte[] FileData;

            if (!TexPath.EndsWith(".png"))
                TexPath = TexPath + ".png";



            if (System.IO.File.Exists(TexPath))
            {
                FileData = System.IO.File.ReadAllBytes(TexPath);
                Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
                if (!Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                    return null;            // If data = readable -> return texture
            }
            else
                return null;
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] pathBytes = System.Text.Encoding.UTF8.GetBytes(TexPath);

            md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

            byte[] contentBytes = System.IO.File.ReadAllBytes(TexPath);

            md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);

            string localHash = System.BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();

            for (int i = 0; i < myTextures.Count; ++i)
            {
                if (myTextures[i].Hash == localHash && System.IO.Path.GetFileName(myTextures[i].Path) == System.IO.Path.GetFileName(TexPath))
                    return myTextures[i].myTexture;
            }

            myTextures.Add(new TextureData(Tex2D, TexPath, System.BitConverter.ToString(md5.Hash).Replace("-", "").ToLower()));

            return Tex2D;
        }
        
        public void RemoveActiveBlank(Blank inBlank)
        {
            inBlank.HideBlank();
            ActiveBlanks.Remove(inBlank);

        }

        public void AddActiveBlank(Blank inBlank, bool HardUpdate = false)
        {
            if (inBlank == null)
            {
                return;
            }

            if (inBlank.Active)
                return;

            
            ActiveBlanks.Add(inBlank);

            for (int i = 0; i < ActiveBlanks.Count; ++i)
                if (!ActiveBlanks[i].Active)
                    ActiveBlanks[i].ActivateBlank();

            if (myBlanksMenu.gameObject.activeSelf)
                if (HardUpdate)
                    myBlanksMenu.UpdateShot();
                else
                    myBlanksMenu.UpdateButtons();
        }

        public void SetAsActiveBlank(Blank inBlank, bool HardUpdate = false)
        {
            if (inBlank == null)
            {
                for (int i = 0; i < ActiveBlanks.Count; ++i)
                    ActiveBlanks[i].HideBlank();
                ActiveBlanks.Clear();

                if (myBlanksMenu.gameObject.activeSelf)
                    if (HardUpdate)
                        myBlanksMenu.UpdateShot();
                    else
                        myBlanksMenu.UpdateButtons();

                return;
            }

            for (int i = 0; i < ActiveBlanks.Count; ++i)
                if(ActiveBlanks[i] != inBlank)
                    ActiveBlanks[i].HideBlank();

            ActiveBlanks.Clear();

            ActiveBlanks.Add(inBlank);
            
            inBlank.ActivateBlank();

            if (myBlanksMenu.gameObject.activeSelf)
                if (HardUpdate)
                    myBlanksMenu.UpdateShot();
                else
                    myBlanksMenu.UpdateButtons();
        }
        public void SetAsActiveBlank(IEnumerable<Blank> inBlanks, bool jff, bool HardUpdate = false)
        {
            if (inBlanks == null)
            {
                for (int i = 0; i < ActiveBlanks.Count; ++i)
                    ActiveBlanks[i].HideBlank();
                ActiveBlanks.Clear();

                if (myBlanksMenu.gameObject.activeSelf)
                    if (HardUpdate)
                        myBlanksMenu.UpdateShot();
                    else
                        myBlanksMenu.UpdateButtons();

                return;
            }

            for (int i = 0; i < ActiveBlanks.Count; ++i)
                ActiveBlanks[i].HideBlank();

            ActiveBlanks.Clear();

            ActiveBlanks.AddRange(inBlanks);

            for (int i = 0; i < ActiveBlanks.Count; ++i)
                ActiveBlanks[i].ActivateBlank();

            if (myBlanksMenu.gameObject.activeSelf)
                if (HardUpdate)
                    myBlanksMenu.UpdateShot();
                else
                    myBlanksMenu.UpdateButtons();
        }

        public void CopyBlanks()
        {


            for (int i = 0; i < OnCopyList.Count; ++i)
                Destroy(OnCopyList[i].myBlank.gameObject);

            OnCopyList.Clear();

            if (ActiveBlanks.Count == 0)
                return;


            if (OnCopyList.Capacity < ActiveBlanks.Count)
                OnCopyList.Capacity = ActiveBlanks.Count;

            for (int i = 0; i < ActiveBlanks.Count; ++i)
            {
                Blank localOne = Instantiate<Blank>(ActiveBlanks[i]);
                localOne.gameObject.SetActive(false);
                OnCopyList.Add(new BlankCopyData() { myBlank = localOne, pos = localOne.transform.position });
            }
        }

        public void PasteBlanks()
        {
            if (OnCopyList.Count == 0)
                return;

            SetAsActiveBlank(null);

            for (int i = 0; i < OnCopyList.Count; ++i)
            {
                Blank localBlank = CreateBlank(OnCopyList[i].myBlank, false);

                if (localBlank == null)
                    continue;

                AddActiveBlank(localBlank);

                if (OnCopyList[i].pos.x + 0.5f < cameraWorldRect.x + cameraWorldRect.width && OnCopyList[i].pos.y - 0.5f > cameraWorldRect.y)
                    localBlank.transform.position = new Vector3(OnCopyList[i].pos.x + 0.5f, OnCopyList[i].pos.y - 0.5f, OnCopyList[i].pos.z);

                else if (OnCopyList[i].pos.x + 0.5f < cameraWorldRect.x + cameraWorldRect.width && OnCopyList[i].pos.y + 0.5f < cameraWorldRect.y + cameraWorldRect.height)
                    localBlank.transform.position = new Vector3(OnCopyList[i].pos.x + 0.5f, OnCopyList[i].pos.y + 0.5f, OnCopyList[i].pos.z);

                else if (OnCopyList[i].pos.x - 0.5f > cameraWorldRect.x && OnCopyList[i].pos.y + 0.5f < cameraWorldRect.y + cameraWorldRect.height)
                    localBlank.transform.position = new Vector3(OnCopyList[i].pos.x - 0.5f, OnCopyList[i].pos.y + 0.5f, OnCopyList[i].pos.z);

                else if (OnCopyList[i].pos.x - 0.5f > cameraWorldRect.x && OnCopyList[i].pos.y - 0.5f > cameraWorldRect.y)
                    localBlank.transform.position = new Vector3(OnCopyList[i].pos.x - 0.5f, OnCopyList[i].pos.y - 0.5f, OnCopyList[i].pos.z);

                OnCopyList[i].pos = localBlank.transform.position;

                localBlank.gameObject.SetActive(true);

            }

            if (myBlanksMenu.gameObject.activeSelf)
                myBlanksMenu.UpdateShot();

        }

        public Blank CreateBlank(E_LevelObjType inObjType, bool Hided = false)
        {
            string localName;

            bool localBreak = true;

            switch (inObjType)
            {
                case E_LevelObjType.Wall:
                    Blank_Wall localWall = Instantiate<Blank_Wall>(WallSample);

                    localName = "Wall";


                    for (int ii = 0; ii < WallList.Count; ++ii)
                        if (WallList[ii].Name == localName)
                        {
                            localBreak = false;
                            break;
                        }

                    if (!localBreak)
                    {

                        for (int i = 1; ; ++i)
                        {

                            localBreak = true;

                            for (int ii = 0; ii < WallList.Count; ++ii)
                                if (WallList[ii].Name == localName + " (" + i.ToString() + ")")
                                {
                                    localBreak = false;
                                    break;
                                }

                            if (localBreak)
                            {
                                localName = localName + " (" + i.ToString() + ")";
                                break;
                            }
                        }
                    }

                    localWall.Name = localName;

                    WallList.Add(localWall);

                    if (Hided)
                        localWall.gameObject.SetActive(false);
                    else if (myBlanksMenu.gameObject.activeSelf)
                        myBlanksMenu.UpdateShot();

                    return localWall;
                case E_LevelObjType.MainBall:
                    Blank_MB localMB = Instantiate<Blank_MB>(MainBallSample);

                    localName = "Main Ball";


                    for (int ii = 0; ii < MBList.Count; ++ii)
                        if (MBList[ii].Name == localName)
                        {
                            localBreak = false;
                            break;
                        }

                    if (!localBreak)
                    {

                        for (int i = 1; ; ++i)
                        {

                            localBreak = true;

                            for (int ii = 0; ii < MBList.Count; ++ii)
                                if (MBList[ii].Name == localName + " (" + i.ToString() + ")")
                                {
                                    localBreak = false;
                                    break;
                                }

                            if (localBreak)
                            {
                                localName = localName + " (" + i.ToString() + ")";
                                break;
                            }
                        }
                    }

                    localMB.Name = localName;

                    MBList.Add(localMB);
                    if (Hided)
                        localMB.gameObject.SetActive(false);
                    else if (myBlanksMenu.gameObject.activeSelf)
                        myBlanksMenu.UpdateShot();
                    return localMB;
                default:
                    break;
            }

            return null;
        }

        private Blank CreateBlank(Blank asSample, bool ChangeActiveBlank = true, bool UpdateBlanksMenu = false)
        {
            System.Type localType = asSample.GetType();
            Blank localOne;

            if (localType == typeof(Blank_MB))
            {
                localOne = Instantiate<Blank>(asSample);
                MBList.Add(localOne as Blank_MB);
                if (ChangeActiveBlank)
                    SetAsActiveBlank(localOne);

                if (myBlanksMenu.gameObject.activeSelf && UpdateBlanksMenu)
                    myBlanksMenu.UpdateShot();

                return localOne;
            }

            if (localType == typeof(Blank_Wall))
            {
                localOne = Instantiate<Blank>(asSample);
                WallList.Add(localOne as Blank_Wall);
                if (ChangeActiveBlank)
                    SetAsActiveBlank(localOne);

                if (myBlanksMenu.gameObject.activeSelf && UpdateBlanksMenu)
                    myBlanksMenu.UpdateShot();

                return localOne;
            }

            return null;
        }

        public void DeleteBlanks()
        {
            for (int i = 0; i < ActiveBlanks.Count; ++i)
                ActiveBlanks[i].DeleteBlank();

            ActiveBlanks.Clear();

            if (myBlanksMenu.gameObject.activeSelf)
                myBlanksMenu.UpdateShot();
        }

        public void OpenWallMenu(Blank_Wall inBlank)
        {
            myWallMenu.OpenMenu(Input.mousePosition, inBlank);
        }

        public void OpenMBMenu(Blank_MB inBlank)
        {
            myMBMenu.OpenMenu(Input.mousePosition, inBlank);
        }

        public void OpenMainPlayerMenu(Blank_Player inBlank)
        {
            myMPMenu.OpenMenu(Input.mousePosition, inBlank);
        }


        public void CloseMenues(E_MenuType inMenu)
        {

            if ((inMenu & E_MenuType.BlanksMenu) == E_MenuType.BlanksMenu)
            {
                myBlanksMenu.CloseMenu();
            }
            if ((inMenu & E_MenuType.ObjectsMenu) == E_MenuType.ObjectsMenu)
            {
                myObjectMenu.CloseMenu();
            }
            if ((inMenu & E_MenuType.Textures) == E_MenuType.Textures)
            {
                if (myColorPicker.gameObject.activeSelf)
                    myColorPicker.gameObject.SetActive(false);
            }
            if ((inMenu & E_MenuType.OnFieldMenu) == E_MenuType.OnFieldMenu)
            {
                if (myWallMenu.gameObject.activeSelf)
                    myWallMenu.CloseMenu();
                if (myMBMenu.gameObject.activeSelf)
                    myMBMenu.CloseMenu();
                if (myMPMenu.gameObject.activeSelf)
                    myMPMenu.CloseMenu();
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftControl))
                CopyBlanks();
            if (Input.GetKeyDown(KeyCode.V) && Input.GetKey(KeyCode.LeftControl))
                PasteBlanks();

            if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl))
                WriteSave(null);

            if (Input.GetAxis("Mouse ScrollWheel") != 0f && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    myCamera.orthographicSize--;
                    if (myCamera.orthographicSize < 1)
                        myCamera.orthographicSize = 1;
                }
                else
                {
                    myCamera.orthographicSize++;
                    if (myCamera.orthographicSize > 20)
                        myCamera.orthographicSize = 20;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                CameraDrag = false;
            }

            if (CameraDrag)
            {
                myCamera.transform.position = StartWorldCameraPos +
                    myCamera.ScreenToWorldPoint(StartScreenMousePos) - myCamera.ScreenToWorldPoint(Input.mousePosition);

                if (cameraWorldRect.width != 0 || cameraWorldRect.y != 0)
                {
                    Vector3 newVec = myCamera.transform.position;

                    if (newVec.x < cameraWorldRect.x)
                        newVec.x = cameraWorldRect.x;
                    if (newVec.y < cameraWorldRect.y)
                        newVec.y = cameraWorldRect.y;
                    if (newVec.x > cameraWorldRect.x + cameraWorldRect.width)
                        newVec.x = cameraWorldRect.x + cameraWorldRect.width;
                    if (newVec.y > cameraWorldRect.y + cameraWorldRect.height)
                        newVec.y = cameraWorldRect.y + cameraWorldRect.height;

                    myCamera.transform.position = newVec;
                }

                return;
            }

            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                CloseMenues(E_MenuType.All & ~E_MenuType.BlanksMenu);

                Collider2D myColl = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                bool Ctrl = Input.GetKey(KeyCode.LeftControl);

                if (myColl != null)
                {

                    Blank myBlank = myColl.gameObject.GetComponentInParent<Blank>();
                    if (myBlank != null)
                    {

                    }
                    else
                        SetAsActiveBlank(null);
                }
                else
                {
                    CameraDrag = true;



                    StartWorldCameraPos = myCamera.transform.position;
                    StartScreenMousePos = Input.mousePosition;
                    if (!Ctrl)
                        SetAsActiveBlank(null);
                }
            }

            if (Input.GetMouseButtonUp(1) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                CloseMenues(E_MenuType.All & ~E_MenuType.BlanksMenu);

                Collider2D myColl = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                if (myColl != null)
                {
                    Blank myBlank = myColl.gameObject.GetComponentInParent<Blank>();
                    if (myBlank != null)
                    {
                        SetAsActiveBlank(myBlank);
                        myBlank.OnRightMouseButtonDown();
                    }
                    else
                        SetAsActiveBlank(null);
                }
                else
                    SetAsActiveBlank(null);
            }

            if (Input.GetKeyDown(KeyCode.Delete))
                DeleteBlanks();

            //if (Input.GetMouseButtonDown(1) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            //{
            //    CloseAllMenues();
            //
            //    Collider2D myColl = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            //
            //    if (myColl != null)
            //    {
            //        Blank myBlank = myColl.gameObject.GetComponentInParent<Blank>();
            //        if (myBlank != null)
            //        {
            //            SetAsActiveBlank(myBlank);
            //        }
            //        else
            //            SetAsActiveBlank(null);
            //    }
            //    else
            //        SetAsActiveBlank(null);
            //}




        }

        public void LeftClickOnObject(Blank inObject)
        {
            bool Ctrl = Input.GetKey(KeyCode.LeftControl);

            if (Ctrl)
            {
                if (inObject.Active)
                    RemoveActiveBlank(inObject);
                else
                    AddActiveBlank(inObject);
            }
            else
                SetAsActiveBlank(inObject);
        }

        //-----------------------------------// Save //-----------------------------------//

        public void SaveAs()
        {
            if (string.IsNullOrEmpty(SavePath))
            {
                myFC.PathStr = "";                

            }
            else
            {

                myFC.PathStr = SavePath;
            }

            myFC.FolderStr = "";

            myFC.onSuccessEvent.RemoveAllListeners();
            myFC.onSuccessEvent.AddListener(new UnityEngine.Events.UnityAction(ContinueSave));
            myFC.OpenMenu(true);

            CloseMenues(E_MenuType.All);
        }

        public void WriteSave(UnityEngine.Events.UnityAction inEventAfterSave)
        {
            EventAfterSave.RemoveAllListeners();

            if (inEventAfterSave != null)
                EventAfterSave.AddListener(inEventAfterSave);
            

            ContinueSave();
            
        }

        public void ContinueSave()
        {
            if (string.IsNullOrEmpty(FileName))
            {
                SaveAs();
                return;
            }

            if (!System.IO.Directory.Exists(SavePath))
            {
                System.IO.Directory.CreateDirectory(SavePath);
            }

            StartCoroutine("WriteFiles");
        }
        
        public IEnumerator WriteFiles()
        {
            List<string> totalStrings = new List<string>(4);

            List<TextureData> localTextures = new List<TextureData>(myTextures.Count);
            List<SpriteData> localSprites = new List<SpriteData>();

            List<string> localStringList = new List<string>();

            string localString;
            
            int localI;

            TextureData localTextureData;

            // ------------------------// Level Props //---------------------------------

            localTextureData = myObjectMenu.LevelImage.GetTextureData();
            if(localTextureData.Hash == 1.ToString())
            {
                localString = "\"SimpleBox\"";
            }
            else if (localTextureData.Hash == 2.ToString())
            {
                localString = "\"SimpleCircle\"";
            }
            else
            {
                localI = LoadTextureIntoList(localTextures, localTextureData);
                if (localI == -1)
                {
                    InfoMessage.ShowError("Can't find Level image");
                }

                localString = localI.ToString();
            }

            System.Text.StringBuilder myBuilder = new System.Text.StringBuilder(LevelName);

            myBuilder.Replace("%", "%10");
            myBuilder.Replace("(", "%11");
            myBuilder.Replace(")", "%12");
            myBuilder.Replace(",", "%13");
            myBuilder.Replace("=", "%14");
            myBuilder.Replace("[", "%15");
            myBuilder.Replace("]", "%16");
            myBuilder.Replace("~", "%17");
            myBuilder.Replace(":", "%18");
            myBuilder.Replace("\"", "%19");
            myBuilder.Replace(" ", "%20");

            totalStrings.Add("LevelProp:\n(Name = \"" + myBuilder.ToString() + "\", LevelTexture = " + localString + ")\n");

            yield return null;

            // ------------------------// Map Settings //---------------------------------

            /*
                MapSettings:
                (PointsMultiplyer = 3.0, TimeSpeed = 1, SideBallStartSpawn = 20, SmallBallStartSpawn = 10, BackgroundColor = 0.8|0.8|0.0|1) 
            */

            totalStrings.Add("MapSettings:\n(PointsMultiplyer = " + PointsMultiplyer.ToString() +
                ", TimeSpeed = " + TimeSpeed.ToString() +
                ", SideBallStartSpawn = " + SideBallStartSpawn.ToString() +
                ", SmallBallStartSpawn = " + SmallBallStartSpawn.ToString() +
                ", BackgroundColor = " + myCamera.backgroundColor.r.ToString() + "|" + myCamera.backgroundColor.g.ToString() + 
                                        "|" + myCamera.backgroundColor.b.ToString() + "|" + myCamera.backgroundColor.a.ToString() +
                ")\n");

            yield return null;
            // ------------------------// Camera //---------------------------------

            /*
                Camera:
                (Size = 5.7, SnapToPlayer = true)
            */

            totalStrings.Add("Camera:\n(Size = " + CameraBorder.transform.localScale.y.ToString() + 
                ", SnapToPlayer = " + SnapCameraToPlayer.ToString().ToLower() + ")\n");


            yield return null;
            // ------------------------// Events //---------------------------------

            /*
                Events(AverageTime = 30, DeltaTime = 8):
                [0] (EventType = Line, a = 10, b = 8, Num = 10)
                [1] (EventType = Circle, a = 20, Num = 5)
            */

            if (myEvents.Count != 0)
            {
                localStringList.Clear();
                if (localStringList.Capacity < myEvents.Count + 1)
                    localStringList.Capacity = myEvents.Count + 1;



                localStringList.Add( "Events(AverageTime = " + AverageTime.ToString() + ", DeltaTime = " + DeltaTime.ToString() + "):");

                for(int i = 0; i < myEvents.Count; ++i)
                {
                    switch (myEvents[i].myType)
                    {
                        case EventsData.E_EventType.Line:
                            localStringList.Add("[" + i.ToString() + "] (EventType = Line, a = " + myEvents[i].a.ToString() + 
                                                        ", b = " + myEvents[i].b.ToString() + ", Num = " + myEvents[i].Num.ToString() + ")");
                            break;
                        case EventsData.E_EventType.Slide:
                            localStringList.Add("[" + i.ToString() + "] (EventType = Slide, a = " + myEvents[i].a +
                                                ", b = " + myEvents[i].b.ToString() + ", c = " + myEvents[i].c.ToString() + ", Num = " + myEvents[i].Num.ToString() + ")");
                            break;
                        case EventsData.E_EventType.Circle:
                            localStringList.Add("[" + i.ToString() + "] (EventType = Circle, a = " + myEvents[i].a.ToString() +
                                                                                    ", Num = " + myEvents[i].Num.ToString() + ")");
                            break;
                        default:
                            break;
                    }

                    yield return null;
                }

                totalStrings.Add(string.Join("\n", localStringList.ToArray()) + "\n");

            }

            yield return null;
            // ------------------------// Main Player //---------------------------------

            /*
                MainPlayer: 
                (SpawnPoint = -7.23|1.46, HP = 5, InvulDamageTime = 1.5, InvulHitTime = 0.7, HitReloadTime = 3, HitRadius = 2)
            */

            if (myPlayer != null)
            {

                totalStrings.Add("MainPlayer:\n" + myPlayer.ToString() + "\n");
            
            }
            yield return null;
            // ------------------------// Main Balls //---------------------------------
            /*
                MainBalls:
                [0](Name = "some", SpawnPoint = 5|1.5|-1, StartRot = 30, Scale = 1.0|1.0, Color = 1|0|1|1, StartSpeed = 5, SpeedHitChange = 1.01, HitsAvarage = 7, HitsDelta = 2, SmallBalls = 3, ExpAngle = 90)
                [1](Name = "some", SpawnPoint = 5|1.5|-1, StartRot = 150,Scale = 1.5|1.5, Color = 1|0|0|1, StartSpeed = 2, SpeedHitChange = 1.5, HitsAvarage = 16, HitsDelta = 5, SmallBalls = 5, ExpAngle = 180)
                [2](Name = "some", SpawnPoint = 5|1.5|-1, Sprite = 0, StartRot = 270)
            */

            localStringList.Clear();

            localStringList.Add("MainBalls:");

            SpriteData localSpriteData = new SpriteData();

            for(int i = 0; i < MBList.Count; ++i)
            {
                if (!MBList[i].gameObject.activeSelf)
                    continue;

                localSpriteData.Pivot.x = MBList[i].GetSprite().pivot.x / MBList[i].GetSprite().rect.width;
                localSpriteData.Pivot.y = MBList[i].GetSprite().pivot.y / MBList[i].GetSprite().rect.height;

                localSpriteData.PPU = MBList[i].GetSprite().pixelsPerUnit;

                localTextureData = MBList[i].myTextureData;

                if (localTextureData.Hash == "1")
                {
                    localSpriteData.TextureIndex = -1;
                }
                else if (localTextureData.Hash == "2")
                {
                    localSpriteData.TextureIndex = -2;
                }
                else
                {
                    localI = LoadTextureIntoList(localTextures, localTextureData);
                    if (localI == -1)
                    {
                        InfoMessage.ShowError("Can't find Main ball image");
                    }

                    localSpriteData.TextureIndex = localI;
                }

                localI = LoadSpriteIntoList(localSprites, localSpriteData);

                myBuilder = new System.Text.StringBuilder(MBList[i].Name);

                myBuilder.Replace("%", "%10");
                myBuilder.Replace("(", "%11");
                myBuilder.Replace(")", "%12");
                myBuilder.Replace(",", "%13");
                myBuilder.Replace("=", "%14");
                myBuilder.Replace("[", "%15");
                myBuilder.Replace("]", "%16");
                myBuilder.Replace("~", "%17");
                myBuilder.Replace(":", "%18");
                myBuilder.Replace("\"", "%19");
                myBuilder.Replace(" ", "%20");


                localStringList.Add("[" + i.ToString() + "](Name = \"" + myBuilder.ToString() + "\", " + MBList[i].ToString() + ", Sprite = " + localI.ToString() + ")");

                yield return null;
            }

            totalStrings.Add(string.Join("\n", localStringList.ToArray()) + "\n");

            yield return null;
            // ------------------------// Side ball //---------------------------------

            /*
                SideBall:
                (Color = 1|0|1|1, StartSpeed = 20)
            */

            localTextureData = myObjectMenu.SideBallBlank.GetTextureData();

            localSpriteData.Pivot.x = myObjectMenu.SideBallBlank.GetSprite().pivot.x / myObjectMenu.SideBallBlank.GetSprite().rect.width;
            localSpriteData.Pivot.y = myObjectMenu.SideBallBlank.GetSprite().pivot.y / myObjectMenu.SideBallBlank.GetSprite().rect.height;

            localSpriteData.PPU = myObjectMenu.SideBallBlank.GetSprite().pixelsPerUnit;

            if (localTextureData.Hash == "1")
            {
                localSpriteData.TextureIndex = -1;
            }
            else if (localTextureData.Hash == "2")
            {
                localSpriteData.TextureIndex = -2;
            }
            else
            {
                localI = LoadTextureIntoList(localTextures, localTextureData);
                if (localI == -1)
                {
                    InfoMessage.ShowError("Can't find side ball image");
                }

                localSpriteData.TextureIndex = localI;
            }

            localI = LoadSpriteIntoList(localSprites, localSpriteData);

            Color localColor = myObjectMenu.SideBallBlank.GetColor();

            totalStrings.Add("SideBall:\n(Color = " + localColor.r.ToString() + "|" + localColor.g.ToString() + "|" + localColor.b.ToString() + "|" + localColor.a.ToString() +
                                ", StartSpeed = " + SideBallStartSpeed.ToString() + ", Sprite = " + localI.ToString() + ")\n");


            yield return null;
            // ------------------------// Small ball //---------------------------------

            /*
                SmallBall:
                (Color = 1|0|1|1, StartSpeed = 1, SpeedHitChange = 4, Reflects = 10)
            */

            localTextureData = myObjectMenu.SmallBallBlank.GetTextureData();

            localSpriteData.Pivot.x = myObjectMenu.SmallBallBlank.GetSprite().pivot.x / myObjectMenu.SmallBallBlank.GetSprite().rect.width;
            localSpriteData.Pivot.y = myObjectMenu.SmallBallBlank.GetSprite().pivot.y / myObjectMenu.SmallBallBlank.GetSprite().rect.height;

            localSpriteData.PPU = myObjectMenu.SmallBallBlank.GetSprite().pixelsPerUnit;

            if (localTextureData.Hash == "1")
            {
                localSpriteData.TextureIndex = -1;
            }
            else if (localTextureData.Hash == "2")
            {
                localSpriteData.TextureIndex = -2;
            }
            else
            {
                localI = LoadTextureIntoList(localTextures, localTextureData);
                if (localI == -1)
                {
                    InfoMessage.ShowError("Can't find small ball image");
                }

                localSpriteData.TextureIndex = localI;
            }

            localI = LoadSpriteIntoList(localSprites, localSpriteData);

            localColor = myObjectMenu.SmallBallBlank.GetColor();

            totalStrings.Add("SmallBall:\n(Color = " + localColor.r.ToString() + "|" + localColor.g.ToString() + "|" + localColor.b.ToString() + "|" + localColor.a.ToString() +
                                ", StartSpeed = " + SmallBallStartSpeed.ToString() + ", SpeedHitChange = " + SmallBallSpeedHitChange.ToString() + 
                                ", Reflects = " + SmallBallReflects.ToString() + ", Sprite = " + localI.ToString() + ")\n");


            yield return null;
            // ------------------------// Out Walls //---------------------------------
            /*
                OutWalls(Color = 0.8|0.1|0.1|1):
                [0](Name = "some", Pos = 0|-5, Scale = 30|1)
                [1](Name = "some", Pos = 0|5, Scale = 30|1)
                [2](Name = "some", Pos = -9.28|0, Scale = 1|15)
                [3](Name = "some", Pos = 9.28|0, Scale = 1|15)
            */

            localStringList.Clear();
            localStringList.Add("OutWalls:");

            for(int i = 0; i < WallList.Count; ++i)
            {
                if (WallList[i].myWallType != WallType.OutWall || !WallList[i].gameObject.activeSelf)
                    continue;

                localTextureData = WallList[i].GetTextureData();

                localSpriteData.Pivot.x = WallList[i].GetSprite().pivot.x / WallList[i].GetSprite().rect.width;
                localSpriteData.Pivot.y = WallList[i].GetSprite().pivot.y / WallList[i].GetSprite().rect.height;

                localSpriteData.PPU = WallList[i].GetSprite().pixelsPerUnit;

                if (localTextureData.Hash == "1")
                {
                    localSpriteData.TextureIndex = -1;
                }
                else if (localTextureData.Hash == "2")
                {
                    localSpriteData.TextureIndex = -2;
                }
                else
                {
                    localI = LoadTextureIntoList(localTextures, localTextureData);
                    if (localI == -1)
                    {
                        InfoMessage.ShowError("Can't find out wall image");
                    }

                    localSpriteData.TextureIndex = localI;
                }

                localI = LoadSpriteIntoList(localSprites, localSpriteData);

                localColor = WallList[i].GetColor();

                myBuilder = new System.Text.StringBuilder(WallList[i].Name);

                myBuilder.Replace("%", "%10");
                myBuilder.Replace("(", "%11");
                myBuilder.Replace(")", "%12");
                myBuilder.Replace(",", "%13");
                myBuilder.Replace("=", "%14");
                myBuilder.Replace("[", "%15");
                myBuilder.Replace("]", "%16");
                myBuilder.Replace("~", "%17");
                myBuilder.Replace(":", "%18");
                myBuilder.Replace("\"", "%19");
                myBuilder.Replace(" ", "%20");

                localStringList.Add("[" + i.ToString() + "](Name = \"" + myBuilder.ToString() + "\", " + WallList[i].ToString() + ", Sprite = " + localI + ")");

                yield return null;
            }

            totalStrings.Add(string.Join("\n", localStringList.ToArray()) + "\n");


            yield return null;
            // ------------------------// in Walls //---------------------------------
            /*
                InWalls(Color = 0.8|0.1|0.1|1):
                [0](Name = "some", Pos = 0|-5, Scale = 30|1)
                [1](Name = "some", Pos = 0|5, Scale = 30|1)
                [2](Name = "some", Pos = -9.28|0, Scale = 1|15)
                [3](Name = "some", Pos = 9.28|0, Scale = 1|15)
            */

            localStringList.Clear();
            localStringList.Add("InWalls:");

            for (int i = 0; i < WallList.Count; ++i)
            {
                if (WallList[i].myWallType != WallType.InWall || !WallList[i].gameObject.activeSelf)
                    continue;

                localTextureData = WallList[i].GetTextureData();

                localSpriteData.Pivot.x = WallList[i].GetSprite().pivot.x / WallList[i].GetSprite().rect.width;
                localSpriteData.Pivot.y = WallList[i].GetSprite().pivot.y / WallList[i].GetSprite().rect.height;

                localSpriteData.PPU = WallList[i].GetSprite().pixelsPerUnit;

                if (localTextureData.Hash == "1")
                {
                    localSpriteData.TextureIndex = -1;
                }
                else if (localTextureData.Hash == "2")
                {
                    localSpriteData.TextureIndex = -2;
                }
                else
                {
                    localI = LoadTextureIntoList(localTextures, localTextureData);
                    if (localI == -1)
                    {
                        InfoMessage.ShowError("Can't find in wall image");
                    }

                    localSpriteData.TextureIndex = localI;
                }

                localI = LoadSpriteIntoList(localSprites, localSpriteData);

                localColor = WallList[i].GetColor();

                myBuilder = new System.Text.StringBuilder(WallList[i].Name);

                myBuilder.Replace("%", "%10");
                myBuilder.Replace("(", "%11");
                myBuilder.Replace(")", "%12");
                myBuilder.Replace(",", "%13");
                myBuilder.Replace("=", "%14");
                myBuilder.Replace("[", "%15");
                myBuilder.Replace("]", "%16");
                myBuilder.Replace("~", "%17");
                myBuilder.Replace(":", "%18");
                myBuilder.Replace("\"", "%19");
                myBuilder.Replace(" ", "%20");

                localStringList.Add("[" + i.ToString() + "](Name = \"" + myBuilder.ToString() + "\", " + WallList[i].ToString() + ", Sprite = " + localI + ")");

                yield return null;
            }

            totalStrings.Add(string.Join("\n", localStringList.ToArray()) + "\n");

            yield return null;
            // ------------------------// Sprites //---------------------------------

            /*
                Sprites:
                [0](Texture = 1, PixelsPerUnit = 100, Pivot = 0.5|0.5)
            */

            localStringList.Clear();

            localStringList.Add("Sprites:");

            for(int i = 0; i < localSprites.Count; ++i)
            {
                if (localSprites[i].TextureIndex == -1)
                {
                    localString = "\"SimpleBox\"";
                }
                else if (localSprites[i].TextureIndex == -2)
                {
                    localString = "\"SimpleCircle\"";
                }
                else
                {                    
                    localString = localSprites[i].TextureIndex.ToString();
                }

                localStringList.Add("[" + i.ToString() + "](Texture = " + localString + ", PixelsPerUnit = " + localSprites[i].PPU.ToString() +
                    ", Pivot = " + localSprites[i].Pivot.x.ToString() + "|" + localSprites[i].Pivot.y.ToString() + ")");

                yield return null;
            }

            totalStrings.Add(string.Join("\n", localStringList.ToArray()) + "\n");

            yield return null;

            // ------------------------// Textures //---------------------------------

            /*
                Textures:
                [0](TextureName = "Textures\TestLevelImage.png")
                [1](TextureName = "Textures\MBall.png")
            */
            
            localStringList.Clear();

            localStringList.Add("Textures:");

            string localPath;

            for (int i = 0; i < localTextures.Count; ++i)
            {
                localPath = SavePath;
                if (string.IsNullOrEmpty(localPath))
                    localPath = "Textures";
                else
                    localPath = localPath + @"\Textures";



                if (!System.IO.Directory.Exists(localPath))
                    System.IO.Directory.CreateDirectory(localPath);

                localString = System.IO.Path.GetFileNameWithoutExtension(localTextures[i].Path);


                localString = localString + ".png";

                byte[] _bytes = ImageConversion.EncodeToPNG(localTextures[i].myTexture);
                System.IO.File.WriteAllBytes(localPath + @"\" + localString, _bytes);

                myBuilder = new System.Text.StringBuilder("Textures\\" + localString);

                myBuilder.Replace("%", "%10");
                myBuilder.Replace("(", "%11");
                myBuilder.Replace(")", "%12");
                myBuilder.Replace(",", "%13");
                myBuilder.Replace("=", "%14");
                myBuilder.Replace("[", "%15");
                myBuilder.Replace("]", "%16");
                myBuilder.Replace("~", "%17");
                myBuilder.Replace(":", "%18");
                myBuilder.Replace("\"", "%19");
                myBuilder.Replace(" ", "%20");

                localStringList.Add("[" + i.ToString() + "](TextureName = \""+ myBuilder.ToString() + "\")");

                yield return null;
            }

            totalStrings.Add(string.Join("\n", localStringList.ToArray()));

            

            yield return null;

            // ------------------------// File itself //---------------------------------

            //if (!System.IO.File.Exists(SavePath + @"\" + DirectoryName + @"\" + LevelName + ".jpl"))
            //{
            //    localStream = System.IO.File.Create(SavePath + @"\" + DirectoryName + @"\" + LevelName + ".jpl");
            //}
            //else



            //try
            //{
            //    localStream = System.IO.File.Open(SavePath + @"\" + DirectoryName + @"\" + LevelName + ".jpl", System.IO.FileMode.OpenOrCreate);
            //
            //    
            //}
            //finally
            //{
            //
            //}

            localPath = SavePath;

            if (string.IsNullOrEmpty(localPath))
                localPath = "";
            else
                localPath = localPath + @"\";

            if (string.IsNullOrEmpty(FileName))
                localPath = localPath + LevelName + ".jrl";
            else
                localPath = localPath + FileName;

            System.IO.File.WriteAllText(localPath, string.Join("~\n", totalStrings.ToArray()));


            EventAfterSave.Invoke();

            EventAfterSave.RemoveAllListeners();

            yield break;

        }

        private int LoadTextureIntoList(List<TextureData> inTextures, TextureData inTextureData)
        {
            for(int i = 0; i < inTextures.Count; ++i)
                if(inTextures[i].Hash == inTextureData.Hash)
                {
                    return i;
                }

            for(int i = 0; i < myTextures.Count; ++i)
                if(myTextures[i].Hash == inTextureData.Hash)
                {
                    inTextures.Add(myTextures[i]);
                    return inTextures.Count - 1;
                }

            return -1;

        }

        private int LoadSpriteIntoList(List<SpriteData> inSprites, SpriteData inSprite)
        {
            for(int i = 0; i < inSprites.Count; ++i)
            {
                if (inSprites[i].TextureIndex == inSprite.TextureIndex && inSprites[i].Pivot == inSprite.Pivot && inSprites[i].PPU == inSprite.PPU)
                    return i;
            }

            inSprites.Add(inSprite);
            return inSprites.Count - 1;
        }


        //-----------------------------------// Load //-----------------------------------//

        public void LoadFile()
        {
            if (string.IsNullOrEmpty(SavePath))
            {
                myFC.PathStr = "";

            }
            else
            {               

                myFC.PathStr = SavePath;
            }

            myFC.FolderStr = "";
            myFC.FileStr = "";

            myFC.onSuccessEvent.RemoveAllListeners();
            myFC.onSuccessEvent.AddListener(new UnityEngine.Events.UnityAction(DirectLoad));
            myFC.OpenMenu(false);

            CloseMenues(E_MenuType.All);

        }

        private void DirectLoad()
        {

            if(string.IsNullOrEmpty(FileName))
            {
                InfoMessage.ShowError("No file to load");
                return;
            }

            
            ReloadAll();

            StartCoroutine("LoadMapFile");

            
        }

        public IEnumerator LoadMapFile()
        {
            string localPath = (string.IsNullOrEmpty(SavePath) ? "" : SavePath + @"\") + FileName;

            if (!System.IO.File.Exists(localPath))
            {
                InfoMessage.ShowError("Level file not found: " + localPath);
                yield break;
            }

            string[] stringArr = System.IO.File.ReadAllLines(localPath);

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
                        yield break;
                    }
                
            }
            yield return null;
            for (int i = stringArr.Length - 1; i >= 0; --i)
            {
                if (stringArr[i].Contains("Sprites"))
                    if (LoadSprites(stringArr[i]))
                        break;
                    else
                    {
                        InfoMessage.ShowError("Load sprites fail");
                        yield break ;
                    }

            }
            yield return null;
            string[] localArr;

            for (int i = 0; i < stringArr.Length; ++i)
            {
                localArr = stringArr[i].Split(':');

                if (localArr.Length != 2)
                    continue;

                localArr = localArr[0].Split('(');

                switch (localArr[0])
                {
                    case "LevelProp":
                        if(!Change_LevelProps(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in level properties read");
                            yield break;
                        }
                        break;

                    case "Camera":
                        if (!Change_Camera(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in camera read");
                            yield break;
                        }
                        break;

                    case "Events":
                        if (!Change_Events(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in events read");
                            yield break;
                        }
                        break;

                    case "MapSettings":
                        if (!Change_MapSettings(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in map settings read");
                            yield break;
                        }
                        break;

                    case "MainPlayer":
                        if (!Change_Player(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in player read");
                            yield break;
                        }
                        break;

                    case "MainBalls":
                        if (!Change_MainBalls(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in main balls read");
                            yield break;
                        }
                        break;

                    case "SmallBall":
                        if (!Change_SmallBall(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in small ball read");
                            yield break;
                        }
                        break;

                    case "SideBall":
                        if (!Change_SideBall(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in side ball read");
                            yield break;
                        }
                        break;

                    case "OutWalls":
                        if (!Change_OutWalls(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in \"out walls\" read");
                            yield break;
                        }
                        break;

                    case "InWalls":
                        if (!Change_InWalls(stringArr[i]))
                        {
                            InfoMessage.ShowError("Fail in \"in walls\" read");
                            yield break;
                        }
                        break;

                    default:
                        break;
                }

                yield return null;
            }

            SetAsActiveBlank(null);

            yield break;
        }

        private bool LoadTextures(string inString)
        {
            string[] lines = inString.Split(':');
            if (lines.Length != 2)
                return false;

            LoadingTextures.Clear();

            lines = lines[1].Split('[');

            string localPath = string.IsNullOrEmpty(SavePath) ? "" : SavePath + @"\";



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

                localArr[1] = localPath + myBuilder.ToString();

                if (System.IO.File.Exists(localArr[1]))
                {
                    
                    FileData = System.IO.File.ReadAllBytes(localArr[1]);
                    Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
                    if (!Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                        continue;            // If data = readable -> return texture
                }
                else
                    continue;

                System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

                byte[] pathBytes = System.Text.Encoding.UTF8.GetBytes(localArr[1]);

                md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                byte[] contentBytes = System.IO.File.ReadAllBytes(localArr[1]);

                md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);

                LoadingTextures.Add(myNum, new TextureData(Tex2D, localArr[1], System.BitConverter.ToString(md5.Hash).Replace("-", "").ToLower()));

            }

            TextureData[] localTexs = LoadingTextures.Values.ToArray();

            myTextures.Clear();

            for (int i = 0; i < localTexs.Length; ++i)
                myTextures.Add(localTexs[i]);

            return true;

        }

        private bool LoadSprites(string inString)
        {
            string[] lines = inString.Split(':');
            if (lines.Length != 2)
                return false;

            LoadingSprites.Clear();

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
                //Texture2D Tex2D = null;
                int TextureID = -1;
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
                                //Tex2D = Resources.Load<Texture2D>("S_SimpleBox");
                                TextureID = -1;
                            }
                            else if (propArr[1] == "\"SimpleCircle\"")
                            {
                                //Tex2D = Resources.Load<Texture2D>("S_SimpleCircle");
                                TextureID = -2;
                            }
                            else
                            {
                                if (!int.TryParse(propArr[1], out TextureID))
                                    break;

                                if (!LoadingTextures.ContainsKey(TextureID))
                                {
                                    print("texture not find (index : " + TextureID.ToString() + ')');
                                    break;
                                }
                            }
                            break;

                        case "PixelsPerUnit":

                            if (!float.TryParse(propArr[1], out PixPerUnit))
                                PixPerUnit = 100;

                            break;

                    }

                }

               

                LoadingSprites.Add(myNum, new SpriteData(TextureID, PixPerUnit, Pivot));

            }

            print("sprites: " + LoadingSprites.Count);

            return true;
        }

        //enum CollisionType
        //{
        //    Circle, Box, Capsule
        //}

        private bool Change_LevelProps(string inString)
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

                //Name = "new%20level", LevelTexture = "SimpleBox"
                switch (propArr[0])
                {
                    case "Name":

                        System.Text.StringBuilder myBuilder = new System.Text.StringBuilder(propArr[1]);

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

                        LevelName = myBuilder.ToString();

                        break;

                    case "LevelTexture":

                        if(propArr[1] == "\"SimpleBox\"")
                        {
                            myObjectMenu.LevelImage.SetTextureData(new TextureData(Resources.Load<Texture2D>("S_SimpleBox"), "", "1"));
                        }
                        else if (propArr[1] == "\"SimpleCircle\"")
                        {
                            myObjectMenu.LevelImage.SetTextureData(new TextureData(Resources.Load<Texture2D>("S_SimpleCircle"), "", "2"));
                        }
                        else
                        {
                            int localI;
                            if (int.TryParse(propArr[1], out localI) && LoadingTextures.ContainsKey(localI))
                                myObjectMenu.LevelImage.SetTextureData(LoadingTextures[localI]);
                            else
                                myObjectMenu.LevelImage.SetTextureData(new TextureData(Resources.Load<Texture2D>("S_SimpleBox"), "", "1"));

                        }


                        break;
                    default:
                        break;
                }
            }

            return true;
        }

        private bool Change_InWalls(string inString)
        {
            Color totalColor = Color.white;
            CollisionType totalColl = CollisionType.Box;
            Vector2 totalCollisionSize = new Vector2(-1f, -1f);
            Vector3 totalPos = new Vector3();
            float totalRot = 0f;
            Vector3 totalScale = Vector3.one;
            SpriteData totalSprite = new SpriteData(-1,100,new Vector2(0.5f,0.5f));

            

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
                                if (!LoadingSprites.ContainsKey(localI))
                                {
                                    InfoMessage.ShowError("Fail to find sprite in Wall");
                                    continue;
                                }
                                else
                                    totalSprite = LoadingSprites[localI];
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
                SpriteData localSprite = totalSprite;

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
                                if (!LoadingSprites.ContainsKey(localI))
                                {
                                    InfoMessage.ShowError("Fail to find sprite in Wall");
                                    continue;
                                }
                                else
                                    localSprite = LoadingSprites[localI];
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

                //GameObject localWall = new GameObject();

                Blank_Wall localWall = (Blank_Wall)CreateBlank(E_LevelObjType.Wall);

                localWall.myWallType = WallType.InWall;



                localWall.transform.position = localPos;
                localWall.transform.rotation = Quaternion.Euler(0, 0, localRot);
                localWall.ScaleObj.GetComponent<Scalable>().ObjectToScale.transform.localScale = localScale;

                TextureData localTD = new TextureData();

                if (localSprite.TextureIndex == -1)
                {
                    localTD.myTexture = Resources.Load<Texture2D>("S_SimpleBox");
                    localTD.Hash = 1.ToString();
                }
                else if (localSprite.TextureIndex == -2)
                {
                    localTD.myTexture = Resources.Load<Texture2D>("S_SimpleCircle");
                    localTD.Hash = 2.ToString();
                }
                else
                    localTD = LoadingTextures[localSprite.TextureIndex];

                localWall.SetTextureData(localTD);
                localWall.ChangePivotAndPPU(localSprite.Pivot, localSprite.PPU);

                localWall.SetColor(localColor);

                switch (localColl)
                {
                    case CollisionType.Circle:

                        localWall.ChangeCollision(CollisionType.Circle);

                        if (localCollisionSize.x > 0)
                            localWall.CollisionOutline.transform.localScale = new Vector3(localCollisionSize.x, localCollisionSize.x, 1);


                        break;
                    case CollisionType.Box:

                        localWall.ChangeCollision(CollisionType.Box);


                        Vector3 localSize = localWall.CollisionOutline.transform.localScale;
                        if (localCollisionSize.x > 0)
                            localSize.x = localCollisionSize.x;

                        if (localCollisionSize.y > 0)
                            localSize.y = localCollisionSize.y;

                        localWall.CollisionOutline.transform.localScale = localSize;

                        break;
                    default:
                        break;
                }

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
            SpriteData totalSprite = new SpriteData(-1, 100, new Vector2(0.5f, 0.5f));

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
                                if (!LoadingSprites.ContainsKey(localI))
                                {
                                    InfoMessage.ShowError("Fail to find sprite in Wall");
                                    continue;
                                }
                                else
                                    totalSprite = LoadingSprites[localI];
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
                SpriteData localSprite = totalSprite;

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
                                if (!LoadingSprites.ContainsKey(localI))
                                {
                                    InfoMessage.ShowError("Fail to find sprite in Wall");
                                    continue;
                                }
                                else
                                    localSprite = LoadingSprites[localI];
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

                Blank_Wall localWall = (Blank_Wall)CreateBlank(E_LevelObjType.Wall);

                localWall.myWallType = WallType.OutWall;



                localWall.transform.position = localPos;
                localWall.transform.rotation = Quaternion.Euler(0, 0, localRot);
                localWall.ScaleObj.GetComponent<Scalable>().ObjectToScale.transform.localScale = localScale;

                TextureData localTD = new TextureData();

                if (localSprite.TextureIndex == -1)
                {
                    localTD.myTexture = Resources.Load<Texture2D>("S_SimpleBox");
                    localTD.Hash = 1.ToString();
                }
                else if (localSprite.TextureIndex == -2)
                {
                    localTD.myTexture = Resources.Load<Texture2D>("S_SimpleCircle");
                    localTD.Hash = 2.ToString();
                }
                else
                    localTD = LoadingTextures[localSprite.TextureIndex];

                localWall.SetTextureData(localTD);
                localWall.ChangePivotAndPPU(localSprite.Pivot, localSprite.PPU);

                localWall.SetColor(localColor);

                switch (localColl)
                {
                    case CollisionType.Circle:

                        localWall.ChangeCollision(CollisionType.Circle);

                        if (localCollisionSize.x > 0)
                            localWall.CollisionOutline.transform.localScale = new Vector3(localCollisionSize.x, localCollisionSize.x, 1);


                        break;
                    case CollisionType.Box:

                        localWall.ChangeCollision(CollisionType.Box);


                        Vector3 localSize = localWall.CollisionOutline.transform.localScale;
                        if (localCollisionSize.x > 0)
                            localSize.x = localCollisionSize.x;

                        if (localCollisionSize.y > 0)
                            localSize.y = localCollisionSize.y;

                        localWall.CollisionOutline.transform.localScale = localSize;

                        break;
                    default:
                        break;
                }
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

                switch (propArr[0])
                {
                    case "Color":
                        
                        Color localColor = myObjectMenu.SideBallBlank.GetColor();

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

                        myObjectMenu.SideBallBlank.SetColor(localColor);

                        break;

                    case "StartSpeed":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        SideBallStartSpeed = localF;

                        break;

                    case "Sprite":


                        if (!int.TryParse(propArr[1], out localI))
                            break;

                        if (LoadingSprites.ContainsKey(localI))
                        {
                            SpriteData localSD = LoadingSprites[localI];
                            TextureData localTD;
                            if (localSD.TextureIndex == -1)
                                localTD = new TextureData(Resources.Load<Texture2D>("S_SimpleBox"), "", "1");
                            else if (localSD.TextureIndex == -2)
                                localTD = new TextureData(Resources.Load<Texture2D>("S_SimpleCircle"), "", "2");
                            else
                                localTD = LoadingTextures[localI];

                            myObjectMenu.SideBallBlank.SetTextureData(localTD);
                            myObjectMenu.SideBallBlank.ChangePivot(localSD.Pivot);
                            myObjectMenu.SideBallBlank.ChangePPU(localSD.PPU);

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

                switch (propArr[0])
                {
                    case "Color":

                        Color localColor = myObjectMenu.SmallBallBlank.GetColor();

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

                        myObjectMenu.SmallBallBlank.SetColor(localColor);

                        break;

                    case "StartSpeed":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        SmallBallStartSpeed = localF;

                        break;

                    case "Sprite":

                        if (!int.TryParse(propArr[1], out localI))
                            break;
                        
                        if (LoadingSprites.ContainsKey(localI))
                        {
                            SpriteData localSD = LoadingSprites[localI];
                            TextureData localTD;
                            if (localSD.TextureIndex == -1)
                                localTD = new TextureData(Resources.Load<Texture2D>("S_SimpleBox"), "", "1");
                            else if (localSD.TextureIndex == -2)
                                localTD = new TextureData(Resources.Load<Texture2D>("S_SimpleCircle"), "", "2");
                            else
                                localTD = LoadingTextures[localI];

                            myObjectMenu.SmallBallBlank.SetTextureData(localTD);
                            myObjectMenu.SmallBallBlank.ChangePivot(localSD.Pivot);
                            myObjectMenu.SmallBallBlank.ChangePPU(localSD.PPU);

                        }

                        break;

                    case "SpeedHitChange":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        SmallBallSpeedHitChange = localF;

                        break;

                    case "Reflects":

                        if (!int.TryParse(propArr[1], out localI))
                            break;

                        SmallBallReflects = localI;

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

            SpriteData TotalSD = new SpriteData(-1, 100, new Vector2(0.5f, 0.5f));

            Blank_MB MBSample = (Blank_MB)CreateBlank(E_LevelObjType.MainBall, true);


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

                            localVector = Vector3.zero;

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

                            MBSample.transform.position = localVector;

                            break;

                        case "StartRot":

                            if (float.TryParse(propArr[1], out localF))
                                MBSample.transform.rotation = Quaternion.Euler(0, 0, localF);

                            break;

                        case "Scale":

                            localVector = Vector3.one;

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

                            MBSample.ScaleObj.GetComponent<Scalable>().ObjectToScale.transform.localScale = localVector;

                            break;

                        case "Sprite":

                            if (int.TryParse(propArr[1], out localI))
                            {
                                if (LoadingSprites.ContainsKey(localI))
                                    TotalSD = LoadingSprites[localI];
                            }

                            break;

                        case "Color":
                            
                            localColor = MBSample.GetColor();

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

                            MBSample.SetColor(localColor);

                            break;

                        case "StartSpeed":

                            if (float.TryParse(propArr[1], out localF))
                                MBSample.StartSpeed = localF;
                            break;

                        case "SpeedHitChange":

                            if (float.TryParse(propArr[1], out localF))
                                MBSample.SpeedHitChange = localF;
                            break;

                        case "HitsAvarage":

                            if (int.TryParse(propArr[1], out localI))
                                MBSample.HitAverage = localI;
                            break;

                        case "HitsDelta":

                            if (int.TryParse(propArr[1], out localI))
                                MBSample.HitDelta = localI;
                            break;

                        case "SmallBalls":

                            if (int.TryParse(propArr[1], out localI))
                                MBSample.SmallBalls = localI;
                            break;

                        case "ExpAngle":

                            if (float.TryParse(propArr[1], out localF))
                                MBSample.ExpAngle = localF;
                            break;

                        case "CollisionRadius":

                            if (float.TryParse(propArr[1], out localF) && localF > 0)
                                MBSample.CollisionOutline.transform.localScale = new Vector3(localF, localF, 1);

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

                Blank_MB localMB = (Blank_MB)CreateBlank(E_LevelObjType.MainBall);

                localMB.transform.position = MBSample.transform.position;
                localMB.transform.rotation = MBSample.transform.rotation;

                localMB.ScaleObj.GetComponent<Scalable>().ObjectToScale.transform.localScale =
                    MBSample.ScaleObj.GetComponent<Scalable>().ObjectToScale.transform.localScale;

                localMB.SetColor(MBSample.GetColor());

                SpriteData localSD = TotalSD;

                localMB.StartSpeed = MBSample.StartSpeed;
                localMB.SpeedHitChange = MBSample.SpeedHitChange;
                localMB.SmallBalls = MBSample.SmallBalls;
                localMB.HitAverage = MBSample.HitAverage;
                localMB.HitDelta = MBSample.HitDelta;
                localMB.ExpAngle = MBSample.ExpAngle;      

                for (int ii = 0; ii < localArr.Length; ++ii)
                {
                    propArr = localArr[ii].Split('=');

                    if (propArr.Length != 2)
                        continue;

                    switch (propArr[0])
                    {
                        case "SpawnPoint":
                            propArr = propArr[1].Split('|');

                            localVector = localMB.transform.position;

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

                            localMB.transform.position = localVector;

                            break;

                        case "StartRot":

                            if (float.TryParse(propArr[1], out localF))
                                localMB.transform.rotation = Quaternion.Euler(0, 0, localF);

                            break;

                        case "Scale":

                            localVector = localMB.ScaleObj.GetComponent<Scalable>().ObjectToScale.transform.localScale;

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

                            localMB.ScaleObj.GetComponent<Scalable>().ObjectToScale.transform.localScale = localVector;

                            break;

                        case "Sprite":

                            if (int.TryParse(propArr[1], out localI))
                            {
                                if(LoadingSprites.ContainsKey(localI))
                                {
                                    localSD = LoadingSprites[localI];
                                }
                            }

                            break;

                        case "Color":

                            localColor = localMB.GetColor();

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

                            localMB.SetColor(localColor);

                            break;

                        case "StartSpeed":

                            if (float.TryParse(propArr[1], out localF))
                                localMB.StartSpeed = localF;
                            break;

                        case "SpeedHitChange":

                            if (float.TryParse(propArr[1], out localF))
                                localMB.SpeedHitChange = localF;
                            break;

                        case "HitsAvarage":

                            if (int.TryParse(propArr[1], out localI))
                                localMB.HitAverage = localI;
                            break;

                        case "HitsDelta":

                            if (int.TryParse(propArr[1], out localI))
                                localMB.HitDelta = localI;
                            break;

                        case "SmallBalls":

                            if (int.TryParse(propArr[1], out localI))
                                localMB.SmallBalls = localI;
                            break;

                        case "ExpAngle":

                            if (float.TryParse(propArr[1], out localF))
                                localMB.ExpAngle = localF;
                            break;

                        case "CollisionRadius":

                            if (float.TryParse(propArr[1], out localF) && localF > 0)
                                localMB.CollisionOutline.transform.localScale = new Vector3(localF, localF, 1);

                            break;

                        default:
                            break;
                    }

                }

                TextureData localTD;

                if (localSD.TextureIndex == -1)
                    localTD = new TextureData(Resources.Load<Texture2D>("S_SimpleBox"), "", "1");
                else if (localSD.TextureIndex == -2)
                    localTD = new TextureData(Resources.Load<Texture2D>("S_SimpleCircle"), "", "2");
                else
                    localTD = LoadingTextures[localSD.TextureIndex];

                localMB.SetTextureData(localTD);
                localMB.ChangePivotAndPPU(localSD.Pivot, localSD.PPU);

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
                        Vector2 localVector = myPlayer.transform.position;

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
                        myPlayer.transform.position = localVector;

                        break;

                    case "HP":

                        if (!int.TryParse(propArr[1], out localI))
                            break;

                        myPlayer.PlayerHP = localI;

                        break;

                    case "InvulDamageTime":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        myPlayer.InvulDamageTime = localF;

                        break;

                    case "InvulHitTime":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        myPlayer.InvulHitTime = localF;

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

                        //localSR = myPlayer.GetComponentInChildren<SpriteRenderer>();
                        //if (localSR == null)
                        //    break;
                        //
                        //if (!int.TryParse(propArr[1], out localI))
                        //    break;
                        //
                        //Sprite localS = mySprites[localI];
                        //if (localS != null)
                        //{
                        //    localSR.sprite = localS;
                        //}
                        //
                        //break;



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

                        Color localColor = myCamera.backgroundColor;

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

                        myCamera.backgroundColor = localColor;

                        break;

                    case "PointsMultiplyer":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        PointsMultiplyer = localF;

                        break;

                    case "TimeSpeed":

                        if (!float.TryParse(propArr[1], out localF))
                            break;

                        TimeSpeed = localF;

                        break;

                    case "SideBallStartSpawn":

                        if (!int.TryParse(propArr[1], out localI))
                            break;

                        SideBallStartSpawn = localI;

                        break;

                    case "SmallBallStartSpawn":

                        if (!int.TryParse(propArr[1], out localI))
                            break;

                        SmallBallStartSpawn = localI;

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

                        CameraBorder.transform.localScale = new Vector3(localF, localF,1);

                        break;

                    case "SnapToPlayer":

                        if (!bool.TryParse(propArr[1], out localB))
                            break;

                        SnapCameraToPlayer = localB;


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
                                AverageTime = localF;
                            break;

                        case "DeltaTime":
                            if (float.TryParse(propArr[1], out localF))
                                DeltaTime = localF;
                            break;


                        default:
                            break;
                    }
                }
            }

            stringLines = stringLines[1].Split('[');

            myEvents.Clear();

            for (int i = 0; i < stringLines.Length; ++i)
            {
                if (stringLines[i].IndexOf('(') == -1 && stringLines[i].IndexOf('(') >= stringLines[i].IndexOf(')'))
                    continue;

                stringLines[i] = stringLines[i].Substring(stringLines[i].IndexOf('(') + 1, stringLines[i].IndexOf(')') - stringLines[i].IndexOf('(') - 1);

                localArr = stringLines[i].Split(',');
                EventsData localED = new EventsData();
                for (int ii = 0; ii < localArr.Length; ++ii)
                {
                    propArr = localArr[ii].Split('=');
                    if (propArr.Length != 2)
                        continue;

                    

                    switch (propArr[0])
                    {
                        case "EventType":

                            switch (propArr[1])
                            {
                                case "Line":
                                    localED.myType = EventsData.E_EventType.Line;
                                    break;

                                case "Slide":
                                    localED.myType = EventsData.E_EventType.Slide;
                                    break;

                                case "Circle":
                                    localED.myType = EventsData.E_EventType.Circle;
                                    break;

                                default:
                                    break;
                            }

                            break;

                        case "Num":
                            if (int.TryParse(propArr[1], out localI))
                                localED.Num = localI;
                            break;

                        case "a":
                            if (float.TryParse(propArr[1], out localF))
                                localED.a = localF;
                            break;

                        case "b":
                            if (float.TryParse(propArr[1], out localF))
                                localED.b = localF;
                            break;

                        case "c":
                            if (float.TryParse(propArr[1], out localF))
                                localED.c = localF;
                            break;

                        default:
                            break;
                    }

                    
                    
                    


                }

                myEvents.Add(localED);
            }

            return true;
        }



        public TextureData GetTextureData()
        {
            return new TextureData();
        }
        public void SetTextureData(TextureData inTD)
        {
            
        }
        public void ChangePivot(Vector2 inPivot)
        {
            
        }
        public void ChangePPU(float PPU)
        {
            
        }
        public Sprite GetSprite()
        {
            return null;
        }
        public void SetColor(Color inColor)
        {
            myCamera.backgroundColor = inColor;
        }
        public Color GetColor()
        {
            return myCamera.backgroundColor;
        }
    }
}
