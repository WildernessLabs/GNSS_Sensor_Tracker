<img src="Design/banner.jpg" style="margin-bottom:10px" />

# GNSS_Sensor_Tracker

The GNSS Sensor Tracker is an open-source, sensor-rich, GNSS/GPS tracking board intended to provide a base design accelerator for custom Meadow-based GNSS tracking solutions.

## Contents
* [Purchasing or Building](#purchasing-or-building)
* [Getting Started](#getting-started)
* [Hardware Specifications](#hardware-specifications)
* [Pinout Diagram](#pinout-diagram)
  * [GNSS Tracker v1.e](#project-lab-v1e)
* [Additional Samples](#additional-samples)

## Purchasing or Building

<table width="100%">
    <tr>
        <td>
            <img src="Design/gnss-tracker-store.jpg" />
        </td>
        <td>
            <img src="Design/gnss-tracker-pcb.jpg" /> 
        </td>
    </tr>
    <tr>
        <td>
            You can get a Gnss Sensor Tracker fully assembled from the <a href="https://store.wildernesslabs.co/collections/frontpage/products/project-lab-board">Wilderness Labs store</a>.
        </td>
        <td> 
            It's also designed so that it can be assembled at home for the adventurous. All design files can be found in the <a href="Source/Hardware">Hardware Design</a> folder.
        </td>
    </tr>
</table>

## Getting Started

To simplify the way to use this Meadow-powered reference IoT product, we've created a NuGet package that instantiates and encapsulates the onboard hardware into a `GnssTracker` class.

1. Add the ProjectLab Nuget package your project: 
    - `dotnet add package Meadow.GnssTracker`, or
    - [Meadow.GnssTracker Nuget Package](https://www.nuget.org/packages/Meadow.GnssTracker)
    - [Explore in Fuget.org](https://www.fuget.org/packages/Meadow.ProjectLab/0.1.0/lib/netstandard2.1/ProjectLab.dll/Meadow.Devices/ProjectLab)

2. Instantiate the `IGnssTrackerHardware` object:  
```csharp
public class MeadowApp : App<F7FeatherV2>
{
    IGnssTrackerHardware gnssTracker;

    public override Task Initialize()
    {
        gnssTracker = GnssTracker.Create();
        ...
```

3. To Access the `GNSS Tracker` onboard peripherals (Display, for example):
```csharp
    if (gnssTracker.Display is { } display)
    {
        microGraphics = new MicroGraphics(display);
        microGraphics.Clear();
        microGraphics.DrawText(10, 10, "Hello World");
        microGraphics.Show();
    }
```

## Hardware Specifications

<img src="Design/gnss-tracker-specs.jpg" style="margin-top:10px;margin-bottom:10px" />

<table>
    <tr>
        <th>Onboard Peripherals</th>
    </tr>
    <tr>
        <td><strong>6V Solar Input</strong></td>
    </tr>
    <tr>
        <td><strong>SSD1680 122x250 Adafruit E-Paper Display</strong></td>
    </tr>
    <tr>
        <td><strong>NEO-M8 GNSS/GPS</strong></td>
    </tr>
    <tr>
        <td><strong>GNSS/GPS Antenna</strong></td>
    </tr>
    <tr>
        <td><strong>Solar Power/Battery Charging Add-on Module</strong></td>
    </tr>
    <tr>
        <td><strong>Meadow F7 Core-Compute Module (CCM)</strong></td>
    </tr>
    <tr>
        <td><strong>USB-C, Boot, Reset Add-on Module</strong></td>
    </tr>
    <tr>
        <td><strong>3.7V Lipo Rechargeable Battery</strong></td>
    </tr>
</table>

You can find the schematics and other design files in the [Hardware folder](Source/Hardware).

## Video Stream Design Series

This board was designed while streaming live, you can watch the videos [here](https://www.youtube.com/watch?v=L4MavM8ilkg&list=PLoP9Fu9zn7qY4rkFJjHBhnpI8mPlw8RfS).

<img src="Design/gnss-playlist.png" href="https://www.youtube.com/watch?v=L4MavM8ilkg&list=PLoP9Fu9zn7qY4rkFJjHBhnpI8mPlw8RfS" style="margin-top:10px;margin-bottom:10px" />


### [Industrial Design](Hardware/Enclosure)

The enclosure was designed in Autodesk Fusion 360 the source file can be found [here](Source/Industrial_Design/GNSS_Tracker_Enclosure.f3d).

![](Design/gnss-tracker-enclosure.png)

STL files for printing can be found in the [Industrial Design folder](Source/Industrial_Design).