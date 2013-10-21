[Variables] 
;URL=file://%APPDATA%\Rainmeter\Plugins\VKPlugin\OnlineUsers.tmp
URL1=http://vk.com
URL2=http://vk.com/images/camera_c.gif
FontColor=255, 255, 255, 255
FontColorOn=128, 192, 255, 255
UPDATE=-1

[MeasureMessages]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Messages
RegExpSubstitute=1
Substitute="1":"Message.png","0":""

[MeasureName1]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Name
FriendsCount=5
UserType=1

[MeasureName2]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Name
UserType=2

[MeasureName3]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Name
UserType=3

[MeasureName4]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Name
UserType=4

[MeasureName5]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Name
UserType=5

-----------------------------------------------------------------------------------

[MeasureUID1]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Id
UserType=1

[MeasureUID2]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Id
UserType=2

[MeasureUID3]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Id
UserType=3

[MeasureUID4]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Id
UserType=4

[MeasureUID5]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Id
UserType=5
==========================================

[MeasureOnlineIcon1]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Status
UserType=1
RegExpSubstitute=1
Substitute="Online":"", "Mobile":"Resources\Mobile.png"

[MeasureOnlineIcon2]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Status
UserType=2
RegExpSubstitute=1
Substitute="Online":"", "Mobile":"Resources\Mobile.png"

[MeasureOnlineIcon3]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Status
UserType=3
RegExpSubstitute=1
Substitute="Online":"", "Mobile":"Resources\Mobile.png"

[MeasureOnlineIcon4]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Status
UserType=4
RegExpSubstitute=1
Substitute="Online":"", "Mobile":"Resources\Mobile.png"

[MeasureOnlineIcon5]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Status
UserType=5
RegExpSubstitute=1
Substitute="Online":"", "Mobile":"Resources\Mobile.png"
==========================================
[Image1]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Photo
UserType=1

[Image2]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Photo
UserType=2

[Image3]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Photo
UserType=3

[Image4]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Photo
UserType=4

[Image5]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Photo
UserType=5

[MeasureImage1]
Measure=Plugin
Plugin=Plugins/WebParser.dll
Url=[&Image1]
Download=1
DownloadFile="image1.jpg"

[MeasureImage2]
Measure=Plugin
Plugin=Plugins/WebParser.dll
Url=[&Image2]
Download=1
DownloadFile="image2.jpg"

[MeasureImage3]
Measure=Plugin
Plugin=Plugins/WebParser.dll
Url=[&Image3]
Download=1
DownloadFile="image3.jpg"

[MeasureImage4]
Measure=Plugin
Plugin=Plugins/WebParser.dll
Url=[&Image4]
Download=1
DownloadFile="image4.jpg"

[MeasureImage5]
Measure=Plugin
Plugin=Plugins/WebParser.dll
Url=[&Image5]
Download=1
DownloadFile="image5.jpg"
==========================================

-----------------------------------------------------------------------------------

[TitleVK]
Meter=String
Text=ВКонтакте
X=60
Y=20
StringStyle=BOLD
FontFace=Calibri
FontSize=12
FontColor=250,250,250,255
AntiAlias=1
Hidden=1
LeftMouseUpAction=["#URL1#"]
MouseOverAction=[!SetOption "#CURRENTSECTION#" "FontColor" "#FontColorOn#"][!UpdateMeter #CURRENTSECTION#][!Redraw]
MouseLeaveAction=[!SetOption "#CURRENTSECTION#" "FontColor" "#FontColor#"][!UpdateMeter #CURRENTSECTION#][!Redraw]
==========================================

[StyleNames]
FontFace=Calibri
FontSize=11
FontColor=247,247,247,255
AntiAlias=1
Text=%1
ClipString=1
StringStyle=BOLD
W=200
H=15
X=50
Y=30R
MouseOverAction=[!SetOption "#CURRENTSECTION#" "FontColor" "#FontColorOn#"][!UpdateMeter #CURRENTSECTION#][!Redraw]
MouseLeaveAction=[!SetOption "#CURRENTSECTION#" "FontColor" "#FontColor#"][!UpdateMeter #CURRENTSECTION#][!Redraw]

[MeterName1]
Meter=String
MeterStyle=StyleNames
MeasureName=MeasureName1
Y=20
LeftMouseUpAction=["http://vk.com/id[MeasureUID1]"]

[MeterName2]
Meter=String
MeterStyle=StyleNames
MeasureName=MeasureName2
LeftMouseUpAction=["http://vk.com/id[MeasureUID2]"]

[MeterName3]
Meter=String
MeterStyle=StyleNames
MeasureName=MeasureName3
LeftMouseUpAction=["http://vk.com/id[MeasureUID3]"]

[MeterName4]
Meter=String
MeterStyle=StyleNames
MeasureName=MeasureName4
LeftMouseUpAction=["http://vk.com/id[MeasureUID4]"]


[MeterName5]
Meter=String
MeterStyle=StyleNames
MeasureName=MeasureName5
LeftMouseUpAction=["http://vk.com/id[MeasureUID5]"]
==========================================

[StyleOnlineIcons]
X=53
Y=45r

[MeterOnlineIcon1]
Meter=Image
MeterStyle=StyleOnlineIcons
MeasureName=MeasureOnlineIcon1
Y=10

[MeterOnlineIcon2]
Meter=Image
MeasureName=MeasureOnlineIcon2
MeterStyle=StyleOnlineIcons

[MeterOnlineIcon3]
Meter=Image
MeasureName=MeasureOnlineIcon3
MeterStyle=StyleOnlineIcons

[MeterOnlineIcon4]
Meter=Image
MeasureName=MeasureOnlineIcon4
MeterStyle=StyleOnlineIcons

[MeterOnlineIcon5]
Meter=Image
MeasureName=MeasureOnlineIcon5
MeterStyle=StyleOnlineIcons
==========================================

[StyleImages]
PreserveAspectRatio=1
W=40
H=40
X=10
Y=45r

[MeterImage1]
Meter=IMAGE
ImageName=#CURRENTPATH#\DownloadFile\image1.jpg
MeterStyle=StyleImages
Y=10

[MeterImage2]
Meter=IMAGE
ImageName=#CURRENTPATH#\DownloadFile\image2.jpg
MeterStyle=StyleImages

[MeterImage3]
Meter=IMAGE
ImageName=#CURRENTPATH#\DownloadFile\image3.jpg
MeterStyle=StyleImages

[MeterImage4]
Meter=IMAGE
ImageName=#CURRENTPATH#\DownloadFile\image4.jpg
MeterStyle=StyleImages

[MeterImage5]
Meter=IMAGE
ImageName=#CURRENTPATH#\DownloadFile\image5.jpg
MeterStyle=StyleImages
==========================================

[MeterMessages]
Meter=Image
MeasureName=MeasureMessages
X=179
Y=26
LeftMouseUpAction=["http://vk.com/im"]