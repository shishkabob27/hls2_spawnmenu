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
        Canvas.Layout.ItemWidth = 92;
        Canvas.Layout.ItemHeight = 92;
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
        if ( SpawnMenu.Current.MainSelector.ActiveTab != "entities" ) return;

        Canvas.Clear();
        //GARBAGE



        var ents = TypeLibrary.GetDescriptions<Entity>()
                                    .OrderBy( x => x.Title )
                                    .ToList();
        System.Type[] b = {
                typeof( BrushEntity ),
                typeof( BaseGamemodeStub ),
                typeof( BaseTrigger ),
                typeof( Water ),
                typeof( WorldEntity ),
                typeof( DoorEntity ),
                typeof( DoorRotatingEntity ),
                typeof( PhysicsBrushEntity ),
                typeof( ButtonEntity ),
                typeof( ButtonEntityRot ),
                typeof( Weapon ),
                typeof( NPC )
            };

        foreach ( System.Type type in b )
        {

            ents.RemoveAll( x => x.TargetType.IsAssignableTo( type ) );
        }

        foreach ( var entry in ents )
        {
            string a = "Other";
            try
            {
                a = (string)entry.GetAttribute<MenuCategory>().Value;
            }
            catch { }
            SpawnMenu.CheckCategory( a );
            if ( a == SpawnMenu.Current.SelectedTab )
            {
                Canvas.AddItem( entry );
            }

        }
    }
    void update2()
    {
        if ( prevtab == SpawnMenu.Current.SelectedTab ) return;
        prevtab = SpawnMenu.Current.SelectedTab;
        if ( SpawnMenu.Current.MainSelector.ActiveTab != "entities" ) return;
        var tab = SpawnMenu.Current.SelectedTab;

        Canvas.Clear();
        //GARBAGE
        if ( tab == "Ammo" )
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
        else if ( tab == "Items" )
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
        else if ( tab == "All" )
        {
            var ents = TypeLibrary.GetDescriptions<Entity>()
                                        //.Where( x => x.HasTag( "spawnable" ) )
                                        .OrderBy( x => x.Title )
                                        .ToList();

            System.Type[] a = { 
                typeof( BrushEntity ), 
                typeof( BaseGamemodeStub ), 
                typeof( BaseTrigger ), 
                typeof( Water ), 
                typeof( WorldEntity ), 
                typeof( DoorEntity ), 
                typeof( DoorRotatingEntity ), 
                typeof( PhysicsBrushEntity ), 
                typeof( ButtonEntity ), 
                typeof( ButtonEntityRot ) 
            };

            foreach (System.Type type in a) {

                ents.RemoveAll( x => x.TargetType.IsAssignableTo( type ) );
            }
            ents.RemoveAll( x => x.Name.Contains("stub") );
            ents.RemoveAll( x => x.Name.Contains("stub") );
            foreach ( var entry in ents )
            {
                Canvas.AddItem( entry );
            }
        }
    }

}
