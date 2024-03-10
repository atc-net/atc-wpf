# SvgImage

The `SvgImage` control is a control that can render a SVG drawing as image.

## Example for SvgImage usages

```xml
<atc:SvgImage
    ControlSizeType="ControlSizeType.ContentToSizeStretch"
    Source="/Atc.Wpf.Sample;component/Assets/eggeaster.svg" />
```

| Enum ControlSizeType   | Description |
|------------------------|-------------|
| None                   | The image is not scaled. The image location is translated so the top left corner of the image bounding box is moved to the top left corner of the image control. |
| ContentToSizeNoStretch | The image is scaled to fit the control without any stretching. Either X or Y direction will be scaled to fill the entire width or height. |
| ContentToSizeStretch   | The image will be stretched to fill the entire width and height. |
| SizeToContent          | The control will be resized to fit the un-scaled image. If the image is larger than the maximum size for the control, the control is set to maximum size and the image is scaled. |

## Properties

| Property            | Type        | Description                                                                       |
|---------------------|-------------------------------------------------------------------------------------------------|
| Background          | Brush       | Background brush                                                                  |
| ControlSizeType     | ControlSizeType | How to stretched/resize/scale the drawing                                     |
| Source              | string      | The relative URL - For assembly component, rember to set "Build action=Resource"  |
| FileSource          | [TODO]      | [TODO] |
| ImageSource         | [TODO]      | [TODO] |
| UseAnimations       | [TODO]      | [TODO] |
| OverrideColor       | [TODO]      | [TODO] |
| OverrideStrokeColor | [TODO]      | [TODO] |
| OverrideStrokeWidth | [TODO]      | [TODO] |
| CustomBrushes       | [TODO]      | [TODO] |
| ExternalFileLoader  | [TODO]      | [TODO] |

## Methods

| Method              | Description                                                                               |
|---------------------|-------------------------------------------------------------------------------------------|
| SetImage            | Alternativ to `Source`, a method that by `string`, `Stream`, `Drawing` render the content |

## Abbreviations

SVG - Scalable Vector Graphics

### W3C Specification for SVG

[W3C - Scalable Vector Graphics (SVG) Tiny 1.2 Specification](https://www.w3.org/TR/SVGTiny12)

| Section | Table of Contents |
| ------- | ----------------- |
| 1  | [Introduction](https://www.w3.org/TR/SVGTiny12/intro.html)
| 2  | [Concepts](https://www.w3.org/TR/SVGTiny12/concepts.html)
| 3  | [Rendering Model](https://www.w3.org/TR/SVGTiny12/render.html)
| 4  | [Basic Data Types](https://www.w3.org/TR/SVGTiny12/types.html)
| 5  | [Document Structure](https://www.w3.org/TR/SVGTiny12/struct.html)
| 6  | [Styling](https://www.w3.org/TR/SVGTiny12/styling.html)
| 7  | [Coordinate Systems, Transformations and Units](https://www.w3.org/TR/SVGTiny12/coords.html)
| 8  | [Paths](http://www.w3.org/TR/SVGTiny12/paths.html)
| 9  | [Basic Shapes](http://www.w3.org/TR/SVGTiny12/shapes.html)
| 10 | [Text](https://www.w3.org/TR/SVGTiny12/text.html)
| 11 | [Painting: Filling, Stroking, Colors and Paint Servers](https://www.w3.org/TR/SVGTiny12/painting.html)
| 12 | [Multimedia](https://www.w3.org/TR/SVGTiny12/multimedia.html)
| 13 | [Interactivity](https://www.w3.org/TR/SVGTiny12/interact.html)
| 14 | [Linking](https://www.w3.org/TR/SVGTiny12/linking.html)
| 15 | [Scripting](https://www.w3.org/TR/SVGTiny12/script.html)
| 16 | [Animation](https://www.w3.org/TR/SVGTiny12/animate.html)
| 17 | [Fonts](https://www.w3.org/TR/SVGTiny12/fonts.html)
| 18 | [Metadata](https://www.w3.org/TR/SVGTiny12/metadata.html)
