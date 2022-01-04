/**
 * Creates a variable that is exposed to the editor.
 * @param name - The name of this variable.
 * @param value - The value of this variable.
 * @returns The set value of this variable.
 * @remarks Variables set here are immutable in script but can be configured in editor.
 */
declare function SetValue(name: string, value: any): any;

/**
 * Returns the value of a variable with a given name.
 * @param name - The name of the variable to get.
 */
declare function GetValue(name: string): any;

/**
 * Sets a video on this script.
 * @param path - The path to the video file relative from the beatmap.
 * @param offset - The offset in milliseconds.
 */
declare function SetVideo(path: string, offset: number): void;

/**
 * Gets or creates a new group.
 * @param name - The name of the group.
 * @returns A new group if it doesn't exist an existing group.
 */
declare function GetGroup(name: string): ScriptElementGroup;

/**
 * Opens a file.
 * @param path - The path to the file relative from the project.
 * @returns The file data as a byte array or null if the file is not found.
 */
declare function OpenFile(path: string): number[];

/**
 * Generates an asset.
 * @param path - The directory where the asset will be generated.
 * @param asset - The asset to be generated.
 * @returns The path to the generated asset.
 */
declare function GetAsset(path: string, asset: Asset): string;

/**
 * Logs a message to the output.
 * @param message - The message to be logged.
 */
declare function Log(message: string): void;

/**
 * A container for grouping elements under a name.
 */
declare class ScriptElementGroup {
    /**
     * The name of the group.
     */
    Name: string;

    /**
     * Creates a new storyboard sprite.
     * @param path - The path to the image file relative from the beatmap.
     * @param origin - The origin point of this sprite.
     * @param initialPosition - The initial position of the sprite.
     * @param layer - The layer where this sprite will be shown.
     */
    CreateSprite(path: string, origin?: Anchor, initialPosition?: Vector2, layer?: Layer): ScriptedSprite;

    /**
     * Creates a new storyboard animation.
     * @param path - The path to the image file relative from the beatmap.
     * @param origin - The origin point of this animation.
     * @param initialPosition - The initial position of this animation.
     * @param frameCount - The number of frames this animation has.
     * @param frameDelay - The time in milliseconds between frames.
     * @param loopType - The kind of loop on how this animation will play.
     * @param layer - The layer where this animation will be shown.
     */
    CreateAnimation(path: string, origin?: Anchor, initialPosition?: Vector2, frameCount?: number, frameDelay?: number, loopType?: AnimationLoopType, layer?: Layer): ScriptedAnimation;

    /**
     * Creates a new storyboard sample.
     * @param path - The path to the audio file relative from the beatmap.
     * @param time - The time when this sample will be played.
     * @param volume - A value between 0 - 100 determining the loudness of the sample.
     * @param layer - The layer where this sample will play.
     */
    CreateSample(path: string, time: number, volume?: number, layer?: Layer): void;
}

/**
 * Implemented by scriptable elements.
 */
declare interface IScriptedElement {
    /**
     * The path to the resource relative from the beatmap.
     */
    readonly Path: string,

    /**
     * The layer where this element will appear.
     */
    readonly Layer: Layer,

    /**
     * The start time of this element in milliseconds.
     */
    readonly StartTime: number,
}

/**
 * Implemented by scriptable elements that have a duration.
 */
declare interface IScriptedElementWithDuration extends IScriptedElement {
    /**
     * The end time of this element in milliseconds.
     */
    readonly EndTime: number,

    /**
     * The duration of this element in milliseconds.
     */
    readonly Duration: number,
}

/**
 * Represents a storyboard sprite.
 */
declare class ScriptedSprite implements IScriptedElementWithDuration {
    readonly Path: string;
    readonly Layer: Layer;
    readonly StartTime: number;
    readonly EndTime: number;
    readonly Duration: number;

    /**
     * Moves this element over time.
     */
    Move(easing: Easing, startTime: number, endTime: number, startPosition: Vector2, endPosition: Vector2): void;
    Move(easing: Easing, startTime: number, endTime: number, startPosition: Vector2, endX: number, endY: number): void;
    Move(easing: Easing, startTime: number, endTime: number, startX: number, startY: number, endPosition: Vector2): void;
    Move(easing: Easing, startTime: number, endTime: number, startX: number, startY: number, endX: number, endY: number): void;
    Move(startTime: number, endTime: number, startPosition: Vector2, endPosition: Vector2): void;
    Move(startTime: number, endTime: number, startPosition: Vector2, endX: number, endY: number): void;
    Move(startTime: number, endTime: number, startX: number, startY: number, endPosition: Vector2): void;
    Move(startTime: number, endTime: number, startX: number, startY: number, endX: number, endY: number): void;
    Move(time: number, position: Vector2): void;
    Move(time: number, x: number, y: number): void;

    /**
     * Moves this element along the X axis over time.
     */
    MoveX(easing: Easing, startTime: number, endTime: number, start: number, end: number): void;
    MoveX(startTime: number, endTime: number, start: number, end: number): void;
    MoveX(time: number, value: number): void;

    /**
     * Moves this element along the Y axis over time.
     */
    MoveY(easing: Easing, startTime: number, endTime: number, start: number, end: number): void;
    MoveY(startTime: number, endTime: number, start: number, end: number): void;
    MoveY(time: number, value: number): void;

    /**
     * Scales this element uniformly over time.
     */
    Scale(easing: Easing, startTime: number, endTime: number, start: number, end: number): void;
    Scale(startTime: number, endTime: number, start: number, end: number): void;
    Scale(time: number, value: number): void;

    /**
     * Scales this element with a defined vector.
     */
    ScaleVec(easing: Easing, startTime: number, endTime: number, startPosition: Vector2, endPosition: Vector2): void;
    ScaleVec(easing: Easing, startTime: number, endTime: number, startPosition: Vector2, endX: number, endY: number): void;
    ScaleVec(easing: Easing, startTime: number, endTime: number, startX: number, startY: number, endPosition: Vector2): void;
    ScaleVec(easing: Easing, startTime: number, endTime: number, startX: number, startY: number, endX: number, endY: number): void;
    ScaleVec(startTime: number, endTime: number, startPosition: Vector2, endPosition: Vector2): void;
    ScaleVec(startTime: number, endTime: number, startPosition: Vector2, endX: number, endY: number): void;
    ScaleVec(startTime: number, endTime: number, startX: number, startY: number, endPosition: Vector2): void;
    ScaleVec(startTime: number, endTime: number, startX: number, startY: number, endX: number, endY: number): void;
    ScaleVec(time: number, position: Vector2): void;
    ScaleVec(time: number, x: number, y: number): void;

    /**
     * Rotates this element in degrees over time.
     */
    Rotate(easing: Easing, startTime: number, endTime: number, startDegrees: number, endDegrees: number): void;
    Rotate(startTime: number, endTime: number, startDegrees: number, endDegrees: number): void;
    Rotate(time: number, degrees: number): void;

    /**
     * Changes this element's opacity over time.
     */
    Fade(easing: Easing, startTime: number, endTime: number, startOpacity: number, endOpacity: number): void;
    Fade(startTime: number, endTime: number, startOpacity: number, endOpacity: number): void;
    Fade(time: number, opacity: number): void;

    /**
     * Changes this element's color over time.
     */
    Color(easing: Easing, startTime: number, endTime: number, start: Colour4, end: Colour4): void;
    Color(easing: Easing, startTime: number, endTime: number, start: Colour4, endRed: number, endBlue: number, endGreen: number): void;
    Color(easing: Easing, startTime: number, endTime: number, startRed: number, startBlue: number, startGreen: number, end: Colour4): void;
    Color(easing: Easing, startTime: number, endTime: number, startRed: number, startBlue: number, startGreen: number, endRed: number, endBlue: number, endGreen: number): void;
    Color(startTime: number, endTime: number, start: Colour4, end: Colour4): void;
    Color(startTime: number, endTime: number, start: Colour4, endRed: number, endBlue: number, endGreen: number): void;
    Color(startTime: number, endTime: number, startRed: number, startBlue: number, startGreen: number, end: Colour4): void;
    Color(startTime: number, endTime: number, startRed: number, startBlue: number, startGreen: number, endRed: number, endBlue: number, endGreen: number): void;
    Color(time: number, color: Colour4): void;
    Color(time: number, red: number, blue: number, green: number): void;
    Color(easing: Easing, startTime: number, endTime: number, startHex: string, endHex: string): void;
    Color(startTime: number, endTime: number, startHex: string, endHex: string): void;
    Color(time: number, hex: string): void;

    /**
     * Changes this element's color over time.
     */
    ColorRGB(easing: Easing, startTime: number, endTime: number, startRed: number, startBlue: number, startGreen: number, endRed: number, endBlue: number, endGreen: number): void;
    ColorRGB(startTime: number, endTime: number, startRed: number, startBlue: number, startGreen: number, endRed: number, endBlue: number, endGreen: number): void;
    ColorRGB(time: number, red: number, blue: number, green: number): void;

    /**
     * Changes this element's color over time.
     */
    ColorHSL(easing: Easing, startTime: number, endTime: number, startHue: number, startSaturation: number, startLightness: number, endHue: number, endSaturation: number, endLightness: number): void;
    ColorHSL(startTime: number, endTime: number, startHue: number, startSaturation: number, startLightness: number, endHue: number, endSaturation: number, endLightness: number): void;
    ColorHSL(time: number, hue: number, saturation: number, lightness: number): void;

    /**
     * Changes this element's color over time.
     */
    ColorHSV(easing: Easing, startTime: number, endTime: number, startHue: number, startSaturation: number, startValue: number, endHue: number, endSaturation: number, endValue: number): void;
    ColorHSV(startTime: number, endTime: number, startHue: number, startSaturation: number, startValue: number, endHue: number, endSaturation: number, endValue: number): void;
    ColorHSV(time: number, hue: number, saturation: number, value: number): void;

    /**
     * Flips this element horizontally.
     */
    FlipH(easing: Easing, startTime: number, endTime: number): void;
    FlipH(startTime: number, endTime: number): void;
    FlipH(time: number): void;

    /**
     * Flips this element vertically.
     */
    FlipV(easing: Easing, startTime: number, endTime: number): void;
    FlipV(startTime: number, endTime: number): void;
    FlipV(time: number): void;

    /**
     * Applies an additive color blending to this element.
     */
    Additive(easing: Easing, startTime: number, endTime: number): void;
    Additive(startTime: number, endTime: number): void;
    Additive(time: number): void;

    /**
     * Starts a loop group.
     *
     * @remarks
     *
     * Subsequent commands have their start and end times offset implicity by this loop group's start time.
     * and must call {@link EndGroup} to end capturing commands for this group.
     *
     * @throws {@link InvalidOperationException} When attempting to start a group when an existing group is currently active.
     */
    StartLoopGroup(startTime: number, repeatCount: number): void;

    /**
     * Starts a trigger group.
     *
     * @remarks
     *
     * Subsequent commands have their start and end times offset implicity by this trigger group's start time
     * and must call {@link EndGroup}to end capturing commands for this group.
     *
     * @throws {@link InvalidOperationException} When attempting to start a group when an existing group is currently active.
     */
    StartTriggerGroup(triggerName: string, startTime: number, endTime: number, group?: number): void;

    /**
     * Ends the current group
     */
    EndGroup(): void;
}

/**
 * An exception thrown when an invalid operation has occured.
 */
declare type InvalidOperationException = Error;

/**
 * Represents the playable beatmap.
 *
 * @remarks
 * See {@link https://github.com/ppy/osu/blob/master/osu.Game/Beatmaps/IBeatmap.cs} for the implmenetation.
 */
declare var Beatmap: any;

/**
 * Represents a waveform. of the current beatmap.
 *
 * @remarks
 * See {@link https://github.com/ppy/osu-framework/blob/master/osu.Framework/Audio/Track/Waveform.cs} for the implementation.
 */
declare var Waveform: any;

/**
 * Represents a storyboard animation.
 */
declare class ScriptedAnimation extends ScriptedSprite {
    /**
     * The number of frames this animation has.
     */
    readonly FrameCount: number;

    /**
     * The delay between frames in milliseconds.
     */
    readonly FrameDelay: number;

    /**
     * The type of looping this animation will perform.
     */
    readonly LoopType: AnimationLoopType;
}

/**
 * Represents a storyboard sample.
 */
declare class ScriptedSample implements IScriptedElement {
    readonly Path: string;
    readonly Layer: Layer;
    readonly StartTime: number;
}

/**
 * Represents a storyboard video.
 */
declare class ScriptedVideo implements IScriptedElement {
    readonly Path: string;
    readonly Layer: Layer;
    readonly StartTime: number;
}

/**
 * A generatable asset.
 */
declare class Asset { }

/**
 * A rectangle.
 */
declare class Rectangle extends Asset {
    /**
     * Creates a rectangle.
     * @param name - The unique name for this rectangle.
     */
    constructor(name: string);
}

/**
 * Text shown on the storyboard.
 */
declare class Text extends Asset {
    /**
     * Creates text.
     * @param text - The text to be displayed.
     * @param font - The font configuration to be used.
     */
    constructor(text: string, font: FontConfiguration);
}

declare class FontConfiguration {
    /**
     * The path to the font file relative to the project.
     */
    Path: string;

    /**
     * The font's family name.
     */
    Name: string;

    /**
     * The font's size.
     */
    Size: number;

    /**
     * Creates a new font configuration.
     * @param path - The path to the font file relative to the project.
     * @param name - The font's family name.
     * @param size - The font's size.
     */
    constructor(path: string, name: string, size: number);
}

/**
 * Represesnts a 2D point.
 *
 * @remarks
 * See {@link https://opentk.net/api/OpenTK.Mathematics.Vector2.html} for documentation.
 */
declare class Vector2 {
    X: number;
    Y: number;
    Length: number;
    LengthSquared: number;
    PerpendicularRight: Vector2;
    PerpendicularLeft: Vector2;

    static Zero: Vector2;
    static One: Vector2;
    static UnitX: Vector2;
    static UnitY: Vector2;

    constructor(x: number, y: number);
    constructor(value: number);

    static Add(left: Vector2, right: Vector2): Vector2;
    static BaryCentric(a: Vector2, b: Vector2, c: Vector2, u: number, v: number): Vector2;
    static Clamp(value: Vector2, min: Vector2, max: Vector2): Vector2;
    static ComponentMin(left: Vector2, right: Vector2): Vector2;
    static ComponentMax(left: Vector2, right: Vector2): Vector2;
    static Distance(a: Vector2, b: Vector2): number;
    static DistanceSquared(a: Vector2, b: Vector2): number;
    static Divide(left: Vector2, right: Vector2): Vector2;
    static Divide(left: Vector2, divisor: number): Vector2;
    static Dot(left: Vector2, right: Vector2): number;
    static Lerp(a: Vector2, b: Vector2, amount: number): Vector2;
    static MagnitudeMax(a: Vector2, b: Vector2): Vector2;
    static MagnitudeMin(a: Vector2, b: Vector2): Vector2;
    static Multiply(left: Vector2, right: Vector2): Vector2;
    static Multiply(left: Vector2, right: number): Vector2;
    static Normalize(value: Vector2): Vector2;
    static NormalizeFast(value: Vector2): Vector2;
    static Subtract(left: Vector2, right: Vector2): Vector2;
    static PerpDot(left: Vector2, right: Vector2): number;

    Equals(other: Vector2): boolean;
    Normalize(): void;
    Normalized(): Vector2;
    NormalizeFast(): void;
    ToString(): string;
}

/**
 * A struct that represents a 4-component color with red, blue, green, and alpha values.
 */
declare class Colour4 {
    /**
     * A number between 0 to 1 that represents the red component.
     */
    R: number;

    /**
     * A number between 0 to 1 that represents the green component.
     */
    G: number;

    /**
     * A number between 0 to 1 that represents the blue component.
     */
    B: number;

    /**
     * A number between 0 to 1 that represents the alpha component.
     */
    A: number;

    static Transparent: Colour4;
    static AliceBlue: Colour4;
    static AntiqueWhite: Colour4;
    static Aqua: Colour4;
    static Aquamarine: Colour4;
    static Azure: Colour4;
    static Beige: Colour4;
    static Bisque: Colour4;
    static Black: Colour4;
    static BlanchedAlmond: Colour4;
    static Blue: Colour4;
    static BlueViolet: Colour4;
    static Brown: Colour4;
    static BurlyWood: Colour4;
    static CadetBlue: Colour4;
    static Chartreuse: Colour4;
    static Chocolate: Colour4;
    static Coral: Colour4;
    static CornflowerBlue: Colour4;
    static Cornsilk: Colour4;
    static Crimson: Colour4;
    static Cyan: Colour4;
    static DarkBlue: Colour4;
    static DarkCyan: Colour4;
    static DarkGoldenrod: Colour4;
    static DarkGray: Colour4;
    static DarkGreen: Colour4;
    static DarkKhaki: Colour4;
    static DarkMagenta: Colour4;
    static DarkOliveGreen: Colour4;
    static DarkOrange: Colour4;
    static DarkOrchid: Colour4;
    static DarkRed: Colour4;
    static DarkSalmon: Colour4;
    static DarkSeaGreen: Colour4;
    static DarkSlateBlue: Colour4;
    static DarkSlateGray: Colour4;
    static DarkTurquoise: Colour4;
    static DarkViolet: Colour4;
    static DeepPink: Colour4;
    static DeepSkyBlue: Colour4;
    static DimGray: Colour4;
    static DodgerBlue: Colour4;
    static Firebrick: Colour4;
    static FloralWhite: Colour4;
    static ForestGreen: Colour4;
    static Fuchsia: Colour4;
    static Gainsboro: Colour4;
    static GhostWhite: Colour4;
    static Gold: Colour4;
    static Goldenrod: Colour4;
    static Gray: Colour4;
    static Green: Colour4;
    static GreenYellow: Colour4;
    static Honeydew: Colour4;
    static HotPink: Colour4;
    static IndianRed: Colour4;
    static Indigo: Colour4;
    static Ivory: Colour4;
    static Khaki: Colour4;
    static Lavender: Colour4;
    static LavenderBlush: Colour4;
    static LawnGreen: Colour4;
    static LemonChiffon: Colour4;
    static LightBlue: Colour4;
    static LightCoral: Colour4;
    static LightCyan: Colour4;
    static LightGoldenrodYellow: Colour4;
    static LightGreen: Colour4;
    static LightGray: Colour4;
    static LightPink: Colour4;
    static LightSalmon: Colour4;
    static LightSeaGreen: Colour4;
    static LightSkyBlue: Colour4;
    static LightSlateGray: Colour4;
    static LightSteelBlue: Colour4;
    static LightYellow: Colour4;
    static Lime: Colour4;
    static LimeGreen: Colour4;
    static Linen: Colour4;
    static Magenta: Colour4;
    static Maroon: Colour4;
    static MediumAquamarine: Colour4;
    static MediumBlue: Colour4;
    static MediumOrchid: Colour4;
    static MediumPurple: Colour4;
    static MediumSeaGreen: Colour4;
    static MediumSlateBlue: Colour4;
    static MediumSpringGreen: Colour4;
    static MediumTurquoise: Colour4;
    static MediumVioletRed: Colour4;
    static MidnightBlue: Colour4;
    static MintCream: Colour4;
    static MistyRose: Colour4;
    static Moccasin: Colour4;
    static NavajoWhite: Colour4;
    static Navy: Colour4;
    static OldLace: Colour4;
    static Olive: Colour4;
    static OliveDrab: Colour4;
    static Orange: Colour4;
    static OrangeRed: Colour4;
    static Orchid: Colour4;
    static PaleGoldenrod: Colour4;
    static PaleGreen: Colour4;
    static PaleTurquoise: Colour4;
    static PaleVioletRed: Colour4;
    static PapayaWhip: Colour4;
    static PeachPuff: Colour4;
    static Peru: Colour4;
    static Pink: Colour4;
    static Plum: Colour4;
    static PowderBlue: Colour4;
    static Purple: Colour4;
    static Red: Colour4;
    static RosyBrown: Colour4;
    static RoyalBlue: Colour4;
    static SaddleBrown: Colour4;
    static Salmon: Colour4;
    static SandyBrown: Colour4;
    static SeaGreen: Colour4;
    static SeaShell: Colour4;
    static Sienna: Colour4;
    static Silver: Colour4;
    static SkyBlue: Colour4;
    static SlateBlue: Colour4;
    static SlateGray: Colour4;
    static Snow: Colour4;
    static SpringGreen: Colour4;
    static SteelBlue: Colour4;
    static Tan: Colour4;
    static Teal: Colour4;
    static Thistle: Colour4;
    static Tomato: Colour4;
    static Turquoise: Colour4;
    static Violet: Colour4;
    static Wheat: Colour4;
    static White: Colour4;
    static WhiteSmoke: Colour4;
    static Yellow: Colour4;
    static YellowGreen: Colour4;

    /**
     * Creates a color from a well-formatted hex string.
     * @param hex - The hex string to create a color from.
     *
     * @remarks
     *
     * Valid formats of a hex string are given as follows:
     * - RGB
     * - #RGB
     * - RGBA
     * - #RGBA
     * - RRGGBB
     * - #RRGGBB
     * - RRGGBBAA
     * - #RRGGBBAA
     */
    static FromHex(hex: string): Colour4;

    /**
     * Creates a color from HSV values.
     * @param hue - A number between 0 to 1 to set the hue component.
     * @param saturation - A number between 0 to 1 to set the saturation component.
     * @param value - A number between 0 to 1 to set the value component.
     * @param alpha - A number between 0 to 1 to set the alpha component.
     */
    static FromHSV(hue: number, saturation: number, value: number, alpha: number): Colour4;

    /**
     * Creates a color from HSL values.
     * @param hue - A number between 0 to 1 to set the hue component.
     * @param saturation - A number between 0 to 1 to set the saturation component.
     * @param lightness - A number between 0 to 1 to set the lightness component.
     * @param alpha - A number between 0 to 1 to set the alpha component.
     */
    static FromHSL(hue: number, saturation: number, lightness: number, alpha: number): Colour4;

    /**
     * Creates a new color.
     * @constructor
     * @param r - A number between 0 to 1 to set the red component of the color.
     * @param g - A number between 0 to 1 to set the green component of the color.
     * @param b - A number between 0 to 1 to set the blue component of the color.
     * @param a - A number between 0 to 1 to set the alpha component of the color.
     */
    constructor(r: number, g: number, b: number, a: number);

    /**
     * Lightens a color by a given amount.
     * @param amount - The amount to lighten this color.
     */
    Lighten(amount: number): Colour4;

    /**
     * Darkens a color by a given amount.
     * @param amount - The amount to darken this color.
     */
    Darken(amount: number): Colour4;

    /**
     * Returns a hex string representation of this color.
     * @param alwaysOutputAlpha - When true, the alpha component is included in the hex string.
     */
    ToHex(alwaysOutputAlpha?: boolean): string;
    ToString(): string;
}

/**
 * A point in a quadrilateral where sprites or animations may originate their transformations
 */
declare enum Anchor {
    TopLeft,
    TopCentre,
    TopRight,
    CentreLeft,
    Centre,
    CentreRight,
    BottomLeft,
    BottomCentre,
    BottomRight
}

/**
 * The easing function to use in transformations.
 *
 * @remarks
 * See {@link https://easings.net} for examples
 */
declare enum Easing {
    None,
    Out,
    In,
    InQuad,
    OutQuad,
    InOutQuad,
    InCubic,
    OutCubic,
    InOutCubic,
    InQuart,
    OutQuart,
    InOutQuart,
    InQuint,
    OutQuint,
    InOutQuint,
    InSine,
    OutSine,
    InOutSine,
    InExpo,
    OutExpo,
    InOutExpo,
    InCirc,
    OutCirc,
    InOutCirc,
    InElastic,
    OutElastic,
    OutElasticHalf,
    OutElasticQuarter,
    InOutElastic,
    InBack,
    OutBack,
    InOutBack,
    InBounce,
    OutBounce,
    InOutBounce
}

/**
 * The layer where storyboard elements may appear
 */
declare enum Layer {
    /** The backmost layer */
    Background,

    /** The layer shown when the player is in a failing state */
    Failing,

    /** The layer shown when the player is in a passing state */
    Passing,

    /** The frontmost layer */
    Foreground,

    /** The layer shown above of gameplay elements */
    Overlay
}

/**
 * Determines how an animation may loop
 */
declare enum AnimationLoopType {
    /** The animation loops indefinitely */
    LoopForever,

    /** The animation plays once */
    LoopOnce
}
