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

		this.BindingContext = this;
	}
}