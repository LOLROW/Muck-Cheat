using System;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using System.Runtime.Remoting.Channels;
using System.Threading;
using UnityEngine;
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace Muck_Cheat
{
    public class Loader
    {
        public static void Init()
        {
            Loader.Load = new GameObject();
            Loader.Load.AddComponent<Functions>();
            UnityEngine.Object.DontDestroyOnLoad(Loader.Load);
        }
        public static GameObject Load;
    }

    public class Functions : MonoBehaviour
    {
        public void Start()
        {
            inventoryHack = FindObjectOfType<InventoryUI>();
            itemHack = FindObjectOfType<ItemManager>();
            playerHack = FindObjectOfType<PlayerStatus>();
            powerUpHack = FindObjectOfType<PowerupInventory>();
            dayHack = FindObjectOfType<DayCycle>();
            mobHack = FindObjectOfType<MobSpawner>();
            MobManagerHack = FindObjectOfType<MobManager>();

            audioSource = gameObject.AddComponent<AudioSource>();
            powerUpSound = powerUpHack.goodPowerupSfx;

            audioSource.clip = powerUpSound;
            audioSource.Play();
            ParticleSystem c = Object.Instantiate<GameObject>(powerUpHack.powerupFx, playerHack.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
#pragma warning disable 618
            c.startLifetime = 10;
            c.startColor = Color.magenta;
#pragma warning restore 618
        }
        
        //GUI CODE
        private bool menuOpen = false;
        private bool mainFeatiresTab = true;
        private bool powerUpTab = false;
        private bool inventoryTab = false;
        
        //bools for features
        private bool godMode = false;
        private bool infStam = false;
        private float timeScaleAmount = 0.01f;
        private bool timeScale = false;
        private bool hungerMode = false;
        private bool alwaysDay = false;
        private bool enemySpawner = false; 
        
        //powerup vars
        private int amountOfPowerUps = 1;
        private int powerUpIndex = 0;
        
        //inventory vars
        private int amountOfItem = 1;
        private Vector2 scrollPosition = Vector2.zero;
        
        //Mob vars
        private Vector2 scrollPosition1 = Vector2.zero;
        private String[] mobNames = {"Cow", "Fire Dave","Electric Dave", "lil Dave", "Water Dave", "Goblin", "Rock Man", "Gronk", "Dragon", "Chunky Man", "Gronk", "Wolf"};
        private int mob = 10000;
        private int mobIndex = 0;
        private int amountOfMobs = 1;
        private int damageMultiplier = 1;
        
        //audio
        private AudioSource audioSource;
        private AudioClip powerUpSound;

        void OnGUI()
        {
            if (menuOpen)
            {
                //center of screen vars
                int baseX = (Screen.width / 2) - 100;
                int baseY = (Screen.height / 2) - 100;
                
                //main Gui config
                if(GUI.Button(new Rect(baseX-95,baseY,95,40), "Main Stuff"))
                {
                    mainFeatiresTab = true;
                    powerUpTab = false;
                    inventoryTab = false;
                    enemySpawner = false;
                }
                if(GUI.Button(new Rect(baseX-95,baseY+40,95,40), "PowerUp stuff"))
                {
                    mainFeatiresTab = false;
                    powerUpTab = true;
                    inventoryTab = false;
                    enemySpawner = false;
                }
                if(GUI.Button(new Rect(baseX-95,baseY+80,95,40), "Inventory stuff"))
                {
                    mainFeatiresTab = false;
                    powerUpTab = false;
                    inventoryTab = true;
                    enemySpawner = false;
                }
                if(GUI.Button(new Rect(baseX-95,baseY+120,95,40), "Enemy stuff"))
                {
                    mainFeatiresTab = false;
                    powerUpTab = false;
                    inventoryTab = false;
                    enemySpawner = true;
                }
                
                if (mainFeatiresTab)
                {
                    GUI.Box(new Rect (baseX,baseY,200,400), "MUCK CHEAT");
                    
                    // GodMode
                    if(GUI.Button(new Rect(baseX+20,baseY+20,135,20), "GOD MODE"))
                    {
                        godMode = !godMode;
                    }
            
                    // Stamina
                    if(GUI.Button(new Rect(baseX+20,baseY+40,135,20), "INFINITE STAMINA"))
                    {
                        //GUI.Label(new Rect(baseX, baseY, 200, 400), "sfdgggdfgsdfgsdfgsdfgsdfgsdfgdsgdsfg");
                        infStam = !infStam;
                        if (!infStam)
                        {
                            StaminaMethod(10f);
                        }
                    }
                
                    //timescale
                    if (GUI.Button(new Rect(baseX+20, baseY+85, 135, 20), "TIME SCALE"))
                    {
                        timeScale = !timeScale;
                        if (!timeScale)
                        {
                            TimeSpeedMethod(0.01f);
                        }
                    }

                    // Infinite hunger 
                    if (GUI.Button(new Rect(baseX+20, baseY+110, 135, 20), "INFINITE HUNGER"))
                    {
                        hungerMode = !hungerMode;
                        if (!hungerMode)
                        {
                            Hunger(50);
                        }
                    }

                    // Always day (set day as well)
                    if (GUI.Button(new Rect(baseX+20, baseY+135, 135, 20), "ALWAYS DAY"))
                    {
                        alwaysDay = !alwaysDay;
                        if (!alwaysDay)
                        {
                            AlwaysDay(false);
                        }
                    }
                    
                    //kill all mobs
                    if (GUI.Button(new Rect(baseX+20, baseY+160, 135, 20), "KILL ALL MOBS"))
                    {

                        foreach (HitableMob Mob in GameObject.FindObjectsOfType<HitableMob>())
                        {
                            Mob.Hit(9999, 9999, 1, Vector3.zero);
                        }
                    }

                    if (GUI.Button(new Rect(baseX + 20, baseY + 180, 135, 20), "RESPAWN"))
                    {
                        Respawn();
                    }
                    
                    timeScaleAmount = GUI.HorizontalSlider(new Rect(baseX+20, baseY+60, 135, 20), timeScaleAmount, 0.01F, 500.0F);
                    timeScaleAmount = Int32.Parse(GUI.TextField(new Rect(baseX+155, baseY+60, 30, 20), timeScaleAmount+"", 25));
                    
                }

                if (powerUpTab)
                {
                    GUI.Box(new Rect (baseX,baseY,595,400), "MUCK CHEAT");
                    
                    amountOfPowerUps = (int)GUI.HorizontalSlider(new Rect(baseX+20, baseY+25, 135, 20), amountOfPowerUps, 1f, 100f);
                    amountOfPowerUps = Int32.Parse(GUI.TextField(new Rect(baseX+155, baseY+20, 30, 20), amountOfPowerUps+"", 25));

                    if(GUI.Button(new Rect(baseX+200,baseY+20,135,20), "Give All"))
                    {
                        powerUpIndex = 0;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                        
                        Invoke("AddOneToIndex",0.1f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0.1f);
                        }
                        
                        Invoke("AddOneToIndex",0.2f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0.2f);
                        }
                        
                        Invoke("AddOneToIndex",0.3f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0.3f);
                        }
                        
                        Invoke("AddOneToIndex",0.4f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0.4f);
                        }
                        
                        Invoke("AddOneToIndex",0.5f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0.5f);
                        }
                        
                        Invoke("AddOneToIndex",0.6f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0.6f);
                        }
                        
                        Invoke("AddOneToIndex",0.7f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0.7f);
                        }
                        
                        Invoke("AddOneToIndex",0.8f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0.8f);
                        }
                        
                        Invoke("AddOneToIndex",0.9f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0.9f);
                        }
                        
                        Invoke("AddOneToIndex",1.0f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",1.0f);
                        }
                        
                        Invoke("AddOneToIndex",1.1f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",1.1f);
                        }
                        
                        Invoke("AddOneToIndex",1.2f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",1.2f);
                        }
                        
                        Invoke("AddOneToIndex",1.3f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",1.3f);
                        }
                        
                        Invoke("AddOneToIndex",1.4f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",1.4f);
                        }
                        
                        Invoke("AddOneToIndex",1.5f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",1.5f);
                        }
                        
                        Invoke("AddOneToIndex",1.6f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",1.6f);
                        }
                        
                        Invoke("AddOneToIndex",1.7f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",1.7f);
                        }
                        
                        Invoke("AddOneToIndex",1.8f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",1.8f);
                        }
                        
                        Invoke("AddOneToIndex",1.9f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",1.9f);
                        }
                        
                        Invoke("AddOneToIndex",2.0f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",2.0f);
                        }
                        
                        Invoke("AddOneToIndex",2.1f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",2.1f);
                        }
                        
                        Invoke("AddOneToIndex",2.2f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",2.2f);
                        }
                        
                        Invoke("AddOneToIndex",2.3f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",2.3f);
                        }
                        
                        Invoke("AddOneToIndex",2.4f);
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",2.4f);
                        }
                        
                        
                    }
                    //row 1
                    if(GUI.Button(new Rect(baseX+20,baseY+40,135,20), "Broccoli"))
                    {
                        powerUpIndex = 0;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+20,baseY+60,135,20), "dumbbell"))
                    {
                        powerUpIndex = 1;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+20,baseY+80,135,20), "jetpack"))
                    {
                        powerUpIndex = 2;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+20,baseY+100,135,20), "orange goose"))
                    {
                        powerUpIndex = 3;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+20,baseY+120,135,20), "nut spread"))
                    {
                        powerUpIndex = 4;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+20,baseY+140,135,20), "blue pill"))
                    {
                        powerUpIndex = 5;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    //row 2
                    if(GUI.Button(new Rect(baseX+155,baseY+40,135,20), "red pill"))
                    {
                        powerUpIndex = 6;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+155,baseY+60,135,20), "sneaker"))
                    {
                        powerUpIndex = 7;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+155,baseY+80,135,20), "robin hood"))
                    {
                        powerUpIndex = 8;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+155,baseY+100,135,20), "bean"))
                    {
                        powerUpIndex = 9;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+155,baseY+120,135,20), "cargo truck thingy"))
                    {
                        powerUpIndex = 10;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+155,baseY+140,135,20), "horsue thing"))
                    {
                        powerUpIndex = 11;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    //row 3
                    if(GUI.Button(new Rect(baseX+290,baseY+40,135,20), "milk"))
                    {
                        powerUpIndex = 12;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+290,baseY+60,135,20), "piggy bank"))
                    {
                        powerUpIndex = 13;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+290,baseY+80,135,20), "crimson dagger"))
                    {
                        powerUpIndex = 14;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+290,baseY+100,135,20), "dracula"))
                    {
                        powerUpIndex = 15;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+290,baseY+120,135,20), "frog"))
                    {
                        powerUpIndex = 16;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+290,baseY+140,135,20), "juice"))
                    {
                        powerUpIndex = 17;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    //row 4
                    if(GUI.Button(new Rect(baseX+425,baseY+40,135,20), "adrenaline"))
                    {
                        powerUpIndex = 18;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+425,baseY+60,135,20), "viking helment"))
                    {
                        powerUpIndex = 19;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+425,baseY+80,135,20), "checkered shirt"))
                    {
                        powerUpIndex = 20;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+425,baseY+100,135,20), "sniper scope"))
                    {
                        powerUpIndex = 21;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+425,baseY+120,135,20), "hammer"))
                    {
                        powerUpIndex = 22;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+425,baseY+140,135,20), "wings"))
                    {
                        powerUpIndex = 23;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    if(GUI.Button(new Rect(baseX+425,baseY+160,135,20), "bazooka thingy"))
                    {
                        powerUpIndex = 24;
                        for (int i = 0; i < amountOfPowerUps; i++)
                        {
                            Invoke("PowerUpMethod",0f);
                        }
                    }
                    
                }

                if (inventoryTab)
                {
                    GUI.Box(new Rect (baseX, baseY,805,500), "MUCK CHEAT");
                    
                    amountOfItem = (int)GUI.HorizontalSlider(new Rect(baseX+20, baseY+25, 135, 20), amountOfItem, 1f, 100f);
                    amountOfItem = Int32.Parse(GUI.TextField(new Rect(baseX+155, baseY+20, 30, 20), amountOfItem+"", 25)); 
                    
                    scrollPosition = GUI.BeginScrollView(new Rect(baseX, baseY+25, 795, 500), scrollPosition, new Rect(0, 0, 800, 10000));

                    int x = 20;
                    int y = 40;
                    for (int i = 0; i < itemHack.allScriptableItems.Length; i++)
                    {
                        InventoryItem item = itemHack.allScriptableItems[i];

                        if (i!=0)
                        {
                            x += 80;
                            if (x >= 700)
                            {
                                x = 20;
                                y += 80;
                            }
                        }

                        if (GUI.Button(new Rect(x, y, 80, 80), new GUIContent(item.sprite.texture, "Spawn " + item.name)))
                            ClientSend.DropItem(item.id, amountOfItem);
                    }
                    GUI.EndScrollView();
                }

                if (enemySpawner)
                {
                    GUI.Box(new Rect (baseX, baseY,805,500), "MUCK CHEAT");
                    
                    amountOfMobs = (int)GUI.HorizontalSlider(new Rect(baseX+20, baseY+25, 135, 20), amountOfMobs, 1f, 100f);
                    amountOfMobs = Int32.Parse(GUI.TextField(new Rect(baseX+155, baseY+20, 30, 20), amountOfMobs+"", 25)); 
                    
                    scrollPosition1 = GUI.BeginScrollView(new Rect(baseX, baseY+25, 795, 500), scrollPosition1, new Rect(0, 0, 800, 10000));

                    int x = 20;
                    int y = 40;
                    for (int i = 0; i < mobHack.allMobs.Length; i++)
                    {
                        if (i!=0)
                        {
                            x += 80;
                            if (x >= 700)
                            {
                                x = 20;
                                y += 80;
                            }
                        }

                        if (GUI.Button(new Rect(x, y, 80, 80), new GUIContent(mobNames[i], "Spawn " + mobNames[i])))
                        {
                            mobIndex = i;
                            for (int j = 0; j < amountOfMobs; j++)
                            {
                                Invoke("addToIndexMob", 0.1f);
                                Invoke("SpawnMob",0.1f);
                            }
                        }
                    }
                    GUI.EndScrollView();
                }
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                if (!menuOpen)
                {
                    menuOpen = true;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    menuOpen = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
            foreach (Mob mobe in FindObjectsOfType<Mob>())
            {
                mobe.gameObject.AddComponent<HitableMob>().mob = mobe;
            }

            if (godMode)
            {
                playerHack.Heal(Int32.MaxValue);
                playerHack.maxHp += 10000;
            }

            if (infStam)
            {
                StaminaMethod(999999999);
            }

            if (timeScale)
            {
                TimeSpeedMethod(timeScaleAmount);
            }

            if (hungerMode)
            {
                Hunger(99999f);
            }

            if (alwaysDay)
            {
                AlwaysDay(true);
            }
        }

        public void StaminaMethod(float amount)
        {
            playerHack.stamina = amount;
        }

        public void PowerUpMethod()
        {
            //UiEvents.Instance.AddPowerup(ItemManager.Instance.allPowerups[powerUpIndex]);
            PlayerStatus.Instance.UpdateStats();
            PowerupUI.Instance.AddPowerup(powerUpIndex);
            //powerUpHack.AddPowerup("gave this guy a powerup cuz im cool like that", powerUpIndex, 1);
        }

        public void AddOneToIndex()
        {
            powerUpIndex++;
        }

        public void TimeSpeedMethod(float val)
        {
            dayHack.timeSpeed = val;
        }

        /*public void Health()
        {
            playerHack.Heal(9999);
        }*/

        public void Hunger(float f)
        {
            playerHack.maxHunger = f;
            playerHack.hunger = f;
        }

        public void AlwaysDay(bool b)
        {
            dayHack.alwaysDay = b;
        }

        public void SpawnMob()
        {
            mobHack.SpawnMob(playerHack.transform.position, mobIndex, mob, damageMultiplier, damageMultiplier);
        }

        public void addToIndexMob()
        {
            mob++;
        }

        public void Respawn()
        {
            playerHack.Respawn();
        }

        public InventoryUI inventoryHack;
        public ItemManager itemHack;
        public PlayerStatus playerHack;
        public PowerupInventory powerUpHack;
        public DayCycle dayHack;
        public PlayerMovement playerMovement;
        public MobSpawner mobHack;
        public MobManager MobManagerHack;
        //public int i = 0;
    }
}