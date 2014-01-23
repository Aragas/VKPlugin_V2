[Rainmeter]
Update=1000

[Variables] 
URL1=http://vk.com
URL2=http://vk.com/images/camera_c.gif
FontColor=255, 255, 255, 255
FontColorOn=128, 192, 255, 255

[MeasureMessages]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Messages
RegExpSubstitute=1
Substitute="1":"Resources/Message.png","0":""
DynamicVariables=1

[MeasureUsers]
Measure=Plugin
Plugin=VKPlugin.dll
Type=List
FriendsCount=5

[MeasureName1]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Name
FriendsCount=5
UserType=1
FriendsCount=5
DynamicVariables=1

[MeasureName2]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Name
UserType=2
DynamicVariables=1

[MeasureName3]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Name
UserType=3
DynamicVariables=1

[MeasureName4]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Name
UserType=4
DynamicVariables=1

[MeasureName5]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Name
UserType=5
DynamicVariables=1

-----------------------------------------------------------------------------------

[MeasureUID1]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Id
UserType=1
DynamicVariables=1

[MeasureUID2]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Id
UserType=2
DynamicVariables=1

[MeasureUID3]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Id
UserType=3
DynamicVariables=1

[MeasureUID4]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Id
UserType=4
DynamicVariables=1

[MeasureUID5]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Id
UserType=5
DynamicVariables=1
==========================================

[MeasureOnlineIcon1]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Status
UserType=1
RegExpSubstitute=1
Substitute="Online":"", "Mobile":"Resources\Mobile.png"
DynamicVariables=1

[MeasureOnlineIcon2]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Status
UserType=2
RegExpSubstitute=1
Substitute="Online":"", "Mobile":"Resources\Mobile.png"
DynamicVariables=1

[MeasureOnlineIcon3]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Status
UserType=3
RegExpSubstitute=1
Substitute="Online":"", "Mobile":"Resources\Mobile.png"
DynamicVariables=1

[MeasureOnlineIcon4]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Status
UserType=4
RegExpSubstitute=1
Substitute="Online":"", "Mobile":"Resources\Mobile.png"
DynamicVariables=1

[MeasureOnlineIcon5]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Status
UserType=5
RegExpSubstitute=1
Substitute="Online":"", "Mobile":"Resources\Mobile.png"
DynamicVariables=1
==========================================
[Image1]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Photo
UserType=1
DynamicVariables=1

[Image2]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Photo
UserType=2
DynamicVariables=1

[Image3]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Photo
UserType=3
DynamicVariables=1

[Image4]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Photo
UserType=4
DynamicVariables=1

[Image5]
Measure=Plugin
Plugin=VKPlugin.dll
Type=Friends
FriendType=Photo
UserType=5
DynamicVariables=1

[MeasureImage1]
Measure=Plugin
Plugin=WebParser.dll
Url=[&Image1]
Download=1
DownloadFile="image1.jpg"
FinishAction=[!UpdateMeter "MeterImage1"]

[MeasureImage2]
Measure=Plugin
Plugin=WebParser.dll
Url=[&Image2]
Download=1
DownloadFile="image2.jpg"
FinishAction=[!UpdateMeter "MeterImage2"]

[MeasureImage3]
Measure=Plugin
Plugin=WebParser.dll
Url=[&Image3]
Download=1
DownloadFile="image3.jpg"
FinishAction=[!UpdateMeter "MeterImage3"]

[MeasureImage4]
Measure=Plugin
Plugin=WebParser.dll
Url=[&Image4]
Download=1
DownloadFile="image4.jpg"
FinishAction=[!UpdateMeter "MeterImage4"]

[MeasureImage5]
Measure=Plugin
Plugin=WebParser.dll
Url=[&Image5]
Download=1
DownloadFile="image5.jpg"
FinishAction=[!UpdateMeter "MeterImage5"]
==========================================

-----------------------------------------------------------------------------------

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
OnUpdateAction=[!Redraw]
DynamicVariables=1
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