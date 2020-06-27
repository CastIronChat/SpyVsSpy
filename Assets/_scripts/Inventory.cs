using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// All the stuff a player is holding.
public class Inventory  {
    public HashSet<Collectible> items = new HashSet<Collectible>();

    public int inventorySize = 3,equippedTrap;
    public bool hasBriefcase = false;
    public List<int> collectibles,traps;



    public bool CanHoldMoreCollectibles()
    {
      if(inventorySize > collectibles.Count){return true;}
      if(hasBriefcase == true)
      {
        if(inventorySize + 1 > collectibles.Count){return true;}
      }

      return false;
    }

    public bool SelectTrap(int whattrap)
    {

      if(traps.Count > whattrap && traps[whattrap] != 0)
      {return true;}

      return false;
    }
    public void UseTrap(int whattrap)
    {
      if(traps.Count > whattrap && traps[whattrap] != 0)
      {traps[whattrap] = 0;}
    }

    public void RemoveCollectible(int whatcollectible)
    {
      if(collectibles.Count > whatcollectible && collectibles[whatcollectible] != 0)
      {collectibles[whatcollectible] = 0;}
    }

    //0 is none: the number of that trap in inventory
    public void AddTrap(int whattrap,int setto)
    {
      while(traps.Count <= whattrap )
      {    traps.Add(0);}

      {traps[whattrap] = setto;}
    }

    public void AddCollectible(int whatCollectible,int setto)
    {
      while(collectibles.Count <= whatCollectible )
      {    collectibles.Add(0);}

      {collectibles[whatCollectible] = setto;}
    }

    public string GetTrapsString()
    {
      string trapstring = "";
      foreach(int el in traps)
      {
        trapstring += el.ToString();
      }
      return trapstring;
    }

    public void UpdateInventorySprites(GameConstants gameConstants, Transform inventoryImages,bool changeTraps)
    {
      //if change traps is true set the list to the traps inventory list
      List<int> tempInventory = changeTraps ? traps : collectibles;
      List<Sprite> tempSpriteList = changeTraps ? gameConstants.trapSprites : gameConstants.collectibleSprites;
      int count = 1;
      int slotsUsed = 0;
      while(slotsUsed < inventoryImages.childCount && count < tempInventory.Count)
      {
        if(tempInventory[count] > 0)
        {
          inventoryImages.GetChild(slotsUsed).GetComponent<RawImage>().texture = tempSpriteList[count].texture;
          slotsUsed ++;
        }
        count++;
      }
      //set the rest of the sprites to blank
      while(slotsUsed <  inventoryImages.childCount) {inventoryImages.GetChild(slotsUsed).GetComponent<RawImage>().texture = tempSpriteList[0].texture;   slotsUsed ++;}
    }

}
