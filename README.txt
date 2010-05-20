Introduction
============

This application is intended to keep track of an inventory.
You can add products you own, virtually arrange them in your house,
Tag them and print labels for them.
To see some screenshots visit the project's wiki on github.

Installation
============

Notice: Only running on x86 processors with 32bit for now!

This application is still in a rather early development stage. No make or
configure scripts exists yet and no .deb package has been created yet.
So installation (or better execution) is still not that easy, but it
is not that hard either. 

The project comes with files for the mono ide monodevelop. In order to run the
application, install monodevelop and open the .sln file with it. 
Now activate GtkGui in the Solutions panel and then click the Run button,
(the gears in the toolbar). If you get some error messages at startup,
it is likly that some librarys are missing. So go to your packagemanager
and install the package that is missing. Adding cli or mono to the search makes
finding the right package more likly.

How to use it
=============

Once started up you see three icons on the very left of the window.

   Items      Add items here. Items describe an object in the real world.
   _______    Most of the time this is some sort of product.
  /      /|   An item is not placed somewhere yet. It only describes something
 /______/ |   you could for example buy in a store.
|       | |   You can assign a lot of different information to each item
|       | /   Like a name, description, images, ...
|_______|/    An example for a product might be the book
              The catcher in the Rye.
              So to create a book click on Create Item. Now
              a panel pops up where you can set the id and type of the item.
              Once done so create it. Now you can set additional information
              like name, description etc.

  Locations   Once you created an item, you can place it somewhere.
      _       The basic idea is that an item can be placed in another item.
     | |      So you can create a tree of items that lay in one another.
    _| |_     For example a book might lay in a bookshelf which is part of 
    \   /___  a room which is again part of a house.
   / \ /   /  So if you want to place your just purchaised book, switch to
  /   v   /   the locations view an press Create Location. Now type the name
 /_______/    of your book and press enter. You now just created your first
              located item. You can now create more books or create new 
              items to create a more complex layout. You can then use drag
              and drop to change the layout.

    Tags      In order to group items of a specific type you can create tags.
              Tags are also organized in a tree. So an the Computer tag could
      /|      be a child of the Electronics tag. You can assign as many tags
    -/ |--.   to an item as you want.
   '/ O+--'
   |   |
   |  /   
   | /
   |/   

To see an example inventory, copy the inventory folder in Testing into your homedirectory's .my_inventory folder.

The git repo
============

What files will be found in this git repo?
.cs        - Those are the most important, they represent the actual sourcecode of the project
.txt       - Files with information for humans

.sln       - Monodevelop file that stores infos about a solution (MyInventory)
.csproj    - Mododevelop file that stores infos about a project  (GtkGui,Model, ...)
.pidb      - Are monodevelop internal cached databases for code completion
.userprefs - Preferences for the monodevelop IDE
Files found in the Libs project are precompiled and are not very important

Contact
=======

For bug reports or help requests contact me using the email
konnew(at)gmx.de
