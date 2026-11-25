


# QRlabel
A4 sheet label printing with support for Serial numbers, Serialized QRCode printing and handling MAC addresses.

This program was written to print pairs of serial numbered labels, one with text+logo, other with QR code link on sheets of 19mm round labels, but the layout can be readily changed to suit other sized labels.

The program keeps a timestamped log of printed labels and warns if the selected serial number range overlaps labels previously printed.



## Launching the program
The program is a Visual studio C# project for .NET Framework 4.8, and uses the **zxing** library for barcode generation.

Directly install it from http://innovodesign.co.uk/QRLabel/publish.htm.

## Use
The user selects the first serial number and the number of labels to be printed, then clicks [Print] and confirms the printer.

[Preview...] (or File > Print Preview) shows the pages without printing. File > Change Printer selects the printer without printing.

When the layout asks for random serial numbers (see below) the *Random Serial* box is ticked and the serial number cannot be typed in.

The last layout file and printer are remembered in `%ProgramData%\QRlabel\settings.ini`. The file is shared by all Windows users of the PC, but Windows only lets the user who first ran the program change it. Other users can still open layouts and print; they are warned once that their choice will not be remembered.


## Editing the layout
QRlabel uses a text file to describe the layout of the label and the label sheet. Launch the QRlabel program, select layout tab and check 'unlock'. This allows the layout script to be edited, with changes shown live on the right panel.

#### Comments start with a //
```
// Label Template for Widget label
// 19mm round labels on A4 sheet
// Labels-Direct.co.uk SLR19
```
#### Page layout
These entries are required and may be modified to suit your label sheet dimensions and printer.
```
topleft:7.64,30.78
pitch:21.0,41.74
across:9
down:6
```
#### Valid serial number range
May be omitted if not required. Serial numbers up to 9223372036854775807 (63 bits) are supported.
```
// Serial numbers greater than 4095 will require a new MAC-ID block
// and a firmware update to support the new MAC
valid_serial_min:0
valid_serial_max:4095
```
#### Random serial numbers
```
random_serial:true
valid_serial_min:10
valid_serial_max:9223372036854775807
```
Every label gets its own random number between `valid_serial_min` and `valid_serial_max`, taken from the operating system's cryptographic random number generator.

The numbers are not checked for duplicates, so print them with a long field, for example the 14 hexadecimal digits (56 bits) of `[XXXXXXXXXXXXXX]`. A preview shows example numbers; a new set is generated when the labels are printed.
#### Fields and values
Fields are automatically generated values set by the serial number. 

Field | Replaced by
---|---
[N] | '1'
[NN] | '01'
[NNN] | '001'
[NNNN] | '0001'
[NNNNN] | '00001'
[NNNNNN] | '000001'
[X] to [XXXXXXXXXXXXXXXX] | serial number in hexadecimal, one digit per X (1 to 16), zero padded. If the number is longer only its lowest digits are printed
[MAC12] | '0-01' (hexadecimal)
[version] | from version textbox

The system also generates four special numeric values from the width and height of the label

Field | Value
---|---
x | Width of label
cx | Centre-X of label
y | Height of label
cy | Centre-Y of label

Custom fields can also be added by listing values in square brackets:
```
[partnumber]=NXS 0004 0013
[QRlink]=http://your.website.here/find/[N]
[MACID]=01-23-45-67-8[MAC12]
```

#### Layout
```javascript
//Top label, change origin to centre with offset command
offset:cx,cy*0.5
pen:RED,0.2
circle:0,0,9.5
QRcode:0,0,14,[QRlink]
font:verdana,6
text:0,7.6,[N]

//Second label, change origin to centre with offset command
offset:0,cy*1
font:verdana,4.8
text:0,-4,MAC-ID:
text:0,-2,[MACID]
font:verdana,6
text:0,0.7,[partnumber]
font:verdana,8
text:0,3.2,Widget
text:0,6,[version]
//graphic:0,-4,10,6,logo.bmp
pen:RED,0.2
circle:0,0,9.5

offset:0,-cy*0.5
line:9.2,8,9.2,-8
line:-9.2,8,-9.2,-8
line:-4,-2,-4,2
line:4,-2,4,2
```
Co-ordinates for text, QRcode and graphic are centre-x,centre-y.
Graphic files should be placed in the same folder as the label file.

Command | Draws
---|---
`pen:colour,width` | sets the pen for the lines and circles that follow
`font:name,size` | sets the font for the text that follows, size in points
`text:x,y,text` | text centred on x,y
`line:x1,y1,x2,y2` | a line
`circle:x,y,radius` | a circle
`QRcode:x,y,size,content` | a QR code centred on x,y
`graphic:x,y,width,height,file` | a picture file centred on x,y
`offset:dx,dy` | moves the origin for the commands that follow
`clear:` | removes everything defined so far and resets the pen and font

Dimensions are in millimetres and may be written as expressions using `+ - * / ( )` and the values `x`, `y`, `cx` and `cy`. Text and QR code content cannot contain a comma.

## Print records
When labels are printed, lines are appended to the bottom of the label file. For this reason, the label file should not be read-only.

The date is always written as `dd/MM/yyyy HH:mm`, whatever the regional settings of the PC. For a random serial layout the record shows when and how many labels were printed, not the numbers that were printed.

```
PRINTED:15/05/2019 08:00 25-78 V2.01
PRINTED:15/05/2019 08:14 79-294 V2.01
```


