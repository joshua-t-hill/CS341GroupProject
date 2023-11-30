using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CS341GroupProject;
public partial class CommunityFeedPage : ContentPage
{
    private ObservableCollection<Post> savePage;
    private ObservableCollection<Post> _currentPage;
    private ObservableCollection<Post> _previousPage;
    private ObservableCollection<Post> _nextPage;
    private int _pageNumber;
    private int _totalPages; 
    private Boolean _nextButtonEnabled;
    private Boolean _prevButtonEnabled;

    public string PageDisplay => $"{PageNumber}/{TotalPages}";

    public ObservableCollection<Post> CurrentPage
    {
        get { return _currentPage; }
        set { _currentPage = value; OnPropertyChanged(nameof(CurrentPage)); }
    }
    public ObservableCollection<Post> PreviousPage
    {
        get { return _previousPage; }
        set { _previousPage = value; OnPropertyChanged(nameof(PreviousPage)); }
    }
    public ObservableCollection<Post> NextPage
    {
        get { return _nextPage; }
        set { _nextPage = value; OnPropertyChanged(nameof(NextPage)); }
    }
    public int PageNumber
    {
        get { return _pageNumber; }
        set { _pageNumber = value; OnPropertyChanged(nameof(PageNumber)); OnPropertyChanged(nameof(PageDisplay)); }
    }
    public int TotalPages
    {
        get { return _totalPages; }
        set { _totalPages = value; OnPropertyChanged(nameof(TotalPages)); OnPropertyChanged(nameof(PageDisplay)); }
    }
    public Boolean NextButtonEnabled
    {
        get { return _nextButtonEnabled; }
        set { _nextButtonEnabled = value; OnPropertyChanged(nameof(NextButtonEnabled)); }
    }
    public Boolean PrevButtonEnabled
    {
        get { return _prevButtonEnabled; }
        set { _prevButtonEnabled = value; OnPropertyChanged(nameof(PrevButtonEnabled)); }
    }
    
    

    //used for thread synchronization. (true indicates that we are signaling the event on creation)
    public static ManualResetEvent PageLoadingEvent = new ManualResetEvent(true);

    public CommunityFeedPage()
	{
		InitializeComponent();
        PreviousPage = new ObservableCollection<Post>();
        CurrentPage = new ObservableCollection<Post>();
        NextPage = new ObservableCollection<Post>();
        savePage = new ObservableCollection<Post>();
        PageNumber = 1;
        PrevButtonEnabled = false;
        NextButtonEnabled = true;
        this.BindingContext = this;

        //Wait for signal from the background thread that loads the first page of posts
        AppShell.PostsLoaded.WaitOne();
        
        //Load first page of posts into the CurrentPage collection
        var firstPage = MauiProgram.BusinessLogic.DynamicPosts;
        foreach (var post in firstPage.Reverse())
        {
            CurrentPage.Add(post);
        }

        //Start loading the next page 
        LoadNextPage(PageNumber);

        
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        //load saved page if it exists
        if (savePage != null && savePage.Count > 0)
        {
            //CurrentPage.Clear();
            foreach (var post in savePage)
            {
                CurrentPage.Add(post);
            }
            savePage.Clear();
            
        }

        //update total number of pages
        Task.Run(() =>
        {
            // Calculate the total number of pages
            int totalPostsCount = MauiProgram.BusinessLogic.NumPosts;
            totalPostsCount = (int)Math.Ceiling((double)totalPostsCount / Constants.POSTS_PER_PAGE);

            // Update the UI on the main thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TotalPages = totalPostsCount;
                OnPropertyChanged(nameof(PageDisplay));
            });
        });

    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        foreach (var post in CurrentPage)
        {
            savePage.Add(post);
        }
        CurrentPage.Clear();
        
    }

    public void OnNextClicked(object sender, EventArgs e)
    {
        // Base case: if at last page, do nothing
        if (PageNumber == TotalPages) { return; }
        PageNumber++;
        

        // move CurrentPage to PreviousPage
        PreviousPage.Clear();
        foreach (var post in CurrentPage)
        {
            PreviousPage.Add(post);
        }

        // move NextPage to CurrentPage
        CurrentPage.Clear();
        foreach (var post in NextPage)
        {
            CurrentPage.Add(post);
        }

        //set button states
        if (PageNumber - 1 >= 1) { PrevButtonEnabled = true; }
        if (PageNumber == TotalPages) { NextButtonEnabled = false; }

        // Load the new next page
        LoadNextPage(PageNumber);
    }

    public void OnPreviousClicked(object sender, EventArgs e)
    {
        // Base case: if at first page, do nothing
        if (PageNumber == 1) { return; }
        PageNumber--;
        

        // move CurrentPage to NextPage
        NextPage.Clear();
        foreach (var post in CurrentPage)
        {
            NextPage.Add(post);
        }

        // move PreviousPage to CurrentPage
        CurrentPage.Clear();
        foreach (var post in PreviousPage)
        {
            CurrentPage.Add(post);
        }

        //set button states
        if (PageNumber + 1 == TotalPages) { NextButtonEnabled = true; }
        if (PageNumber == 1) { PrevButtonEnabled = false; }

        // Load the new next page
        LoadPreviousPage(PageNumber);
        
    }


    private void LoadNextPage(int basePage)
    {
        if (PageNumber == TotalPages) { NextPage.Clear();  return; }
        // User must wait for pages to load when navigating to a new page
        Boolean savePrevButton = PrevButtonEnabled;
        Boolean saveNextButton = NextButtonEnabled;
        PrevButtonEnabled = false;
        NextButtonEnabled = false;

        // Wait for any other page loading to finish, then reset the event and start loading the next page
        Task.Run(() =>
        {
            PageLoadingEvent.WaitOne();
            PageLoadingEvent.Reset();
            var nextPagePosts = MauiProgram.BusinessLogic.DynamicSelectPosts(basePage + 1);
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                NextPage.Clear();
                foreach (var post in nextPagePosts.Reverse())
                {
                    NextPage.Add(post);
                }

                // Signal that the posts have been loaded
                PageLoadingEvent.Set();

                //load previous button states
                PrevButtonEnabled = savePrevButton;
                NextButtonEnabled = saveNextButton;
            });
        });

    }

    
    private void LoadPreviousPage(int basePage)
    {
        if (PageNumber == 1) { PreviousPage.Clear();  return; }
        // User must wait for pages to load when navigating to a new page
        Boolean savePrevButton = PrevButtonEnabled;
        Boolean saveNextButton = NextButtonEnabled;
        PrevButtonEnabled = false;
        NextButtonEnabled = false;

        // Wait for any other page loading to finish, then reset the event and start loading the previous page
        Task.Run(() =>
        {
            PageLoadingEvent.WaitOne();
            PageLoadingEvent.Reset();
            var previousPagePosts = MauiProgram.BusinessLogic.DynamicSelectPosts(basePage - 1);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                PreviousPage.Clear();
                foreach (var post in previousPagePosts.Reverse())
                {
                    PreviousPage.Add(post);
                }

                // Signal that the posts have been loaded
                PageLoadingEvent.Set();

                //load previous button states
                PrevButtonEnabled = savePrevButton;
                NextButtonEnabled = saveNextButton;
            });
        });
    }

}