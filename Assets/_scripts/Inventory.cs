using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
/// All the stuff a player is holding.
public class Inventory {
    public HashSet<Collectible> items = new HashSet<Collectible>();

    private GameManager gameManager = GameManager.getGlobalSingletonGameManager();
    public int inventorySize, equippedTrapIndex;
    public TrapType equippedTrap {
        get => gameManager.gameConstants.trapTypes[equippedTrapIndex];
        set => equippedTrapIndex = value.uniqueId;
    }
    public bool hasBriefcase = false;
    public Dictionary<CollectibleType, int> collectibles;
    public Dictionary<TrapType, int> traps;

    private TrapTypeRegistry trapTypes {
        get => gameManager.gameConstants.trapTypes;
    }
    private CollectibleTypeRegistry collectibleTypes {
        get => gameManager.gameConstants.collectibleTypes;
    }

    public Inventory() {
        inventorySize = gameManager.gameConstants.playerMaxInventorySpace;
        traps = new Dictionary<TrapType, int>();
        foreach(var trapType in trapTypes) {
            traps.Add(trapType, 0);
        }
        collectibles = new Dictionary<CollectibleType, int>();
        foreach(var collectibleType in collectibleTypes) {
            collectibles.Add(collectibleType, 0);
        }
    }

    /// Total number of collectibles held.  Sum of quantity held of each type.
    public int CollectiblesHeldCount {
        get => (from count in collectibles.Values select count).Sum();
    }
    public int MaxCarryingCapacity {
        get {
            var maxCanBeHeld = inventorySize;
            if(hasBriefcase) maxCanBeHeld += gameManager.gameConstants.briefcaseGrantsExtraInventorySpace;
            return maxCanBeHeld;
        }
    }
    public bool CanHoldMoreCollectibles()
    {
        return MaxCarryingCapacity > CollectiblesHeldCount;
    }

    public bool CanEquipTrap(TrapType trapType)
    {
        if(trapType.isNoneTrap) return true;
        return HasTrap(trapType);
    }
    public bool HasTrap(TrapType type)
    {
        return traps[type] > 0;
    }
    public void UseTrap(TrapType type)
    {
        if ( traps[type] > 0 )
        {
            traps[type] -= 1;
        }
    }

    public void RemoveCollectible(CollectibleType type)
    {
        if(collectibles[type] > 0)
        collectibles[type] -= 1;
        }

    //0 is none: the number of that trap in inventory
    public void AddTrap(TrapType type, int count)
    {
        traps[type] += count;
    }
    public void SetTrapCount(TrapType type, int count)
    {
      traps[type] = count;
    }

    public void AddCollectible(CollectibleType type, int count)
    {
      collectibles[type] = count;
    }

    public string GetTrapsString()
    {
      string trapstring = "";
      foreach(var el in traps)
      {
        trapstring += el.Value.ToString();
      }
      return trapstring;
    }

    public void UpdateInventoryHud()
    {
        {
            IconRowHUD inventoryImages = gameManager.playertrapimages;
        int index = 0;
        foreach(var type in trapTypes) {
            var count = traps[type];
            inventoryImages.setIconVisibility(index, count > 0);
            inventoryImages.setIcon(index, type.sprite);
            if(equippedTrap == index) {
                inventoryImages.setCursorVisibility(true);
                inventoryImages.setCursorPosition(index);
            }
            index++;
        }
        }
        {
            IconRowHUD inventoryImages = gameManager.playerinventoryimages;
        int index = 0;
        int slot = 0;
        inventoryImages.setCursorVisibility(false);
        foreach(var type in collectibleTypes) {
            var count = collectibles[type];
            if(count > 0) {
                inventoryImages.setIconVisibility(slot, true);
                inventoryImages.setIcon(slot, type.sprite);
                slot++;
            }
            index++;
        }
        for(; slot < inventoryImages.getIconCount(); slot++) {
            inventoryImages.setIconVisibility(slot, false);
        }
        }

    //   //if change traps is true set the list to the traps inventory list
    //   Dictionary<Object, int> tempInventory = changeTraps ? traps : collectibles;
    //   List<Sprite> tempSpriteList = (changeTraps ? from type in gameManager.gameConstants.trapTypes select type.sprite : from type in gameManager.gameConstants.collectibleTypes select type.sprite).ToList();
    //   int count = 1;
    //   int slotsUsed = 0;
    //   while(slotsUsed < inventoryImages.childCount && count < tempInventory.Count)
    //   {
    //     if(tempInventory[count] > 0)
    //     {
    //       inventoryImages.GetChild(slotsUsed).GetComponent<RawImage>().texture = tempSpriteList[count].texture;
    //       slotsUsed ++;
    //     }
    //     count++;
    //   }
    //   //set the rest of the sprites to blank
    //   while(slotsUsed <  inventoryImages.childCount) {inventoryImages.GetChild(slotsUsed).GetComponent<RawImage>().texture = tempSpriteList[0].texture;   slotsUsed ++;}
    }

}
