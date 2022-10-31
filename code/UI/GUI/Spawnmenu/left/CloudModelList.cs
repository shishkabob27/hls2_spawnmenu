using Sandbox;

namespace SpawnMenuAddon;

[Library]
public partial class CloudModelList : Panel
{
	VirtualScrollPanel Canvas;

	public CloudModelList()
	{
        AddClass("spawnpage");
        AddChild(out Canvas, "canvas");

        Canvas.Layout.AutoColumns = true;
        Canvas.Layout.ItemWidth = 91;
        Canvas.Layout.ItemHeight = 91;

        Canvas.OnCreateCell = (cell, data) =>
        {
            var file = (Package)data;
            var btn = cell.Add.Button(file.Title);
            btn.AddClass("icon");
            btn.AddEventListener("onclick", () => ConsoleSystem.Run( "spawnmenu_spawn", file.FullIdent, SpawnMenu.Current.TypeSelector.ActiveTab) );
            btn.Style.BackgroundImage = Texture.Load(file.Thumb);
        };

        _ = UpdateItems();
    }
	string prevSearch = "";
    string prevtab;
    bool prevshowincomp;
    public override void Tick()
    {
        if ( SpawnMenu.Current.MainSelector.ActiveTab != "assetparty" ) return;
        if ( prevSearch == SpawnMenu.Current.SearchQuery && prevtab == SpawnMenu.Current.SelectedTab && prevshowincomp == SpawnMenu.Current.ShowIncompatible ) return;
		prevSearch = SpawnMenu.Current.SearchQuery;
		prevtab = SpawnMenu.Current.SelectedTab;
		prevshowincomp = SpawnMenu.Current.ShowIncompatible;
        RefreshItems();
	} 
	public async Task UpdateItems( int offset = 0 )
	{
		var q = new Package.Query();
		q.Type = Package.Type.Model;
		 
        switch ( SpawnMenu.Current.TypeSelector.ActiveTab )
        {
            case "model":
                q.Type = Package.Type.Model;
                break;
            case "addon":
                q.Type = Package.Type.Addon;
                break;
            case "material":
                q.Type = Package.Type.Material;
                break;
            case "map":
                q.Type = Package.Type.Map;
                break;
            case "sound":
                q.Type = Package.Type.Sound;
                break;
        }

        q.Order = Package.Order.Newest;
		switch(prevtab)
		{
			case "Most Recent":
                q.Order = Package.Order.Newest;
				break;
			case "Most Popular":
                q.Order = Package.Order.Popular;
				break;
			case "Most Downloads":
                q.Order = Package.Order.Live;
				break;
			case "Trending":
                q.Order = Package.Order.Trending;
				break;
        }
		q.Take = 200;
		q.Skip = offset;
		q.SearchText = SpawnMenu.Current.SearchQuery;

		var found = await q.RunAsync( default );
		var foundNew = found.ToList();
		if ( SpawnMenu.Current.TypeSelector.ActiveTab == "addon" && !SpawnMenu.Current.ShowIncompatible)
        {
			foundNew.RemoveAll( x => !( x.GetMeta<string>( "ParentPackage" ) == "shishkabob.hls2" || x.Tags.Contains("game:any") ) ) ; 
        }
		Canvas.SetItems( foundNew );

		for (var i = 1; i < 5; i++ )
        {
            q.Take = 200;
            q.Skip = 200 * i;
            q.SearchText = SpawnMenu.Current.SearchQuery;

            var found2 = await q.RunAsync( default );
            var foundNew2 = found2.ToList();
            if ( SpawnMenu.Current.TypeSelector.ActiveTab == "addon" && !SpawnMenu.Current.ShowIncompatible )
            {
                foundNew2.RemoveAll( x => !( x.GetMeta<string>( "ParentPackage" ) == "shishkabob.hls2" || x.Tags.Contains( "game:any" ) ) );
            }
            Canvas.AddItems( foundNew2.ToArray() );
        }

        // TODO - auto add more items here
    }

	public void RefreshItems()
	{
		Canvas.Clear();
		_ = UpdateItems();
	}

	static async Task<string> SpawnPackageModel( string packageName, Vector3 pos, Rotation rotation, Entity source )
	{
		var package = await Package.Fetch( packageName, false );
		if ( package == null || package.PackageType != Package.Type.Model || package.Revision == null )
		{
			// spawn error particles
			return null;
		}

		if ( !source.IsValid ) return null; // source entity died or disconnected or something

		var model = package.GetMeta( "PrimaryAsset", "models/dev/error.vmdl" );
		var mins = package.GetMeta( "RenderMins", Vector3.Zero );
		var maxs = package.GetMeta( "RenderMaxs", Vector3.Zero );

		// downloads if not downloads, mounts if not mounted
		await package.MountAsync();

		return model;
	} 
	static async Task<string> SpawnPackageSound( string packageName, Vector3 pos, Rotation rotation, Entity source )
	{
		var package = await Package.Fetch( packageName, false );
		if ( package == null || package.PackageType != Package.Type.Sound || package.Revision == null )
		{
			// spawn error particles
			return null;
		}

		if ( !source.IsValid ) return null; // source entity died or disconnected or something

		var asset = package.GetMeta( "PrimaryAsset", "sounds/dev/error.sound" );

		// downloads if not downloads, mounts if not mounted
		await package.MountAsync();   
        Log.Info( $"Playing sound {asset}" ); 
        Sound.FromEntity( asset, source );
        return asset;

    }	

	static async Task<string> SpawnPackageAddon( string packageName, Entity source )
	{
		var package = await Package.Fetch( packageName, false );
		if ( package == null || package.PackageType != Package.Type.Addon || package.Revision == null )
		{
			// spawn error particles
			return null;
		}

		if ( !source.IsValid ) return null; // source entity died or disconnected or something

		var asset = package.GetMeta( "PrimaryAsset", "sounds/dev/error.sound" );

		// downloads if not downloads, mounts if not mounted
		await package.MountAsync();   
		
        Log.Info( $"Loading addon {asset}" );  
        return asset;

    }	
	static async Task<string> SpawnPackageGeneric( string packageName, Entity source )
	{
		var package = await Package.Fetch( packageName, false );
		if ( package == null || package.Revision == null )
		{
			// spawn error particles
			return null;
		}

		if ( !source.IsValid ) return null; // source entity died or disconnected or something

		var asset = package.GetMeta( "PrimaryAsset", "sounds/dev/error" );

		// downloads if not downloads, mounts if not mounted
		await package.MountAsync();   
		
        Log.Info( $"Loading package {asset}" );  
        return asset;

    }

	[ConCmd.Server( "spawnmenu_spawn" )]
	public static void Spawn( string ident , string activetab )
	{
		var owner = ConsoleSystem.Caller?.Pawn;
		Log.Info( ident ); 
		if ( ConsoleSystem.Caller == null )
			return;
        switch ( activetab )
        {
            case "model":
				SpawnModel( ident, owner );
                break;
            case "addon":
                SpawnPackageAddon( ident, owner );
                break;
            case "material":
                SpawnPackageGeneric( ident, owner );
                break;
            case "map":
				Global.ChangeLevel( ident );
                break;
			case "sound":
                SpawnPackageSound( ident, Vector3.Zero, Rotation.From( Angles.Zero ), owner );
                break;
			default:
                SpawnModel( ident, owner );
                break;
        }
    }
	static async void SpawnModel( string modelname, Entity owner ) {

        var tr = Trace.Ray( owner.EyePosition, owner.EyePosition + owner.EyeRotation.Forward * 500 )
            .UseHitboxes()
            .Ignore( owner )
            .Run();

        var modelRotation = Rotation.From( new Angles( 0, owner.EyeRotation.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );

		//
		// Does this look like a package?
		//
		if ( modelname.Count( x => x == '.' ) == 1 && !modelname.EndsWith( ".vmdl", System.StringComparison.OrdinalIgnoreCase ) && !modelname.EndsWith( ".vmdl_c", System.StringComparison.OrdinalIgnoreCase ) )
		{
			modelname = await SpawnPackageModel( modelname, tr.EndPosition, modelRotation, owner );
			if ( modelname == null )
				return;
		}

		var model = Model.Load( modelname );
		if ( model == null || model.IsError )
			return;

		var ent = new Prop
		{
			Position = tr.EndPosition + Vector3.Down * model.PhysicsBounds.Mins.z,
			Rotation = modelRotation,
			Model = model
		};

		// Let's make sure physics are ready to go instead of waiting
		ent.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		// If there's no physics model, create a simple OBB
		if ( !ent.PhysicsBody.IsValid() )
		{
			ent.SetupPhysicsFromOBB( PhysicsMotionType.Dynamic, ent.CollisionBounds.Mins, ent.CollisionBounds.Maxs );
		}
	}
}
