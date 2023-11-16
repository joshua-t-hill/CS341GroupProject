Project Group 3 / Authors:
	Alex Ceithamer [ARC]
	Alexsa Walczak [AMW]
	Joshua T. Hill [JTH]
	Samuel Ayoade [SA]

Resource Attribution:
	maptabicon.png		--> <a href="https://www.flaticon.com/free-icons/pin" title="pin icons">Pin icons created by Freepik - Flaticon</a>
	camertabicon.png	--> <a href="https://www.flaticon.com/free-icons/camera" title="camera icons">Camera icons created by Good Ware - Flaticon</a>
	feedtabicon.png		--> <a href="https://www.flaticon.com/free-icons/list" title="list icons">List icons created by Smartline - Flaticon</a>
	app_icon.png		--> <a href="https://www.flaticon.com/free-icons/hobbies-and-free-time" title="hobbies and free time icons">Hobbies and free time icons created by Vichanon Chaimsuk - Flaticon</a>

Sprint3 Changes:
	~ Researched sending emails; found that we would need to give bank info to another company.. changing idea for now
	+ Added column 'salt' to table 'users'
	+ Encrypted passwords :) :)
	+ Created table 'photos' with columns 'id' and 'image_data'
	+ Connected the camera output to the 'photos' table :)
	+ Created custom Pin objects that allow us to manipulate and track data accross all map interactions :) :) :)
	+ Created a Popup page that responds to the default info window on a Pin being tapped. Displays a picture and the name of the plant currently. Will be expanded upon. :) :)

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

