using Moq;
using Rug.Osc;
using StarDust.CasparCG.net.OSC;
using StarDust.CasparCG.net.OSC.EventHub;
using StarDust.CasparCG.net.OSC.EventHub.Events;
using Xunit;

namespace StartDust.CasparCG.net.UnitTest
{
    public class CasparCGOscEventsHubTest
    {
        [Fact]
        public void Test_ProfilerTimeChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedCurrent = 0.041f;
            var expectedExpected= 0.04f;

            ProfilerTimeEventArgs args = null;
            oscEventHub.ProfilerTimeChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/profiler/time", expectedCurrent,expectedExpected)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);         
            Assert.Equal(args.ExpectedTime, expectedExpected);
            Assert.Equal(args.ActualTime, expectedCurrent);
        }


        [Fact]
        public void TestFlashBufferChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedBuffer = 200;



            BufferEventArgs args = null;
            oscEventHub.FlashProducerBufferChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/stage/layer/10/buffer", expectedBuffer)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.Buffer, expectedBuffer);
        }

        [Fact]
        public void Test_MixerAudioDbfsChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedDbfs = -20F;



            MixerAudioDbfsEventArgs args = null;
            oscEventHub.MixerAudioDbfsChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/mixer/audio/dBFS", expectedDbfs)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(args.Dbfs, expectedDbfs);
        }


        [Fact]
        public void Test_MixerAudioChannelsCountChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedCount = 2;



            MixerAudioChannelsCountEventArgs args = null;
            oscEventHub.MixerAudioChannelsCountChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/mixer/audio/nb_channels", expectedCount)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);         
            Assert.Equal(args.ChannelsCount, expectedCount);
        }


        [Fact]
        public void Test_ConsumerFrameCreatedChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedframesCreated = 200;
            var expectedAvailableFrames = 999999999;
            var expectedPortId = 9;

            ConsumerFrameCreatedEventArgs args = null;
            oscEventHub.ConsumerFrameCreatedChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/output/port/{expectedPortId}/frame", expectedframesCreated, expectedAvailableFrames)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(args.PortId, (uint)expectedPortId);
            Assert.Equal(args.FramesCreated, expectedframesCreated);
            Assert.Equal(args.AvailableFrames, expectedAvailableFrames);
        }

        [Fact]
        public void Test_TemplateFpsChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedFps = 25F;

            TemplateFpsEventArgs args = null;
            oscEventHub.TemplateFpsChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/stage/layer/10/host/fps", expectedFps)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.TemplateFps, expectedFps);
        }


        [Fact]
        public void Test_TemplateWidthChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedWidth = 1920;

            TemplateWidthEventArgs args = null;
            oscEventHub.TemplateWidthChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/stage/layer/10/host/width", expectedWidth)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.TemplateWidth, (uint)expectedWidth);
        }


        [Fact]
        public void Test_TemplateHeightChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedHeight = 1080;

            TemplateHeightEventArgs args = null;
            oscEventHub.TemplateHeightChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/stage/layer/10/host/height", expectedHeight)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.TemplateHeight, (uint)expectedHeight);
        }


        [Fact]
        public void Test_TemplatePathChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedPath = "template_file.ft";

            TemplatePathEventArgs args = null;
            oscEventHub.TemplatePathChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/stage/layer/10/host/path", expectedPath)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.TemplatePath, expectedPath);
        }


        [Fact]
        public void Test_LayerPaused()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedPaused = true;
         
            LayerPausedEventArgs args = null;
            oscEventHub.LayerPausedChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/stage/layer/10/paused", expectedPaused)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.IsPause, expectedPaused);      
        }

        [Fact]
        public void Test_LayerProfilerChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedActual = 0.39F;
            var expectedExpected = 0.4F;

            LayerProfilerEventArgs args = null;
            oscEventHub.LayerProfilerChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/stage/layer/10/profiler/time",expectedActual, expectedExpected)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.ActualValue, expectedActual);
            Assert.Equal(args.ExpectedValue, expectedExpected);
        }

        [Fact]
        public void Test_LayerTypeChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedType = "transition";

            LayerTypeEventArgs args = null;
            oscEventHub.LayerTypeChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/stage/layer/10/type", expectedType)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.LayerType, expectedType);
        }

        [Fact]
        public void Test_BackgroundLayerTypeChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedType = "transition";

            LayerTypeEventArgs args = null;
            oscEventHub.BackgroundLayerTypeChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/stage/layer/10/background/type", expectedType)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.LayerType, expectedType);
        }

        [Fact]
        public void Test_LayerActiveFrameChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedFrame = 2524;

            LayerActiveFrameEventArgs args = null;
            oscEventHub.LayerActiveFrameChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/stage/layer/10/frame", expectedFrame)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.ActiveFrame, (uint)expectedFrame);
        }

        [Fact]
        public void Test_LayerActiveTimeChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedTime = 101.24F;          

            LayerActiveTimeEventArgs args = null;
            oscEventHub.LayerActiveTimeChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/stage/layer/10/time", expectedTime)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.ActiveTime, expectedTime);
        }

        [Fact]
        public void Test_OutputFormatChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedFormat = "PAL";
          

            OutputFormatEventArgs args = null;
            oscEventHub.OutputFormatChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/format", expectedFormat)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(args.Format, expectedFormat);           
        }

        [Fact]
        public void Test_OutputPortChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedType= "screen";       
            var expectedPortId = 9;

            OutputPortTypeEventArgs args = null;
            oscEventHub.OutputPortChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/output/port/{expectedPortId}/type", expectedType)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(args.PortId, (uint)expectedPortId);
            Assert.Equal(args.Type, expectedType);     
        }


        [Fact]
        public void Test_ClipChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedClip = "TEST/GO1080P25";

            PlaybackClipClipChangedEventArgs args = null;
            oscEventHub.PlaybackClipChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/file/name", expectedClip)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.ActiveClip, expectedClip);

        }

        [Fact]
        public void Test_FrameRateChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedFps = 23.98F;
            uint expectedStreamId = 5;

            StreamFramerateEventArgs args = null;
            oscEventHub.StreamFrameRateChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage($"/channel/1/stage/layer/10/background/file/{expectedStreamId}/fps", expectedFps)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.Fps, expectedFps);
            Assert.Equal(args.StreamId, expectedStreamId);

        }



        [Fact]
        public void Test_ClipPathChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedClip = @"D:\MEDIA\TEST\GO1080P25";

            PlaybackClipPathEventArgs args = null;
            oscEventHub.PlaybackClipPathChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/file/path", expectedClip)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.Path, expectedClip);
        }

        [Fact]
        public void Test_ClipTimeChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedElapsed = 10.2f;
            var expectedTotalElapsed = 30.5f;

            PlaybackClipTimeEventArgs args = null;
            oscEventHub.PlaybackClipTimeChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/file/time", expectedElapsed, expectedTotalElapsed)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.SecondsElapsed, expectedElapsed);
            Assert.Equal(args.TotalSeconds, expectedTotalElapsed);

        }

        [Fact]
        public void Test_ClipFrameChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            int expectedElapsed = 10;
            int expectedTotalElapsed = 30;

            PlaybackClipFrameEventArgs args = null;
            oscEventHub.PlaybackClipFrameChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/file/frame", expectedElapsed, expectedTotalElapsed)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.FramesElapsed, (uint)expectedElapsed);
            Assert.Equal(args.TotalFrames, (uint)expectedTotalElapsed);

        }

        [Fact]
        public void Test_ClipWidthChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            int expectedWidth = 1920;


            PlaybackClipWidthEventArgs args = null;
            oscEventHub.PlaybackClipWidthChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/file/video/width", expectedWidth)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.Width, (uint)expectedWidth);
        }

        [Fact]
        public void Test_ClipHeightChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            int expectedHeight = 1080;


            PlaybackClipHeightEventArgs args = null;
            oscEventHub.PlaybackClipHeightChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/file/video/height", expectedHeight)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.Height, (uint)expectedHeight);
        }

        [Fact]
        public void Test_ClipFieldChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);


            PlaybackClipFieldEventArg args = null;
            oscEventHub.PlaybackClipFieldChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/file/video/field", "progressive")));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.True(args.IsProgressive);

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/file/video/field", "interlaced")));

            Assert.False(args.IsProgressive);
        }

        [Fact]
        public void Test_ClipLoopChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedLoop = true;

            PlaybackLoopEventArgs args = null;
            oscEventHub.PlaybackLoopChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/loop", expectedLoop)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(expectedLoop, args.IsLoop);

            expectedLoop = false;
            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/loop", expectedLoop)));
            Assert.Equal(expectedLoop, args.IsLoop);
        }



        [Fact]
        public void Test_ClipAudioFormatChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedFormat = "s16";

            PlaybackClipAudioFormatEventArg args = null;
            oscEventHub.PlaybackClipAudioFormatChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/file/audio/format", expectedFormat)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(expectedFormat, args.Format);
        }

        [Fact]
        public void Test_ClipVideoCodecChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedCodec = "H.264 /AVC";

            PlaybackClipCodecEventArg args = null;
            oscEventHub.PlaybackClipVideoCodecChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/file/video/codec", expectedCodec)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(expectedCodec, args.Codec);
        }

        [Fact]
        public void Test_ClipAudioChannelsChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedChannels = 4;

            PlaybackClipAudioChannelsEventArg args = null;
            oscEventHub.PlaybackClipAudioChannelsChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/file/audio/channels", expectedChannels)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal((uint)expectedChannels, args.Channels);
        }

        [Fact]
        public void Test_ClipAudioSampleRateChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedSr = 48000;

            PlaybackClipAudioSampleRateEventArg args = null;
            oscEventHub.PlaybackClipAudioSampleRateChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/file/audio/sample-rate", expectedSr)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal((uint)expectedSr, args.SampleRate);
        }

        [Fact]
        public void Test_ClipAudioCodecChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedCodec = "expectedCodec";

            PlaybackClipCodecEventArg args = null;
            oscEventHub.PlaybackClipAudioCodecChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/file/audio/codec", expectedCodec)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(expectedCodec, args.Codec);
        }




        [Fact]
        public void Test_ClipFrameRateChanged()
        {
            var mockOscListener = new Mock<IOscListener>();
            var oscEventHub = new CasparCGOscEventsHub(mockOscListener.Object);
            var expectedFps = 23.98F;


            PlaybackClipFrameRateEventArgs args = null;
            oscEventHub.PlaybackClipFrameRateChanged += (s, e) =>
            {
                args = e;
            };

            mockOscListener.Raise(f => f.OscMessageReceived += null, this, new OscMessageEventArgs(new OscMessage("/channel/1/stage/layer/10/background/file/fps", expectedFps)));

            Assert.NotNull(args);
            Assert.Equal(1, args.ChannelId);
            Assert.Equal(10, args.LayerId);
            Assert.Equal(args.FramesRate, expectedFps);

        }



    }
}
