
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
Nuget for OSC Event Hub | [![NuGet](http://img.shields.io/nuget/v/StarDust.CasparCG.net.OSC.EventHub.svg)](https://www.nuget.org/packages/StarDust.CasparCG.net.OSC.EventHub/2020.5.11.2-alpha/) [![NuGet](https://img.shields.io/nuget/dt/StarDust.CasparCG.net.OSC.EventHub.svg)](https://www.nuget.org/packages/StarDust.CasparCG.net.OSC.EventHub/)



# Write in .net Standard 2.0 and .net Standard 2.1. 
**List of supported .net framework.**

* .net core 2.0 or later 
* and more other [here the compatibility matrix](https://docs.microsoft.com/fr-fr/dotnet/standard/net-standard)


# Quick Start up for AMCP control

## Install nuget package

```
dotnet add package StarDust.CasparCG.net.Microsoft.DependencyInjection
```

**Use of dependency injection**

The library can be use with Dependency Injection. In this example we use Microsoft Dependency Injection.
Register all dependencies:
Snippet

```csharp 
static void ConfigureIOC(IServiceCollection services)
{
    services.AddCasparCG();
}
```

**Initialize connection**  
      
Then you just need to call the Configure IOC and enjoy ;):
  
```csharp
ConfigureIOC();

//Get casparCG device instance
var casparCGServer = serviceProvider.GetService<ICasparDevice>();

//Handler to be notify if the Server is connected or disconnected
 casparCGServer.ConnectionStatusChanged += CasparDevice_ConnectionStatusChanged;
 
//Initialize the connection
casparCGServer.Connect("127.0.0.1");
```
**Work with server**

 You can get the version of the current CasparCG Server:
```csharp
var casparCGServer = serviceProvider.GetService<ICasparDevice>();
Console.WriteLine(casparCGServer.GetVersion());
 ``` 
 Or clips list:
 
 ````csharp
var casparCGServer = serviceProvider.GetService<ICasparDevice>();
 var clips = casparCGServer.GetMediafiles();
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

## Install nuget package

```
dotnet add package StarDust.CasparCG.net.OSC 
```

**Use of dependency injection**
 
 ```csharp      
  _container = new UnityContainer();
  _container.RegisterType<IOscListener, OscListener>(new ContainerControlledLifetimeManager()); 
  ///Create a connection to CasparCG Server to allow osc message to be sent
   _container.RegisterInstance<IServerConnection>( new ServerConnection(),new SingletonLifetimeManager());
 ``` 
 
 **Initialize the connection to listen to OSC message**
 
 ```csharp 
 ///Connect to CasparCG to allow OSC message to be sent to osc client
  _container.Resolve<IServerConnection>().Connect(new CasparCGConnectionSettings("127.0.0.1"));
  
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
 
 ## Install nuget package
 
 `dotnet add package StarDust.CasparCG.net.OSC.EventHub`
 
 ```csharp      
  _container = new UnityContainer();
  _container.RegisterType<IOscListener, OscListener>(new ContainerControlledLifetimeManager());
  _container.RegisterType<ICasparCGOscEventsHub, CasparCGOscEventsHub>();
  
  ///Create a connection to CasparCG Server to allow osc message to be sent
   _container.RegisterInstance<IServerConnection>( new ServerConnection(),new SingletonLifetimeManager());
   
  var eventHub = _container.Resolve<ICasparCGOscEventsHub>();
            
  //Attach the desire event
  eventHub.PlaybackClipChanged += OnPlaybackClipChanged;
  
  ///Connect to CasparCG to allow OSC message to be sent to osc client
 _container.Resolve<IServerConnection>().Connect(new CasparCGConnectionSettings("127.0.0.1"));
 
  //Start to listen Osc message on 6250 port. Check your Osc port on the config of Caspar CG
  eventHub.CasparCgOscListener.StartListening(6250);
 ``` 
 ## Ouput OSC Message/Event
 
<table>
    <tbody>
        <tr><th>Address</th><th></th><th>Example Arguments</th><th>Description</th><th>Event</th></tr>
        <tr><td rowspan="4">/channel/[0-9]/</td><td>format</td><td>PAL</td><td>The
                <a href="/wiki/CasparCG_Server#Video_Formats" title="CasparCG
                    Server">video format</a> of the channel</td><td>OutputFormatChanged</td></tr>
        <tr><td>profiler/time</td><td>0.041 | 0.04</td><td>The amount of time
                that CasparCG Server is spending rendering the frame, two
                arguments are sent in this message, what it is and what it
                should be as shown in the example.</td><td>ProfilerTimeChanged</td></tr>
        <tr><td>output/port/<i>[0-9]</i>/type</td><td>screen</td><td>A message
                like this will exists for each of the outputs in use, with the
                default CasparCG config file in use this will result in two
                rows; one of type screen, and one of type system-audio. Current
                types are [[screen|Screen Consumer]], system-audio,
                [[decklink|Decklink Consumer]], [[bluefish|Bluefish Consumer]]
                and [[file|Disk Consumer]]</td><td>OutputPortChanged</td></tr>
        <tr><td>output/port/[0-9]/frame</td><td>200 | 922222222888836854</td><td>The
                number of frames that have been created by the consumer on this
                port, the example indicates that 200 frames have been written to
                disk and that a maximum of 922222222888836854 can be written.
            </td><td>ConsumerFrameCreatedChanged</td></tr>
    </tbody>
</table>
 
 ## Stage Messages
Stage messages contain properties related to CasparCG Server layers and the
producers that are contained within them.

### General Stage messages
The following messages do not directly relate to a producer.

<table><tbody>
        <tr><th>Address</th><th></th><th>Example Arguments</th><th>Description</th><th>Event</th></tr>
        <tr><td rowspan="6">/channel/[0-9]/stage/layer/[0-9]/</td><td>time</td><td>101.24</td><td>Seconds
                the layer has been active</td><td>LayerActiveTimeChanged</td></tr>
        <tr><td>frame</td><td>2531</td><td>Time in frames that the layer has
                been active</td><td>LayerActiveFrameChanged</td></tr>
        <tr>
            <td>type</td><td>transition</td>
            <td></td>    
            <td>LayerTypeChanged</td>
        </tr>
        <tr>
            <td>background/type</td><td>empty</td><td></td><td>BackgroundLayerTypeChanged</td>
        </tr>
        <tr>
            <td>profiler/time</td><td>0.39 | 0.4</td><td>Actual | Expected time on frame</td><td>LayerProfilerChanged</td>
            </tr>
        <tr>
            <td>paused</td><td>True/False</td><td>Whether the layer is paused or
                not</td><td>LayerPausedChanged</td></tr>
    </tbody></table>

### FFMPEG Producer

| Address | Example Arguments | Description | Event |
| --- | --- | --- | --- |
| file/time | 12 / 400 | Seconds elapsed on file playback / Total Seconds | PlaybackClipTimeChanged |
| file/frame | 300 / 10000 | Frames elapsed on file playback / Total frames | PlaybackClipFrameChanged |
| file/fps | 25 | Framerate of the file being played | PlaybackClipFrameRateChanged |
| file/path | AMB.mp4 | Filename and path (if file is in a sub-folder) of the media file, paths relative to the media folder defined in the config file | PlaybackClipPathChanged |
| file/video/width | 1920 | Frame width of the video file | PlaybackClipWidthChanged |
| file/video/height | 1080 | Frame height of the video file | PlaybackClipHeightChanged |
| file/video/field | progressive | Scan type of the video file, progressive or interlaced | PlaybackClipFieldChanged |
| file/video/codec | H.264 /AVC | Codec of the video file | PlaybackClipVideoCodecChanged |
| file/audio/sample-rate | 48000 | Audio sample rate of this files audio track | PlaybackClipAudioSampleRateChanged |
| file/audio/channels | 2 | Number of channels in this files audio track | PlaybackClipAudioChannelsChanged |
| file/audio/format | s16 | Audio compression format, in this case uncompressed 16 bit PCM audio | PlaybackClipAudioFormatChanged |
| file/audio/codec | AAC | Audio codec for the audio track in this file	 | PlaybackClipAudioCodecChanged |
| loop | 1 | Whether the file is set to loop playback or not, only applies to ffmpeg inputs of type file not stream or device. | PlaybackLoopChanged |
    
### Flash Producer
The messages below may be produced when an object utilising the [[Flash Producer]] is in use on the stage.

<table>
<tbody><tr>
<th> Address</th>
<th></th>
<th> Example Arguments</th>
<th> Description</th>
<th> Event</th>
</tr>
<tr>
<td rowspan="4"> /channel/[0-9]/stage/layer/[0-9]/host/
</td><td> path</td>
<td> template_file.ft</td>
<td></td>
<td>TemplatePathChanged</td>
</tr>
<tr>
<td> width
</td><td> 1920
</td><td>
</td>
<td>TemplateWidthChanged</td>
</tr>
<tr>
<td> height
</td><td> 1080
</td><td>
</td>
<td>TemplateHeightChanged</td>
</tr>
<tr>
<td> fps
</td><td> 50
</td><td>
</td>
<td>TemplateFpsChanged</td>
</tr>
<tr>
<td> /channel/[0-9]/stage/layer/[0-9]/
</td><td> buffer
</td><td>
</td><td>
</td>
<td>FlashProducerBufferChanged</td>
</tr></tbody></table>
  
## Mixer Messages
The majority of information from CasparCG's compositing module is not yet made available via OSC (an issue is open for this), the only information available currently is audio related as listed below.

<table>
<tbody><tr>
<th> Address
</th><th>
</th><th> Example Arguments
</th><th> Description
</th>
<th> Event
</th></tr>
<tr>
<td rowspan="2"> /channel/[0-9]/mixer/audio/
</td><td> nb_channels
</td><td> 2
</td><td> Number of audio channels in use on this CasparCG channel
</td>
<td> MixerAudioChannelsCountChanged
</td></tr>
<tr>
<td> [0-9]/dBFS
</td><td> -20
</td><td> Audio level in dBFS
</td>
<td> MixerAudioDbfsChanged
</td></tr></tbody></table>
    
 # What I need to do next:
 
 * Abstract looging to add logging
 * Fill missing XML Doc. Some help is welcome :p
 * Implement lib that trigger event for CasparCG OSC messages
 
  # Issue or Feedback
 * For some enhancement request, please [open a ticket](https://github.com/dust63/StartDust.CasparCG.net/issues). 
 * Feel free for feedcback on the [caspar CG Forum topic]( https://casparcgforum.org/t/net-library-stardust-casparcg-net/1426)
