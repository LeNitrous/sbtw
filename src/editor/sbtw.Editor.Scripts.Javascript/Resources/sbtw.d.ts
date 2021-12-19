/**
 * Creates a variable that is exposed to the editor
 * @public
 * @param name - The name of this variable
 * @param value - The value of this variable
 * @returns The set value of this variable
 * @remarks Variables set here are immutable in script but can be configured in editor
 */
declare function SetValue(name: string, value: any): any;

/**
 * Returns the value of a variable with a given name
 * @public
 * @param name - The name of the variable to get
 */
declare function GetValue(name: string): any;

/**
 * Sets a video on this script
 * @public
 * @param path - The path to the video file relative from the beatmap
 * @param offset - The offset in milliseconds
 */
declare function SetVideo(path: string, offset: number): void;

/**
 * Gets or creates a new group
 * @public
 * @param name - The name of the group
 * @returns A new group if it doesn't exist an existing group
 */
declare function GetGroup(name: string): ScriptElementGroup;

/**
 * Opens a file
 * @public
 * @param path - The path to the file relative from the project
 * @returns The file data as a byte array
 */
declare function OpenFile(path: string): number[];

/**
 * A container for grouping elements under a name
 * @public
 */
declare class ScriptElementGroup {
    /**
     * The name of the group
     */
    Name: string;

    /**
     * Creates a new storyboard sprite
     * @param path - The path to the image file relative from the beatmap
     * @param origin - The origin point of this sprite
     * @param initialPosition - The initial position of the sprite
     * @param layer - The layer where this sprite will be shown
     */
    CreateSprite(path: string, origin?: Anchor, initialPosition?: Vector2, layer?: Layer): ScriptedSprite;

    /**
     * Creates a new storyboard animation
     * @param path - The path to the image file relative from the beatmap
     * @param origin - The origin point of this animation
     * @param initialPosition - The initial position of this animation
     * @param frameCount - The number of frames this animation has
     * @param frameDelay - The time in milliseconds between frames
     * @param loopType - The kind of loop on how this animation will play
     * @param layer - The layer where this animation will be shown
     */
    CreateAnimation(path: string, origin?: Anchor, initialPosition?: Vector2, frameCount?: number, frameDelay?: number, loopType?: AnimationLoopType, layer?: Layer): ScriptedAnimation;

    /**
     * Creates a new storyboard sample
     * @param path - The path to the audio file relative from the beatmap
     * @param time - The time when this sample will be played
     * @param volume - A value between 0 - 100 determining the loudness of the sample
     * @param layer - The layer where this sample will play
     */
    CreateSample(path: string, time: number, volume?: number, layer?: Layer): void;
}

/**
 * Implemented by scriptable elements
 */
declare interface IScriptedElement {
    /**
     * The path to the resource relative from the beatmap
     */
    readonly Path: string,

    /**
     * The layer where this element will appear
     */
    readonly Layer: Layer,

    /**
     * The start time of this element in milliseconds
     */
    readonly StartTime: number,
}

/**
 * Implemented by scriptable elements that have a duration
 */
declare interface IScriptedElementWithDuration extends IScriptedElement {
    /**
     * The end time of this element in milliseconds
     */
    readonly EndTime: number,

    /**
     * The duration of this element in milliseconds
     */
    readonly Duration: number,
}

/**
 * Represents a storyboard sprite
 * @public
 */
declare class ScriptedSprite implements IScriptedElementWithDuration {
    readonly Path: string;
    readonly Layer: Layer;
    readonly StartTime: number;
    readonly EndTime: number;
    readonly Duration: number;

    /**
     * Moves this element over time
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
     * Moves this element along the X axis over time
     */
    MoveX(easing: Easing, startTime: number, endTime: number, start: number, end: number): void;
    MoveX(startTime: number, endTime: number, start: number, end: number): void;
    MoveX(time: number, value: number): void;

    /**
     * Moves this element along the Y axis over time
     */
    MoveY(easing: Easing, startTime: number, endTime: number, start: number, end: number): void;
    MoveY(startTime: number, endTime: number, start: number, end: number): void;
    MoveY(time: number, value: number): void;

    /**
     * Scales this element uniformly over time
     */
    Scale(easing: Easing, startTime: number, endTime: number, start: number, end: number): void;
    Scale(startTime: number, endTime: number, start: number, end: number): void;
    Scale(time: number, value: number): void;

    /**
     * Scales this element with a defined vector
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
     * Rotates this element in degrees over time
     */
    Rotate(easing: Easing, startTime: number, endTime: number, startDegrees: number, endDegrees: number): void;
    Rotate(startTime: number, endTime: number, startDegrees: number, endDegrees: number): void;
    Rotate(time: number, degrees: number): void;

    /**
     * Changes this element's opacity over time
     */
    Fade(easing: Easing, startTime: number, endTime: number, startOpacity: number, endOpacity: number): void;
    Fade(startTime: number, endTime: number, startOpacity: number, endOpacity: number): void;
    Fade(time: number, opacity: number): void;

    /**
     * Changes this element's color over time
     */
    Color(easing: Easing, startTime: number, endTime: number, start: Color, end: Color): void;
    Color(easing: Easing, startTime: number, endTime: number, start: Color, endRed: number, endBlue: number, endGreen: number): void;
    Color(easing: Easing, startTime: number, endTime: number, startRed: number, startBlue: number, startGreen: number, end: Color): void;
    Color(easing: Easing, startTime: number, endTime: number, startRed: number, startBlue: number, startGreen: number, endRed: number, endBlue: number, endGreen: number): void;
    Color(startTime: number, endTime: number, start: Color, end: Color): void;
    Color(startTime: number, endTime: number, start: Color, endRed: number, endBlue: number, endGreen: number): void;
    Color(startTime: number, endTime: number, startRed: number, startBlue: number, startGreen: number, end: Color): void;
    Color(startTime: number, endTime: number, startRed: number, startBlue: number, startGreen: number, endRed: number, endBlue: number, endGreen: number): void;
    Color(time: number, color: Color): void;
    Color(time: number, red: number, blue: number, green: number): void;
    Color(easing: Easing, startTime: number, endTime: number, startHex: string, endHex: string): void;
    Color(startTime: number, endTime: number, startHex: string, endHex: string): void;
    Color(time: number, hex: string): void;

    /**
     * Changes this element's color over time
     */
    ColorRGB(easing: Easing, startTime: number, endTime: number, startRed: number, startBlue: number, startGreen: number, endRed: number, endBlue: number, endGreen: number): void;
    ColorRGB(startTime: number, endTime: number, startRed: number, startBlue: number, startGreen: number, endRed: number, endBlue: number, endGreen: number): void;
    ColorRGB(time: number, red: number, blue: number, green: number): void;

    /**
     * Changes this element's color over time
     */
    ColorHSL(easing: Easing, startTime: number, endTime: number, startHue: number, startSaturation: number, startLightness: number, endHue: number, endSaturation: number, endLightness: number): void;
    ColorHSL(startTime: number, endTime: number, startHue: number, startSaturation: number, startLightness: number, endHue: number, endSaturation: number, endLightness: number): void;
    ColorHSL(time: number, hue: number, saturation: number, lightness: number): void;

    /**
     * Changes this element's color over time
     */
    ColorHSV(easing: Easing, startTime: number, endTime: number, startHue: number, startSaturation: number, startValue: number, endHue: number, endSaturation: number, endValue: number): void;
    ColorHSV(startTime: number, endTime: number, startHue: number, startSaturation: number, startValue: number, endHue: number, endSaturation: number, endValue: number): void;
    ColorHSV(time: number, hue: number, saturation: number, value: number): void;

    /**
     * Flips this element horizontally
     */
    FlipH(easing: Easing, startTime: number, endTime: number): void;
    FlipH(startTime: number, endTime: number): void;
    FlipH(time: number): void;

    /**
     * Flips this element vertically
     */
    FlipV(easing: Easing, startTime: number, endTime: number): void;
    FlipV(startTime: number, endTime: number): void;
    FlipV(time: number): void;

    /**
     * Applies an additive color blending to this element
     */
    Additive(easing: Easing, startTime: number, endTime: number): void;
    Additive(startTime: number, endTime: number): void;
    Additive(time: number): void;

    /**
     * Starts a loop group
     *
     * @remarks
     *
     * Subsequent commands have their start and end times offset implicity by this loop group's start time
     * and must call {@link EndGroup} to end capturing commands for this group.
     *
     * @throws {@link InvalidOperationException} When attempting to start a group when an existing group is currently active
     */
    StartLoopGroup(startTime: number, repeatCount: number): void;

    /**
     * Starts a trigger group
     *
     * @remarks
     *
     * Subsequent commands have their start and end times offset implicity by this trigger group's start time
     * and must call {@link EndGroup}to end capturing commands for this group.
     *
     * @throws {@link InvalidOperationException} When attempting to start a group when an existing group is currently active
     */
    StartTriggerGroup(triggerName: string, startTime: number, endTime: number, group?: number): void;

    /**
     * Ends the current group
     */
    EndGroup(): void;
}

/**
 * An exception thrown when an invalid operation has occured
 */
declare type InvalidOperationException = object;

/**
 * Represents a storyboard animation
 */
declare class ScriptedAnimation extends ScriptedSprite {
    /**
     * The number of frames this animation has
     */
    readonly FrameCount: number;

    /**
     * The delay between frames in milliseconds
     */
    readonly FrameDelay: number;

    /**
     * The type of looping this animation will perform
     */
    readonly LoopType: AnimationLoopType;
}

declare class ScriptedSample implements IScriptedElement {
    readonly Path: string;
    readonly Layer: Layer;
    readonly StartTime: number;
}

declare class ScriptedVideo implements IScriptedElement {
    readonly Path: string;
    readonly Layer: Layer;
    readonly StartTime: number;
}

declare class Vector2 {
    X: number;
    Y: number;

    static Zero: Vector2;
    static UnitX: Vector2;
    static UnitY: Vector2;

    constructor(x: number, y: number);
    constructor(value: number);

    static Abs(value: Vector2): Vector2;
    static Add(left: Vector2, right: Vector2): Vector2;
    static Clamp(value: Vector2, min: Vector2, max: Vector2): Vector2;
    static Distance(a: Vector2, b: Vector2): number;
    static DistanceSquared(a: Vector2, b: Vector2): number;
    static Divide(left: Vector2, right: Vector2): Vector2;
    static Divide(left: Vector2, divisor: number): Vector2;
    static Dot(left: Vector2, right: Vector2): number;
    static Lerp(a: Vector2, b: Vector2, amount: number): Vector2;
    static Max(a: Vector2, b: Vector2): Vector2;
    static Min(a: Vector2, b: Vector2): Vector2;
    static Multiply(left: Vector2, right: Vector2): Vector2;
    static Multiply(left: Vector2, right: number): Vector2;
    static Multiply(left: number, right: Vector2): Vector2;
    static Negate(value: Vector2): Vector2;
    static Normalize(value: Vector2): Vector2;
    static Reflect(vector: Vector2, normal: Vector2): Vector2;
    static SquareRoot(vector: Vector2): Vector2;
    static Subtract(left: Vector2, right: Vector2): Vector2;

    Equals(other: Vector2): boolean;
    Length(): number;
    LengthSquared(): number;
    ToString(): string;
}

/**
 * A struct that represents a 3-component color with red, blue, and green values
 */
declare class Color {
    /**
     * A number between 0 to 1 that represents the red component
     */
    R: number;

    /**
     * A number between 0 to 1 that represents the green component
     */
    G: number;

    /**
     * A number between 0 to 1 that represents the blue component
     */
    B: number;

    /**
     * Creates a new color
     * @constructor
     * @param r A number between 0 to 1 to set the red component of the color
     * @param g A number between 0 to 1 to set the green component of the color
     * @param b A number between 0 to 1 to set the blue component of the color
     */
    constructor(r: number, g: number, b: number);
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
 * The easing function to use in transformations. See https://easings.net for examples
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
