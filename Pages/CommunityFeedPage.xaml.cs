using System.Collections.ObjectModel;

namespace CS341GroupProject;
public partial class CommunityFeedPage : ContentPage
{
    private ObservableCollection<Post> savePage;
    private ObservableCollection<Post> currentPage;
    private ObservableCollection<Post> previousPage;
    private ObservableCollection<Post> nextPage;
    private int pageNumber;
    public ObservableCollection<Post> CurrentPage
    {
        get { return currentPage; }
        set
        {
            currentPage = value;
            OnPropertyChanged(nameof(CurrentPage));
        }
    }
    public ObservableCollection<Post> PreviousPage
    {
        get { return previousPage; }
        set
        {
            previousPage = value;
            OnPropertyChanged(nameof(PreviousPage));
        }
    }
    public ObservableCollection<Post> NextPage
    {
        get { return nextPage; }
        set
        {
            nextPage = value;
            OnPropertyChanged(nameof(NextPage));
        }
    }
    
    

    //used for thread synchronization. (False indicates that we aren't signaling the event on creation)
    public static ManualResetEvent PageLoadingEvent = new ManualResetEvent(false);

    public CommunityFeedPage()
	{
		InitializeComponent();
        PreviousPage = new ObservableCollection<Post>();
        CurrentPage = new ObservableCollection<Post>();
        NextPage = new ObservableCollection<Post>();
        savePage = new ObservableCollection<Post>();
        pageNumber = 1;

        //Wait for signal from the background thread that loads the first page of posts
        AppShell.PostsLoaded.WaitOne();
        
        
        //Load first page of posts into the CurrentPage collection
        var firstPage = MauiProgram.BusinessLogic.DynamicPosts;
        foreach (var post in firstPage.Reverse())
        {
            CurrentPage.Add(post);
        }

        //Start loading the next page 
        LoadNextPage();

        this.BindingContext = this;
    }

    public void setFirstPage(ObservableCollection<Post> firstPage)
    {
        foreach(var post in firstPage.Reverse())
        {
            CurrentPage.Add(post);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (savePage != null && savePage.Count > 0)
        {
            CurrentPage.Clear();
            foreach (var post in savePage)
            {
                CurrentPage.Add(post);
            }
            savePage.Clear();
        }
        
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        foreach(var post in CurrentPage)
        {
            savePage.Add(post);
        }
    }


    private void LoadNextPage()
    {
        Task.Run(() =>
        {
            var nextPagePosts = MauiProgram.BusinessLogic.DynamicSelectPosts(pageNumber + 1);
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                NextPage.Clear();
                foreach (var post in nextPagePosts.Reverse())
                {
                    NextPage.Add(post);
                }

                // Signal that the posts have been loaded
                PageLoadingEvent.Set();
            });
        });

    }

    /// <summary>
    /// Will mainly be used for backwards navigation or jumping to a specific page.
    /// </summary>
    private void LoadPreviousPage()
    {
        Task.Run(() =>
        {
            var previousPagePosts = MauiProgram.BusinessLogic.DynamicSelectPosts(pageNumber - 1);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                PreviousPage.Clear();
                foreach (var post in previousPagePosts.Reverse())
                {
                    PreviousPage.Add(post);
                }

                // Signal that the posts have been loaded
                PageLoadingEvent.Set();
            });
        });

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