using static HLCombat;

namespace SpawnMenuAddon;

[Library]
public partial class EntityList : Panel
{
	VirtualScrollPanel Canvas;

	public EntityList()
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
                var t = Texture.Load( FileSystem.Mounted, $"/ui/spawnmenu/{type.ClassName}.png", false );
                if ( t == null || t.IsPromise || SpawnMenu.AlwaysUseRenderedIcons )
                {
                    btn.Style.BackgroundImage = SpawnMenu.MakeIcon( type );
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
        if ( prevtab == SpawnMenu.Current.SelectedTab ) return;
        prevtab = SpawnMenu.Current.SelectedTab;
        var tab = SpawnMenu.Current.SelectedTab;

        Canvas.Clear();
        //GARBAGE
        if ( tab == "ammo" )
        {

            var ents = TypeLibrary.GetDescriptions<BaseAmmo>()
                                        //.Where( x => x.HasTag( "spawnable" ) )
                                        .OrderBy( x => x.Title )
                                        .ToArray();
            foreach ( var entry in ents )
            {
                Canvas.AddItem( entry );
            }
        }
        else if ( tab == "items" )
        {

            var ents = TypeLibrary.GetDescriptions<Suit>().ToArray();
            foreach ( var entry in ents )
            {
                Canvas.AddItem( entry );
            }

            ents = TypeLibrary.GetDescriptions<Battery>().ToArray();
            foreach ( var entry in ents )
            {
                Canvas.AddItem( entry );
            }

            ents = TypeLibrary.GetDescriptions<HealthKit>().ToArray();
            foreach ( var entry in ents )
            {
                Canvas.AddItem( entry );
            }

            ents = TypeLibrary.GetDescriptions<item_sodacan>().ToArray();
            foreach ( var entry in ents )
            {
                Canvas.AddItem( entry );
            }
        }
        else if ( tab == "all" )
        {
            var ents = TypeLibrary.GetDescriptions<Entity>()
                                        //.Where( x => x.HasTag( "spawnable" ) )
                                        .OrderBy( x => x.Title )
                                        .ToList();
            ents.RemoveAll( x => x.TargetType.IsSubclassOf( typeof( BrushEntity ) ));
            ents.RemoveAll( x => x.TargetType.IsSubclassOf( typeof( BaseGamemodeStub ) ));
            ents.RemoveAll( x => x.TargetType.IsSubclassOf( typeof( BaseTrigger ) ));
            ents.RemoveAll( x => x.TargetType.IsSubclassOf( typeof( Water ) ));
            ents.RemoveAll( x => x.TargetType.IsSubclassOf( typeof( WorldEntity ) ));
            ents.RemoveAll( x => x.TargetType.IsSubclassOf( typeof( DoorEntity ) ));
            ents.RemoveAll( x => x.TargetType.IsSubclassOf( typeof( DoorRotatingEntity ) ));
            ents.RemoveAll( x => x.TargetType.IsSubclassOf( typeof( PhysicsBrushEntity ) ));
            ents.RemoveAll( x => x.TargetType.IsSubclassOf( typeof( ButtonEntity ) ));
            ents.RemoveAll( x => x.TargetType.IsSubclassOf( typeof( ButtonEntityRot ) ));


            ents.RemoveAll( x => x.TargetType == ( typeof( BrushEntity ) ) );
            ents.RemoveAll( x => x.TargetType == ( typeof( BaseGamemodeStub ) ) );
            ents.RemoveAll( x => x.TargetType == ( typeof( BaseTrigger ) ) );
            ents.RemoveAll( x => x.TargetType == ( typeof( Water ) ) );
            ents.RemoveAll( x => x.TargetType == ( typeof( WorldEntity ) ) );
            ents.RemoveAll( x => x.TargetType == ( typeof( DoorEntity ) ) );
            ents.RemoveAll( x => x.TargetType == ( typeof( DoorRotatingEntity ) ) );
            ents.RemoveAll( x => x.TargetType == ( typeof( PhysicsBrushEntity ) ) );
            ents.RemoveAll( x => x.TargetType == ( typeof( ButtonEntity ) ) );
            ents.RemoveAll( x => x.TargetType == ( typeof( ButtonEntityRot ) ) );

            ents.RemoveAll( x => x.Name.Contains("stub") );
            ents.RemoveAll( x => x.Name.Contains("stub") );
            foreach ( var entry in ents )
            {
                Canvas.AddItem( entry );
            }
        }
    }

}
