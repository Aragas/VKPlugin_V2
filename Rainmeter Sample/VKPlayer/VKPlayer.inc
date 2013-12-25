[VKPlugin]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Player
PlayerType=SETTINGS
SaveAudio=False

[State]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Player
PlayerType=STATE
Substitute="0":"Player\MusicPlay.W.png","1":"Player\MusicPause.W.png","0":"Player\MusicPlay.W.png"

[Artist]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Player
PlayerType=ARTIST

[Title]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Player
PlayerType=TITLE

[Progress]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Player
PlayerType=PROGRESS

[Repeat]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Player
PlayerType=REPEAT
Substitute="0":"255","1":"128"

[Shuffle]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Player
PlayerType=SHUFFLE
Substitute="0":"255","1":"128"

[MeterArtist]
Meter=STRING
MeasureName=Artist
X=10
Y=0
FontFace=Proxima
FontSize=10
FontColor=250,250,250,255
AntiAlias=1

[MeterTitle]
Meter=STRING
MeasureName=Title
X=10
Y=15
FontFace=Proxima
FontSize=10
FontColor=250,250,250,255
AntiAlias=1


[MeterPrev]
Meter=Image
MeterStyle=StyleButtons
ImageName=Player\MusicPrevious.W.png
LeftMouseUpAction=[!CommandMeasure "VKPlugin" "Previous"][!Update]
X=12

[MeterPlayPause]
Meter=Image
MeterStyle=StyleButtons
MeasureName=State
LeftMouseUpAction=[!CommandMeasure "VKPlugin" "PlayPause"][!Update]

[MeterNext]
Meter=Image
MeterStyle=StyleButtons
ImageName=Player\MusicNext.W.png
LeftMouseUpAction=[!CommandMeasure "VKPlugin" "Next"][!Update]

[MeterRepeat]
Meter=Image
MeterStyle=StyleButtons
ImageName=Player\MusicRepeat.W.png
ImageAlpha=[Repeat]
LeftMouseUpAction=[!CommandMeasure "VKPlugin" "SetRepeat -1"][!Update]
DynamicVariables=1

[MeterShuffle]
Meter=Image
MeterStyle=StyleButtons
ImageName=Player\MusicShuffle.W.png
ImageAlpha=[Shuffle]
LeftMouseUpAction=[!CommandMeasure "VKPlugin" "SetShuffle -1"][!Update]
DynamicVariables=1

[StyleButtons]
W=12
H=12
X=20r
Y=35

[MeterProgress]
MeasureName=Progress
Meter=BAR
Flip=0
BarColor=185,250,160,255
SolidColor=150,150,150,255
BarOrientation=Horizontal
X=12
Y=50
W=180
H=10