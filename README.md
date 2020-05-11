
# StarDust.CasparCG.net

Is a set of libraries wrote in .net standard to allow control a CasparrCG server and receive OSC messages.


CasparCG Server is a Windows and Linux software used to play out professional graphics, audio and video to multiple outputs. It has been in 24/7 broadcast production since 2006.

More info about CasparCG Server [here](https://github.com/CasparCG/server)  
You can contact me or discuss about this lib [here](https://casparcgforum.org/t/net-library-stardust-casparcg-net/1426)

Compatible to 2.0.7 version to 2.2 of CasparCG for now.
Currently testing on 2.3 please feel free to test and report issue if you found one.



| | Badges |
| -- | -- |
Build | [![Build status](https://dust63.visualstudio.com/StarDust.CasparCG.net/_apis/build/status/StarDust.CasparCG.net-CI)](https://dust63.visualstudio.com/StarDust.CasparCG.net/_build/latest?definitionId=1)
Nuget for AMCP Control | [![NuGet](http://img.shields.io/nuget/v/StarDust.CasparCg.net.Device.svg)](https://www.nuget.org/packages/StarDust.CasparCg.net.Device/) [![NuGet](https://img.shields.io/nuget/dt/StarDust.CasparCg.net.Device.svg)](https://www.nuget.org/packages/StarDust.CasparCg.net.Device/)
Nuget for OSC | [![NuGet](http://img.shields.io/nuget/v/StarDust.CasparCg.net.OSC.svg)](https://www.nuget.org/packages/StarDust.CasparCg.net.OSC/) [![NuGet](https://img.shields.io/nuget/dt/StarDust.CasparCg.net.OSC.svg)](https://www.nuget.org/packages/StarDust.CasparCg.net.OSC/)



# Write in .net Standard 2.0. 
**List of supported .net framework.**

* .net core 2.0 or later 
* .net framework 4.6.1 or later
* and more other [here the compatibility matrix](https://docs.microsoft.com/fr-fr/dotnet/standard/net-standard)


# Quick Start up for AMCP control

**Use of dependency injection**

The library can be use with Dependency Injection. In this example we use Unity.
Register all dependencies:
Snippet

```csharp       
static IUnityContainer _container;

     
static void ConfigureIOC()
{
 _container = new UnityContainer();
 _container.RegisterInstance<IServerConnection>(new ServerConnection(new CasparCGConnectionSettings("127.0.0.1")));
 _container.RegisterType(typeof(IAMCPTcpParser), typeof(AmcpTCPParser));
 _container.RegisterSingleton<IDataParser, CasparCGDatasParser>();
 _container.RegisterType(typeof(IAMCPProtocolParser), typeof(AMCPProtocolParser));
 _container.RegisterType<ICasparDevice, CasparDevice>(new ContainerControlledLifetimeManager());
}
```

**Initialize connection**  
      
Then you just need to call the Configure IOC and enjoy ;):
  
```csharp
ConfigureIOC();

//Get casparCG device instance
var casparCGServer = _container.Resolve<ICasparDevice>();

//Handler to be notify if the Server is connected or disconnected
 casparCGServer.ConnectionStatusChanged += CasparDevice_ConnectionStatusChanged;
 
//Initialize the connection
casparCGServer.Connect();
```
**Work with server**

 You can get the version of the current CasparCG Server:
```csharp
var casparCGServer = _container.Resolve<ICasparDevice>();
Console.WriteLine(casparCGServer.GetVersion());
 ``` 
 Or clips list:
 
 ````csharp
 var casparCGServer = _container.Resolve<ICasparDevice>();
 var clips = casparCGServer.GetMediafiles()
 ````
 
 **Work with Channel Manager:**
 
 At the first connection the code will retrieve all channels available.
 The library declare a Channel manager for each channel. Channel manager has the amcp command that require a channel ID.
 If you want to play a clip on a channel
  
 ```csharp        
var channel = casparCGServer.Channels.First(x => x.ID == 1);
channel.LoadBG(new CasparPlayingInfoItem { VideoLayer = 1, Clipname = "AMB" });
channel.Play(1);
 ``` 
 
**Work with CG Manager:**
A CG Manager is present on each Channel Manager
If you want to play a template:
 
  ```csharp       
var channel = casparCGServer.Channels.First(x => x.ID == 1);
channel.CG.Add(10, 1, "caspar_text");
channel.CG.Play(10, 1);
  ``` 
**Work with Mixer Manager:**
A Mixer Manager is present on each Channel Manager
If you want to play with the mixer here we set the brigthness:
  
   ```csharp      
   var channel = casparCGServer.Channels.First(x => x.ID == 1);
   channel.Mixer.Brightness(1, 0.2F);
            
   ``` 
  
 **Demo project**
 
 You can see more example in [demo project](https://github.com/dust63/StartDust.CasparCG.net/tree/master/src/Demo/Demo%20AMCP).
 
 
 # Quick Start up to receive OSC Message

**Use of dependency injection**
 
 ```csharp      
  _container = new UnityContainer();
  _container.RegisterType<IOscListener, OscListener>(new ContainerControlledLifetimeManager());            
 ``` 
 
 **Initialize the connection to listen to OSC message**
 
 ```csharp 
 //Get an instance of OcsListener from Unity
 var oscListener = _container.Resolve<IOscListener>();
 //Attach to event to get the OSC message when received
 oscListener.OscMessageReceived += OscListener_OscMessageReceived;
 //Begin to listen to OSC Message from CasparCG
 oscListener.StartListening("127.0.0.1", 6250);          
 ``` 
 
 **Stop to listen**
 
  ```csharp 
  oscListener.StopListening();
  ``` 
  
  **Can filter to receive notification only for some address**
  
  ```csharp 
  //Filter for a simple address
  oscListener.AddToAddressFiltered("/channel/1/stage/layer/1/file/time");
  //Filter for a range of address. Here we get all address for layer to 1-1000... and channel 1-10000...
  oscListener.AddToAddressFiltered("/channel/[0-9]/stage/layer/[0-9]/file/time");
  //Filter by regex. Here we want all message that begin by /channel/1/stage/layer/1 and not ended by time
  oscListener.AddToAddressFilteredWithRegex("^/channel/[0-9]/stage/layer/1(?!.*?time)");
  ``` 
  
  **Or you can simply black list an address to don't be notify for it**
  
  ```csharp 
    //I don't want to be notify for this address, in this case [0-9] means for all channels
    oscListener.AddToAddressBlackList("/channel/[0-9]/output/consume_time");
    //Or you can use also a regex
    oscListener.AddToAddressBlackListWithRegex("^/channel/[0-9]/stage/layer/1(?!.*?time)");
 ```
 
  **Remove from filtered list address**
 
 Pass the address or the pattern that you add before
 
 ```csharp
 oscListener.RemoveFromAddressFiltered("/channel/1/stage/layer/1/file/time"); 
 ```
 
 **Remove from black list address**
 
 Pass the address or the pattern that you add before
 
 ```csharp
 oscListener.RemoveFromAddressBlackListed("/channel/[0-9]/output/consume_time"); 
 ```
  **Demo project**
 
 You can see more example in [demo project](https://github.com/dust63/StartDust.CasparCG.net/tree/master/src/Demo/Demo%20OSC).
 
 # Quick Start up for OSC Event hub [WIP]
 
 OSC Event hub is a class that capture the osc message parse the address and trigger .net event.
 
 How to start up. Obviously ready for the Dependency injection. So you need to map interface and class.
 OSC Event hub has a dependency to the Osc listener.
 
 
 ```csharp      
  _container = new UnityContainer();
  _container.RegisterType<IOscListener, OscListener>(new ContainerControlledLifetimeManager());  
  _container.RegisterType<ICasparGCOscEventHub, CasparCGOscEventHub>(new ContainerControlledLifetimeManager());  
 ``` 
 
 
 
 # What I need to do next:
 
 * Abstract looging to add logging
 * Fill missing XML Doc. Some help is welcome :p
 * Implement lib that trigger event for CasparCG OSC messages
 
  # Issue or Feedback
 * For some enhancement request, please [open a ticket](https://github.com/dust63/StartDust.CasparCG.net/issues). 
 * Feel free for feedcback on the [caspar CG Forum topic]( https://casparcgforum.org/t/net-library-stardust-casparcg-net/1426)
 
 
            
            
  

