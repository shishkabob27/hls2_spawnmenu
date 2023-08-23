using Sandbox.UI.Tests;
namespace SpawnMenuAddon;

[Library]
public partial class WeaponList : Panel
{
	VirtualScrollPanel Canvas;
	public WeaponList()
	{
		AddClass("spawnpage");
		AddChild(out Canvas, "canvas");

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemWidth = 92;
		Canvas.Layout.ItemHeight = 92;
		Canvas.OnCreateCell = (cell, data) =>
		{
			if (data is TypeDescription type)
			{
				var btn = cell.Add.Button(type.Title);
				btn.AddClass("icon");
				btn.AddEventListener("onclick", () => ConsoleSystem.Run("ent_create", type.ClassName));
				var t = Texture.Load(FileSystem.Mounted, $"/ui/spawnmenu/{type.ClassName}.png", false);
				if (t == null || t.IsPromise || SpawnMenu.AlwaysUseRenderedIcons)
				{
					btn.Style.BackgroundImage = SpawnMenu.MakeIcon(type);
				}
				else
				{
					btn.Style.BackgroundImage = t;
					btn.Style.BackgroundPositionX = Length.Auto;
					btn.Style.BackgroundPositionY = Length.Auto;
				}
				var x = type.GetType();


			}
		};



	}
	string[] HL1Weapons = new string[] { "weapon_crowbar", "weapon_9mmhandgun", "weapon_357", "weapon_mp5", "weapon_shotgun", "weapon_crossbow", "weapon_rpg", "weapon_gauss", "weapon_egon", "weapon_hornet", "weapon_handgrenade", "weapon_tripmine", "weapon_satchel", "weapon_snark" };
	string prevtab;
	[GameEvent.Tick]
	void update()
	{
		if (prevtab == SpawnMenu.Current.SelectedTab) return;
		prevtab = SpawnMenu.Current.SelectedTab;
		if (SpawnMenu.Current.MainSelector.ActiveTab != "weapons") return;

		Canvas.Clear();
		//GARBAGE



		var ents = TypeLibrary.GetTypes<Weapon>()
									.OrderBy(x => x.Title)
									.ToArray();

		foreach (var entry in ents)
		{
			string a = "Other";
			try
			{
				a = (string)entry.GetAttribute<MenuCategory>().Value;
			}
			catch { }
			SpawnMenu.CheckCategory(a);
			if (a == SpawnMenu.Current.SelectedTab)
			{
				Canvas.AddItem(entry);
			}

		}
	}
}
