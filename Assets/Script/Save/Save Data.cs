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
        if (!File.Exists(Application.persistentDataPath + "/save.bench.data")) //if file doesnt exist, well create the file
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.bench.data"));
        }
        if (!File.Exists(Application.persistentDataPath + "/save.player.data")) //if file doesnt exist, well create the file
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.player.data"));
        }
        if (!File.Exists(Application.persistentDataPath + "/save.shade.data")) //if file doesnt exist, well create the file
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
        UnityEngine.Debug.Log("Saved Bench data: " + benchSceneName + " at " + benchPos);
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
            UnityEngine.Debug.Log("Loaded Bench data: " + benchSceneName + " at " + benchPos);
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
            // Lưu dữ liệu sức khỏe của người chơi
            int playerHealth = PlayerController.Instance.Health;
            writer.Write(playerHealth);

            // Lưu dữ liệu sức khỏe tối đa của người chơi
            int playerMaxHealth = PlayerController.Instance.maxHealth;
            writer.Write(playerMaxHealth);

            // Lưu số lượng mảnh trái tim người chơi đã thu thập
            int playerHeartShards = PlayerController.Instance.heartShards;
            writer.Write(playerHeartShards);

            // Lưu lượng mana của người chơi
            float playerMana = PlayerController.Instance.Mana;
            writer.Write(playerMana);

            // Lưu trạng thái nửa mana của người chơi
            bool playerHalfMana = PlayerController.Instance.halfMana;
            writer.Write(playerHalfMana);

            // Lưu trạng thái đã mở khả năng nhảy tường của người chơi
            bool playerUnlockedWallJump = PlayerController.Instance.unlockedWallJump;
            writer.Write(playerUnlockedWallJump);

            // Lưu trạng thái đã mở khả năng dash của người chơi
            bool playerUnlockedDash = PlayerController.Instance.unlockedDash;
            writer.Write(playerUnlockedDash);

            // Lưu trạng thái đã mở khả năng nhảy biến đổi của người chơi
            bool playerUnlockedVarJump = PlayerController.Instance.unlockedVarJump;
            writer.Write(playerUnlockedVarJump);

            // Lưu trạng thái đã mở khả năng hồi phục của người chơi
            bool playerUnlockedHeal = PlayerController.Instance.unlockedHeal;
            writer.Write(playerUnlockedHeal);

            // Lưu trạng thái đã mở khả năng phép thuật của người chơi
            bool playerUnlockedCast = PlayerController.Instance.unlockedCastSpell;
            writer.Write(playerUnlockedCast);

            // Lưu vị trí của người chơi
            Vector3 playerPosition = PlayerController.Instance.transform.position;
            writer.Write(playerPosition.x);
            writer.Write(playerPosition.y);

            // Lưu tên của scene cuối cùng người chơi đã chơi
            string lastScene = SceneManager.GetActiveScene().name;
            writer.Write(lastScene);
        }
        UnityEngine.Debug.Log("Đã lưu dữ liệu người chơi");


    }
    public void LoadPlayerData()
    {
        string filePath = Application.persistentDataPath + "/save.player.data";
        if (File.Exists(filePath))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.player.data")))
            {

                // Đọc dữ liệu sức khỏe của người chơi
                int playerHealth = reader.ReadInt32();

                // Đọc dữ liệu sức khỏe tối đa của người chơi
                int playerMaxHealth = reader.ReadInt32();

                // Đọc số lượng mảnh trái tim người chơi đã thu thập
                int playerHeartShards = reader.ReadInt32();

                // Đọc lượng mana của người chơi
                float playerMana = reader.ReadSingle();

                // Đọc trạng thái nửa mana của người chơi
                bool playerHalfMana = reader.ReadBoolean();

                // Đọc trạng thái đã mở khả năng nhảy tường của người chơi
                bool playerUnlockedWallJump = reader.ReadBoolean();

                // Đọc trạng thái đã mở khả năng dash của người chơi
                bool playerUnlockedDash = reader.ReadBoolean();

                // Đọc trạng thái đã mở khả năng nhảy biến đổi của người chơi
                bool playerUnlockedVarJump = reader.ReadBoolean();

                // Đọc trạng thái đã mở khả năng hồi phục của người chơi
                bool playerUnlockedHeal = reader.ReadBoolean();

                // Đọc trạng thái đã mở khả năng phép thuật của người chơi
                bool playerUnlockedCast = reader.ReadBoolean();

                // Đọc vị trí của người chơi
                Vector3 playerPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), 0);

                // Đọc tên của scene cuối cùng người chơi đã chơi
                string lastScene = reader.ReadString();

                // Load scene cuối cùng mà người chơi đã chơi
                SceneManager.LoadScene(lastScene);

                // Thiết lập vị trí của người chơi
                PlayerController.Instance.transform.position = playerPosition;

                // Thiết lập lại trạng thái của người chơi
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

                UnityEngine.Debug.Log("Đã tải dữ liệu người chơi thành công");
                GlobalController.instance.LoadPlayerScore();
            }
        }
        else
        {
            UnityEngine.Debug.Log("Không tìm thấy tệp dữ liệu người chơi");

            // Thiết lập lại dữ liệu người chơi về mặc định nếu không tìm thấy tệp
            PlayerController.Instance.ResetToDefault();
            /*
            PlayerController.Instance.halfMana = false;
            PlayerController.Instance.Health = PlayerController.Instance.maxHealth;
            PlayerController.Instance.Mana = 0.5f;
            PlayerController.Instance.heartShards = 0;

            PlayerController.Instance.unlockedWallJump = false;
            PlayerController.Instance.unlockedDash = false;
            PlayerController.Instance.unlockedVarJump = false;
            PlayerController.Instance.unlockedHeal = false;
            PlayerController.Instance.unlockedCastSpell = false;
            */
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
            UnityEngine.Debug.Log("Saved Bench data");
        }
        else
        {
            UnityEngine.Debug.Log("Shade doeesn't exit");
        }
    }
    #endregion
}
