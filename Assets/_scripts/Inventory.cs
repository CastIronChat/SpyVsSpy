using System.Collections;
using System.Collections.Generic;

/// All the stuff a player is holding.
public class Inventory  {
    public HashSet<Collectible> items = new HashSet<Collectible>();

    public int inventorySize,equippedTrap;
    public List<int> collectibles,traps;


    public bool CanHoldMoreCollectibles()
    {
      if(inventorySize > collectibles.Count){return true;}
      return false;
    }

    public bool SelectTrap(int whattrap)
    {
      if(traps.Count > whattrap && traps[whattrap] != 0)
      {return true;}

      return false;
    }


}
