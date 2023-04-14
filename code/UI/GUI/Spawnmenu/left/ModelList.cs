using Sandbox.UI.Tests;

namespace SpawnMenuAddon;

[Library]
public partial class ModelList : Panel
{
	VirtualScrollPanel Canvas;

	public ModelList()
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
			}
		};


		//GARBAGE

	}

	string prevtab;
	[Event.Tick.Client]
	void update()
	{
		if (prevtab == SpawnMenu.Current.SelectedTab) return;
		prevtab = SpawnMenu.Current.SelectedTab;
		if (SpawnMenu.Current.MainSelector.ActiveTab != "models") return;

		Canvas.Clear();

	}

}
