
# StarDust.CasparCG.net

Library to allow control a CasparrCG server in .net standard.

CasparCG Server is a Windows and Linux software used to play out professional graphics, audio and video to multiple outputs. It has been in 24/7 broadcast production since 2006.

More info about CasparCG Server [here](https://github.com/CasparCG/server)

Compatible to 2.0.7 version to 2.2 for now.

# Can be use on you .net project

* .net core 2.0 or later 
* .net framework 4.6.1 or later
* and more other [here the compatibility matrix](https://docs.microsoft.com/fr-fr/dotnet/standard/net-standard)

| | Badges |
| -- | -- |
Build | [![Build status](https://dust63.visualstudio.com/StarDust.CasparCG.net/_apis/build/status/StarDust.CasparCG.net-CI)](https://dust63.visualstudio.com/StarDust.CasparCG.net/_build/latest?definitionId=1)
Nuget | [![NuGet](http://img.shields.io/nuget/v/StarDust.CasparCg.net.Device.svg)](https://www.nuget.org/packages/StarDust.CasparCg.net.Device/) [![NuGet](https://img.shields.io/nuget/dt/StarDust.CasparCg.net.Device.svg)](https://www.nuget.org/packages/StarDust.CasparCg.net.Device/)



# Quick Start up

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
  
 
 
            
            
  

