using System.Collections.ObjectModel;

namespace CS341GroupProject;
public partial class CommunityFeedPage : ContentPage
{
	//Property to hold ObservableCollection of Post objects
	public ObservableCollection<Post> Posts { get; set; }
	public CommunityFeedPage()
	{
		InitializeComponent();

        Posts = new ObservableCollection<Post>();

        LoadPosts();

        this.BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoadPosts();
    }

    private void LoadPosts()
    {
        Posts.Clear();

        var posts = MauiProgram.BusinessLogic.Posts;
        // load posts in reverse order (newest first)
        /*
        foreach (var post in posts.Reverse())
        {
            Posts.Add(post);
        }*/
        //FIXME: placeholder for now to only load 10 posts
        foreach(var post in posts.Reverse().Take(10))
        {
            Posts.Add(post);
        }
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