# TheGaerBears Spelling Bee Game
### Description
<p>This is an adaptation of the <b>New York Times Spelling Bee</b> game. </p>
<p>If you haven't played this before, from the NYT themselves:</p>
<blockquote>
    How many words can you make with 7 letters?
</blockquote>

<p>This game can be played through the console or through our GUI. There are only a few steps to get set up,
follow them below to start playing!</p>

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
