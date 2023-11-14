using System.Collections.ObjectModel;

namespace CS341GroupProject;
/*
 * Alexsa Walczak
 */
public partial class CommunityFeedPage : ContentPage
{
	//Property to hold ObservableCollection of Post objects
	public ObservableCollection<Post> Posts { get; set; }
	public CommunityFeedPage()
	{
		InitializeComponent();

		Posts = new ObservableCollection<Post>();

		//example adding items to the collection
		Posts.Add(new Post { Username = "User1", Photo = "plant.jpg", Plant = "plant1", Notes = "This is a note about plant1" });
		Posts.Add(new Post { Username = "User2", Photo = "flower.png", Plant = "plant2", Notes = "This is a note about plant2" });
		Posts.Add(new Post { Username = "User3", Photo = "plant.jpg", Plant = "plant3", Notes = "This is a note about plant3" });
		Posts.Add(new Post { Username = "User4", Photo = "flower.jpg", Plant = "plant4", Notes = "This is a note about plant4" });
		Posts.Add(new Post { Username = "User5", Photo = "plant.png", Plant = "plant5", Notes = "This is a note about plant5" });

		this.BindingContext = this;
	}

	/*
	 * TODO: So far, my changes (alex) appear to have added the proper binding to the feed page.
	 * We need to have a table for posts in the database, and then implement either the constructor
	 * to populate the ObservableCollection with the posts, or have it pull a pre-loaded observable
	 * collection. It would depend on how things are loaded.
	 *		If loading it here, means that it loads when switching to this tab - it may be useful to
	 *		test performance and see if it's better to load it when we click onto the feed page or
	 *		have it loaded at the start of the app and pull it from memory.
	 *	
	 *	TODO: If the posts get too long, it won't be a good idea to load all of the posts, so we may
	 *	need to implement a way to load a certain number of posts at a time, and then load more as
	 *	the user wishes.
	 *		Could be done with pages, or somehow code the program to load more posts as the user 
	 *		scrolls down. (but then if they scroll too far, would that affect performance?)
	 */
}