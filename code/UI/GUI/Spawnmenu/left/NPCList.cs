namespace SpawnMenu;

[Library]
public partial class NPCList : Panel
{
	VirtualScrollPanel Canvas;

	public NPCList()
	{
		AddClass( "spawnpage" );
		AddChild( out Canvas, "canvas" );

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemWidth = 64;
		Canvas.Layout.ItemHeight = 64;
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


		var ents = TypeLibrary.GetDescriptions<NPC>()
									//.Where( x => x.HasTag( "spawnable" ) )
									.OrderBy( x => x.Title )
									.ToArray();

		foreach ( var entry in ents )
		{
			Canvas.AddItem( entry );
		}
	}

}
