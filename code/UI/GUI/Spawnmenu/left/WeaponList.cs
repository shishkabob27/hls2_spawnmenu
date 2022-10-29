namespace SpawnMenu;

[Library]
public partial class WeaponList : Panel
{
	VirtualScrollPanel Canvas;

	public WeaponList()
	{
		AddClass( "spawnpage" );
		AddChild( out Canvas, "canvas" );

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemWidth = 96;
		Canvas.Layout.ItemHeight = 96;
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

		var ents = TypeLibrary.GetDescriptions<Weapon>()
									//.Where( x => x.HasTag( "spawnable" ) )
									.OrderBy( x => x.Title )
									.ToArray();
        foreach (var entry in ents)
        {
            Canvas.AddItem(entry);
        }

        ents = TypeLibrary.GetDescriptions<BaseAmmo>()
									//.Where( x => x.HasTag( "spawnable" ) )
									.OrderBy( x => x.Title )
									.ToArray();
		foreach ( var entry in ents )
		{
			Canvas.AddItem( entry );
		}
	}

}
