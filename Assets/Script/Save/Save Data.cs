using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public struct SaveData
{
    public static SaveData Instance;

    //map stuff
    public HashSet<string> sceneNames;
    //bench stuff
    public string benchSceneName;
    public Vector2 benchPos;


    //player stuff
    public int playerHealth;
    public int playerMaxHealth;
    public int playerHeartShards;
    public float playerMana;
    public bool playerHalfMana;
    public Vector2 playerPosition;
    public string lastScene;

    public bool playerUnlockedWallJump, playerUnlockedDash, playerUnlockedVarJump, playerUnlockedHeal ,playerUnlockedCast;

    //enemies stuff
    //shade
    public Vector2 shadePos;
    public string sceneWithShade;
    public Quaternion shadeRot;


    public void Initialize()
    {
        if (!File.Exists(Application.persistentDataPath + "/save.bench.data")) 
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.bench.data"));
        }
        if (!File.Exists(Application.persistentDataPath + "/save.player.data")) 
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.player.data"));
        }
        if (!File.Exists(Application.persistentDataPath + "/save.shade.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.shade.data"));
        }
        if (sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }
    #region Bench Stuff
    public void SaveBench()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.bench.data")))
        {
            writer.Write(benchSceneName);
            writer.Write(benchPos.x);
            writer.Write(benchPos.y);
        }
        //UnityEngine.Debug.Log("Saved Bench data: " + benchSceneName + " at " + benchPos);
    }
    public void LoadBench()
    {
        string filepath = Application.persistentDataPath + "/save.bench.data";
        if (File.Exists(filepath))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.bench.data")))
            {
                benchSceneName = reader.ReadString();
                benchPos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            }
            //UnityEngine.Debug.Log("Loaded Bench data: " + benchSceneName + " at " + benchPos);
        }
        else
        {
            UnityEngine.Debug.Log("Bench doesnt exist");
        }
    }
    #endregion
    #region Player stuff
    public void SavePlayerData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.player.data")))
        {
            int playerHealth = PlayerController.Instance.Health;
            writer.Write(playerHealth);

            int playerMaxHealth = PlayerController.Instance.maxHealth;
            writer.Write(playerMaxHealth);

            int playerHeartShards = PlayerController.Instance.heartShards;
            writer.Write(playerHeartShards);

            float playerMana = PlayerController.Instance.Mana;
            writer.Write(playerMana);

            bool playerHalfMana = PlayerController.Instance.halfMana;
            writer.Write(playerHalfMana);

            bool playerUnlockedWallJump = PlayerController.Instance.unlockedWallJump;
            writer.Write(playerUnlockedWallJump);

            bool playerUnlockedDash = PlayerController.Instance.unlockedDash;
            writer.Write(playerUnlockedDash);

            bool playerUnlockedVarJump = PlayerController.Instance.unlockedVarJump;
            writer.Write(playerUnlockedVarJump);

            bool playerUnlockedHeal = PlayerController.Instance.unlockedHeal;
            writer.Write(playerUnlockedHeal);

            bool playerUnlockedCast = PlayerController.Instance.unlockedCastSpell;
            writer.Write(playerUnlockedCast);

            Vector3 playerPosition = PlayerController.Instance.transform.position;
            writer.Write(playerPosition.x);
            writer.Write(playerPosition.y);

            string lastScene = SceneManager.GetActiveScene().name;
            writer.Write(lastScene);
        }
        //UnityEngine.Debug.Log("Saved player data");
    }
    public void LoadPlayerData()
    {
        string filePath = Application.persistentDataPath + "/save.player.data";
        if (File.Exists(filePath))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.player.data")))
            {
                int playerHealth = reader.ReadInt32();

                int playerMaxHealth = reader.ReadInt32();

                int playerHeartShards = reader.ReadInt32();

                float playerMana = reader.ReadSingle();

                bool playerHalfMana = reader.ReadBoolean();

                bool playerUnlockedWallJump = reader.ReadBoolean();

                bool playerUnlockedDash = reader.ReadBoolean();

                bool playerUnlockedVarJump = reader.ReadBoolean();

                bool playerUnlockedHeal = reader.ReadBoolean();

                bool playerUnlockedCast = reader.ReadBoolean();

                Vector3 playerPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), 0);

                string lastScene = reader.ReadString();

                SceneManager.LoadScene(lastScene);

                PlayerController.Instance.transform.position = playerPosition;

                PlayerController.Instance.Health = playerHealth;
                PlayerController.Instance.maxHealth = playerMaxHealth;
                PlayerController.Instance.heartShards = playerHeartShards;
                PlayerController.Instance.Mana = playerMana;
                PlayerController.Instance.halfMana = playerHalfMana;
                PlayerController.Instance.unlockedWallJump = playerUnlockedWallJump;
                PlayerController.Instance.unlockedDash = playerUnlockedDash;
                PlayerController.Instance.unlockedVarJump = playerUnlockedVarJump;
                PlayerController.Instance.unlockedHeal = playerUnlockedHeal;
                PlayerController.Instance.unlockedCastSpell = playerUnlockedCast;

                GlobalController.instance.LoadPlayerScore();
            }
        }
        else
        {
            UnityEngine.Debug.Log("Can't find player data");
            PlayerController.Instance.ResetToDefault();
        }
    }

    #endregion
    #region enemy stuff
    public void SaveShadeData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.shade.data")))
        {
            sceneWithShade = SceneManager.GetActiveScene().name;
            shadePos = Shade.Instance.transform.position;
            shadeRot = Shade.Instance.transform.rotation;

            writer.Write(sceneWithShade);

            writer.Write(shadePos.x);
            writer.Write(shadePos.y);

            writer.Write(shadeRot.x);
            writer.Write(shadeRot.y);
            writer.Write(shadeRot.z);
            writer.Write(shadeRot.w);
        }
    }
    public void LoadShadeData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.shade.data"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.shade.data")))
            {
                sceneWithShade = reader.ReadString();
                shadePos.x = reader.ReadSingle();
                shadePos.y = reader.ReadSingle();

                float rotationX = reader.ReadSingle();
                float rotationY = reader.ReadSingle();
                float rotationZ = reader.ReadSingle();
                float rotationW = reader.ReadSingle();
                shadeRot = new Quaternion(rotationX, rotationY, rotationZ, rotationW);
            }
        }
        else
        {
            UnityEngine.Debug.Log("Shade doesn't exit");
        }
    }
    #endregion
}
