Project Group 3 / Authors:
	Alex Ceithamer [ARC]
	Alexsa Walczak [AMW]
	Joshua T. Hill [JTH]
	Samuel Ayoade [SA]

Resource Attribution:
	maptabicon.png		--> <a href="https://www.flaticon.com/free-icons/pin" title="pin icons">Pin icons created by Freepik - Flaticon</a>
	camertabicon.png	--> <a href="https://www.flaticon.com/free-icons/camera" title="camera icons">Camera icons created by Good Ware - Flaticon</a>
	feedtabicon.png		--> <a href="https://www.flaticon.com/free-icons/list" title="list icons">List icons created by Smartline - Flaticon</a>
	admintabicon.png	--> <a href="https://www.flaticon.com/free-icons/gear-cog" title="gear cog icons">Gear cog icons created by tuktukdesign - Flaticon</a>
	app_icon.png		--> <a href="https://www.flaticon.com/free-icons/hobbies-and-free-time" title="hobbies and free time icons">Hobbies and free time icons created by Vichanon Chaimsuk - Flaticon</a>

Sprint4 Changes:
	~ Fixed Navigation bugs from Sprint3 :) :)
	~ Feed page now displays data from the database (user photos!) :) :)
	~ Refactored UserSearchPage for better performance
	+ Added searchbar, live filtering, and ban buttons for each object for the BannedUserPage
	~ Fixed another navigation "bug" where returning to the Camera tab after posting a plant would return the user to the same AddPlantPage as before. It now opens to the CameraPage.
	+ Added column 'has_temp_password' to table 'users'
	+ Added methods to allow users to reset their passwords with the help of an admin.
	+ Added multithreading to load posts for better performance in CommunityFeedPage and Post.cs(From 10+ seconds for 10 posts to almost instant)
	~ Refactored Post.cs for binding properties and to hold the actual image (major bottleneck FIXED)
	~ Refactored CommunityFeedPage to load page-by-page instead of all at once
	+ Added ability to use location services on MapPage startup to open map on user's current location. :) :)
	~ Made some layout changes to the PlantDetailPopup subpage to look closer to its final intended form. :)



Sprint3 Changes:
	~ Researched sending emails; found that we would need to give bank info to another company.. changing idea for now
	+ Added column 'salt' to table 'users'
	+ Encrypted passwords :) :)
	+ Created table 'photos' with columns 'photo_id' and 'image_data'
	+ Connected the camera output to the 'photos' table :)
	~ Researched storing photos on the cloud, will implement in a future sprint as we wanted to get the basic methods created first
	+ Created table 'posts' with columns 'username', 'plant_genus', 'plant_species', 'notes', and 'photo_id'
	+ Added functionality to AddPlantPage and connected it to the 'posts' table (not connected to Feed just yet) :)
	+ Added username on login to Secure Storage so we can access it on different pages throughout the app
	+ Created custom Pin objects that allow us to manipulate and track data accross all map interactions :) :) :)
	+ Created a Popup page that responds to the default info window on a Pin being tapped. Displays a picture and the name of the plant currently. Will be expanded upon. :) :)
	~ Switched Navigation to use Routes to fix the double header bug
	+ Added login error reporting so we can block banned users from logging in
	~ Updated methods to account for is_admin column on database
	~ Squashed some bugs with the UserSearchPage and BannedUserPage
	+ Admin page now shows up for admin users
	+ CommunityFeedPage is now functional in terms of displaying to the XML. Just need to grab the data from the database for displayin rather than hard-coded posts


Sprint2 Changes:
	+ Constructed a 'users' table, with columns 'name','password', 'email', and 'is_banned'
	+ Created the User object class and added methods to the BusinessLogic and Database to select, insert, and update users :) :)
	+ Added some functionality to LoginPage allowing users existing in the users table to access the functions of the app :)
	+ Added functionality to CreateAccountPage allowing users to create a new account, thus adding their user to the table for future login :)
	+ Created navigation from LoginPage to AppShell
	~ Moved all pages besides App and AppShell into Pages folder for organization purposes
	+ Added icons and made tab names more compact for Map, Camera, and Feed tabs; reduced amount of tabs :) :)
	+ Set up project database and put access information in Constants.cs
	+ Designed and implemented a table for MapPage to store Pin information with consideration for future feature implementation :) :) :)
	~ Researched Pin event handlers to customize on-tap actions; wasn't able to get working as planned so commented out for later reference
	+ Leverage generic information window for on-tap event on Pins to show plant names and unique ID numbers fetched dynamically from SQL database; not final design :)
	~ Modified the BannedUserPage and UserSearchPage to work with the database and user.cs object instead of sample data
	~ Updated the User object class and database methods to work with the IsBanned boolean

