# iayos.extensions

Extension methods and Helper class functionality to extend common .NET functionality

E.g. 

##### ICollection<T>.InsertOrSet(this ICollection<T> collection, Func<T, bool> func) :

```csharp
public class Wolf
{
    public int WolfId { get; set; }
    public string Name { get; set; }
	public List<BattleScar> BattleScars { get;set; }
}

public class BattleScar
{
	public int BattleScarId { get; set; }
	public string ScarredOn { get; set; }	// yeah yeah, its not a DateTime
	public int DamageFactor { get; set; }
}

var wolf = new Wolf { WolfId = 1, Name = "Old One-eye" };

var firstScar = new BattleScar { BattleScarId = 1, ScarredOn = '20.08.1981', DamageFactor = 1 };
wolf.BattleScars.InsertOrSet(firstScar, bs => bs.BattleScarId == firstScar.BattleScarId);	// inserts first scar into collection

var secondScar = new BattleScar { BattleScarId = 2, ScarredOn = '18.02.2014', DamageFactor = 5 };
wolf.BattleScars.InsertOrSet(secondScar, bs => bs.BattleScarId == secondScar.BattleScarId);	// inserts second scar into collection

// whoops, we later loaded up some second collection of scars and the unbeknownst to us, the first scar has since been updated and is included in this new list, (note: its been updated and hurt more than we thought)
// --> newInstanceOfFirstScar.DamageFactor = 10;	// note that newly loaded record hurts a lot more
wolf.BattleScars.InsertOrSet(newInstanceOfFirstScar, bs => bs.BattleScarId = newInstanceOfFirstScar.BattleScarId);	// dont insert duplicate by id, instead remove old instance and reassign new, based on matching id

```
Results in array of only two scars in our collection
```
{ id = 1, ScarredOn = '20.08.1981',  DamageFactor = 10 } and { id = 2, ScarredOn = '18.02.2014',  DamageFactor = 5 } 
```





# Todos

- move all the rest of my functions over (work in progress)
-