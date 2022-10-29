using Sandbox.UI;
using Sandbox.UI.Construct;
using Sandbox.UI.Tests;

[Library]
public partial class EntityList : Panel
{
	VirtualScrollPanel Canvas;

	public EntityList()
	{
		AddClass( "spawnpage" );
		AddChild( out Canvas, "canvas" );

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemWidth = 128;
		Canvas.Layout.ItemHeight = 128;
		Canvas.OnCreateCell = ( cell, data ) =>
		{
			if ( data is TypeDescription type )
			{
				var btn = cell.Add.Button( type.Title );
				btn.AddClass( "icon" );
				btn.AddEventListener( "onclick", () => ConsoleSystem.Run( "ent_create", type.ClassName ) );
				btn.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, $"/ui/spawnmenu/{type.ClassName}.png", false );
			}
		};


		//GARBAGE
		var ents = TypeLibrary.GetDescriptions<Suit>()
									.ToArray();
		foreach ( var entry in ents )
		{
			Canvas.AddItem(  entry );
		}

		ents = TypeLibrary.GetDescriptions<Battery>()
									.ToArray();
		foreach ( var entry in ents )
		{
			Canvas.AddItem(  entry );
		}

		ents = TypeLibrary.GetDescriptions<HealthKit>()
									.ToArray();
		foreach ( var entry in ents )
		{
			Canvas.AddItem(  entry );
		}

		ents = TypeLibrary.GetDescriptions<item_sodacan>()
									.ToArray();
		foreach ( var entry in ents )
		{
			Canvas.AddItem(  entry );
		}

	}

}
