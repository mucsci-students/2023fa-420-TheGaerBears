# TheGaerBears Spelling Bee Game
### Description
<p>This is an adaptation of the <b>New York Times Spelling Bee</b> game. </p>
<p>If you haven't played this before, from the NYT themselves:</p>
<blockquote>
    How many words can you make with 7 letters?
</blockquote>

<p>This game can be played through the console or through our GUI. There are only a few steps to get set up,
follow them below to start playing!</p>

### Design Patterns
<p>
    <ul>
    <li><b>Model View Controller</b>: We have a GameModel, CliView, MainWindowViewModel (GuiView), CliController, and GuiController.</li>
    <li><b>Template</b>: We use an abstract Controller class of which CliController and GuiController are subclasses.</li>
    <li><b>Adapter</b>: The fileService class we use for opening up the file picker in the services folder.</li>
    <li><b>Null Object</b>: We use a NullModel class so that instead of getting a return null result, we can return a nullModel object.</li>
    <li><b>Singleton</b>: Our CliController was refactored as a singleton because we only need one instance of it at any time.</li>
    <li><b>Proxy</b>: We are using a DatabaseAccess class as a proxy to access the database in GameModel.</li>
    </ul>
</p>

### Getting Set Up
1. <p>Download <a href=https://dotnet.microsoft.com/en-us/download/dotnet/6.0>.NET 6.0.414</a> for your operating system. This requires about 4.5Gb of storage according to Microsoft.</p>

2. <p>Navigate into the directory .../SpellingBee in the terminal/console/command prompt.</p>

3. Type the commands:
    #### FOR WINDOWS USERS
    <p>To start the GUI:</p>
    
    ```
    dotnet build
    ```
    ```
    dotnet run
    ```
    
    <p>To start the CLI:</p>
    
    ```
    dotnet run "-cli"
    ```
    <br/>

    #### FOR MAC AND LINUX
    <p>Begin with:</p>
    
    ```
    dotnet build SpellingBee.csproj
    ```
    
    <p>To start the GUI:</p>
    
    ```
    ./bin/Debug/net6.0/SpellingBee
    ```
    
    <p>To start the CLI:</p>

    ```
    ./bin/Debug/net6.0/SpellingBee "-cli"
    ```
    
 ###### Contributors: Skyfa Inthavong, Noah Irgang, Alex Stan, and Ian Martin
