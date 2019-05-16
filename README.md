


# QRlabel
A4 sheet label printing with support for Serial numbers, Serialized QRCode printing and handling MAC addresses.

This program was written to print pairs of serial numbered labels, one with text+logo, other with QR code link on sheets of 19mm round labels, but the layout can be readily changed to suit other sized labels.

The program keeps a timestamped log of printed labels and warns if the selected serial number range overlaps labels previously printed.


## Launching the program
The program is a Visual studio C# project, and uses the **zxing** library for barcode generation.

## Use
The user selects the first serial number and the number of labels to be printed, then click [Print] 


## Editing the layout
QRlabel uses a text file to describe the layout of the label and the label sheet. Launch the QRlabel program, select layout tab and check 'unlock'. This allows the layout script to be edited, with changes shown live on the right panel.

### Comments start with a //
```
//Label Template for NVLink2
// 19mm round labels on A4 sheet
// Labels-Direct.co.uk SLR19
```
### Page layout
Modify these entries to suit your label sheet dimensions and printer.
```
topleft:7.64,30.78
pitch:21.0,41.74
across:9
down:6
```
### Valid serial number range
May be omitted if not required
```
// Serial numbers greater than 4095 will require a new MAC-ID block
// and a firmware update to support the new MAC
valid_serial_min:0
valid_serial_max:4095
```
### Fields and values
Fields are automatically generated values set by the serial number as follows

Field | Replaced by
---|---
[N] | '1'
[NN] | '01'
[NNN] | '001'
[NNNNNN] | '000001'
[MAC12] | '0-01' (hexadecimal)
[version] | from version textbox

The label size also generates four special numeric values from the width and height of the label

Field | Value
---|---
x | Width of label
cx | Centre-X of label
y | Height of label
cy | Centre-Y of label

In addition, custom fields can also be added by listing values in square brackets:
```
[QRlink]=http://your.website.here/find/[N]
[MACID]=01-23-45-67-8[MAC12]
[partnumber]=NXS 0004 0013
```

## Layout
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

## Print records
When labels are printed, lines are appended to the bottom of the label file. For this reason, the label file should not be read-only.

```
PRINTED:15/05/2019 08:00 25-78 V2.01
PRINTED:15/05/2019 08:14 79-294 V2.01
```


