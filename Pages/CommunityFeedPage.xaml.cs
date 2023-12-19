using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CS341GroupProject;
public partial class CommunityFeedPage : ContentPage
{
    private int numPosts;
    private ObservableCollection<Post> _savePage;
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
    public ObservableCollection<Post> SavePage
    {
        get { return _savePage; }
        set { _savePage = value; OnPropertyChanged(nameof(SavePage)); }
    }
    
    

    //used for thread synchronization. (true indicates that we are signaling the event on creation)
    public static ManualResetEvent PageLoadingEvent = new ManualResetEvent(true);

    public CommunityFeedPage()
	{
		InitializeComponent();
        PreviousPage = new ObservableCollection<Post>();
        CurrentPage = new ObservableCollection<Post>();
        NextPage = new ObservableCollection<Post>();
        SavePage = new ObservableCollection<Post>();
        PageNumber = 1;
        PrevButtonEnabled = false;
        NextButtonEnabled = true;
        

        //Wait for signal from the background thread that loads the first page of posts
        AppShell.PostsLoaded.WaitOne();
        
        //Load first page of posts into the CurrentPage collection
        var firstPage = MauiProgram.BusinessLogic.DynamicPosts;
        foreach (var post in firstPage.Reverse())
        {
            CurrentPage.Add(post);
        }

        //Start loading the next page 
        Boolean savePrevButton = PrevButtonEnabled;
        Boolean saveNextButton = NextButtonEnabled;
        PrevButtonEnabled = false;
        NextButtonEnabled = false;
        PageLoadingEvent.WaitOne();
        PageLoadingEvent.Reset();
        LoadPage(PageNumber, 1, savePrevButton, saveNextButton);

        this.BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Calculate the total number of pages
        int currentNumPosts = MauiProgram.BusinessLogic.NumPosts;
        TotalPages = (int)Math.Ceiling((double)currentNumPosts / Constants.POSTS_PER_PAGE);
        
        if (MauiProgram.BusinessLogic.JustAddedPost)
        {
            MauiProgram.BusinessLogic.JustAddedPost = false;
            //wait for signal from the background thread that loads the first page of posts
            AddPlantPage.FirstPageLoaded.WaitOne();

            //Load first page of posts into the CurrentPage collection
            var firstPage = MauiProgram.BusinessLogic.DynamicPosts;
            foreach (var post in firstPage.Reverse())
            {
                CurrentPage.Add(post);
            }
            PrevButtonEnabled = false;
            NextButtonEnabled = true;
            numPosts = currentNumPosts;
            PageNumber = 1;
            PreviousPage.Clear();

            //Start loading the next page
            Boolean savePrevButton = PrevButtonEnabled;
            Boolean saveNextButton = NextButtonEnabled;
            PrevButtonEnabled = false;
            NextButtonEnabled = false;
            PageLoadingEvent.WaitOne();
            PageLoadingEvent.Reset();
            LoadPage(PageNumber, 1, savePrevButton, saveNextButton);

        }
        //load saved page if it exists
        else if (SavePage != null && SavePage.Count > 0)
        {
            //CurrentPage.Clear();
            foreach (var post in SavePage)
            {
                CurrentPage.Add(post);
            }
            SavePage.Clear();

        }


    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        foreach (var post in CurrentPage)
        {
            SavePage.Add(post);
        }
        CurrentPage.Clear();
        
    }

    public void OnNextClicked(object sender, EventArgs e)
    {
        // Base case: if at last page, do nothing
        if (PageNumber == TotalPages) { return; }

        //set button states
        ++PageNumber;
        if (PageNumber > 1) { PrevButtonEnabled = true; }
        if (PageNumber == TotalPages) { NextButtonEnabled = false; }

        // Save button states
        Boolean savePrevButton = PrevButtonEnabled;
        Boolean saveNextButton = NextButtonEnabled;
        PrevButtonEnabled = false;
        NextButtonEnabled = false;

        //await loading of other pages
        PageLoadingEvent.WaitOne();
        PageLoadingEvent.Reset();

        // Swap pages (PreviousPage <- CurrentPage <- NextPage)
        PreviousPage.Clear();
        foreach (var post in CurrentPage)
        {
            PreviousPage.Add(post);
        }
        CurrentPage.Clear();
        foreach (var post in NextPage)
        {
            CurrentPage.Add(post);
        }

        // Load the new next page
        LoadPage(PageNumber, 1, savePrevButton, saveNextButton);
    }

    public void OnPreviousClicked(object sender, EventArgs e)
    {
        // Base case: if at first page, do nothing
        if (PageNumber == 1) { return; }

        //set button states
        --PageNumber;
        if (PageNumber < TotalPages) { NextButtonEnabled = true; }
        if (PageNumber == 1) { PrevButtonEnabled = false; }

        // save button states
        Boolean savePrevButton = PrevButtonEnabled;
        Boolean saveNextButton = NextButtonEnabled;
        PrevButtonEnabled = false;
        NextButtonEnabled = false;

        //await loading of other pages
        PageLoadingEvent.WaitOne();
        PageLoadingEvent.Reset();

        // Swap pages (PreviousPage -> CurrentPage -> NextPage)
        NextPage.Clear();
        foreach (var post in CurrentPage)
        {
            NextPage.Add(post);
        }
        CurrentPage.Clear();
        foreach (var post in PreviousPage)
        {
            CurrentPage.Add(post);
        }

        

        // Load the new next page
        LoadPage(PageNumber, -1, savePrevButton, saveNextButton);
        
    }


    private async void LoadPage(int basePage, int direction, bool savePrevButton, bool saveNextButton)
    {
        

        // Wait for any other page loading to finish, then reset the event and start loading the next page
        await Task.Run(() =>
        {

            //load next page
            if (direction == 1)
            {
                //base case: if not at last page, load the next page
                if (PageNumber != TotalPages)
                {

                    var nextPagePosts = MauiProgram.BusinessLogic.DynamicSelectPosts(basePage + 1);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        NextPage.Clear();
                        foreach (var post in nextPagePosts.Reverse())
                        {
                            NextPage.Add(post);
                        }
                    });
                }
                //base case continued: if at last page, clear the next page and restore buttons
                else { NextPage.Clear(); }
            }
            //load previous page
            else if (direction == -1)
            {
                //base case: if not at first page, load the previous page
                if (PageNumber != 1)
                {

                    var previousPagePosts = MauiProgram.BusinessLogic.DynamicSelectPosts(basePage - 1);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        PreviousPage.Clear();
                        foreach (var post in previousPagePosts.Reverse())
                        {
                            PreviousPage.Add(post);
                        }
                    });
                }
                //base case continued: if at first page, clear the previous page and restore buttons
                else { PreviousPage.Clear(); }
            }


            // Signal that the posts have been loaded
            PageLoadingEvent.Set();

            //load previous button states
            PrevButtonEnabled = savePrevButton;
            NextButtonEnabled = saveNextButton;
            
        });

    }
    

}