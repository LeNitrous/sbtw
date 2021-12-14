declare function SetVideo(path: string, offset: number): void;

declare function GetGroup(name: string): ScriptElementGroup;

declare class ScriptElementGroup {
    Name: string;
    CreateSprite(path: string, origin?: Anchor, initialPosition?: Vector2, layer?: Layer): ScriptedSprite;
    CreateAnimation(path: string, origin?: Anchor, initialPosition?: Vector2, frameCount?: number, frameDelay?: number, loopType?: AnimationLoopType, layer?: Layer): ScriptedAnimation;
    CreateSample(path: string, time: number, volume?: number, layer?: Layer): void;
}

declare class ScriptedSprite {
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
    MoveX(easing: Easing, startTime: number, endTime: number, start: number, end: number): void;
    MoveX(startTime: number, endTime: number, start: number, end: number): void;
    MoveX(time: number, value: number): void;
    MoveY(easing: Easing, startTime: number, endTime: number, start: number, end: number): void;
    MoveY(startTime: number, endTime: number, start: number, end: number): void;
    MoveY(time: number, value: number): void;
    Scale(easing: Easing, startTime: number, endTime: number, start: number, end: number): void;
    Scale(startTime: number, endTime: number, start: number, end: number): void;
    Scale(time: number, value: number): void;
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
    Rotate(easing: Easing, startTime: number, endTime: number, start: number, end: number): void;
    Rotate(startTime: number, endTime: number, start: number, end: number): void;
    Rotate(time: number, value: number): void;
    Fade(easing: Easing, startTime: number, endTime: number, start: number, end: number): void;
    Fade(startTime: number, endTime: number, start: number, end: number): void;
    Fade(time: number, value: number): void;
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
    ColorRGB(easing: Easing, startTime: number, endTime: number, startRed: number, startBlue: number, startGreen: number, endRed: number, endBlue: number, endGreen: number): void;
    ColorRGB(startTime: number, endTime: number, startRed: number, startBlue: number, startGreen: number, endRed: number, endBlue: number, endGreen: number): void;
    ColorRGB(time: number, red: number, blue: number, green: number): void;
    ColorHSL(easing: Easing, startTime: number, endTime: number, startHue: number, startSaturation: number, startLightness: number, endHue: number, endSaturation: number, endLightness: number): void;
    ColorHSL(startTime: number, endTime: number, startHue: number, startSaturation: number, startLightness: number, endHue: number, endSaturation: number, endLightness: number): void;
    ColorHSL(time: number, hue: number, saturation: number, lightness: number): void;
    ColorHSV(easing: Easing, startTime: number, endTime: number, startHue: number, startSaturation: number, startValue: number, endHue: number, endSaturation: number, endValue: number): void;
    ColorHSV(startTime: number, endTime: number, startHue: number, startSaturation: number, startValue: number, endHue: number, endSaturation: number, endValue: number): void;
    ColorHSV(time: number, hue: number, saturation: number, value: number): void;
    FlipH(easing: Easing, startTime: number, endTime: number): void;
    FlipH(startTime: number, endTime: number): void;
    FlipH(time: number): void;
    FlipV(easing: Easing, startTime: number, endTime: number): void;
    FlipV(startTime: number, endTime: number): void;
    FlipV(time: number): void;
    Additive(easing: Easing, startTime: number, endTime: number): void;
    Additive(startTime: number, endTime: number): void;
    Additive(time: number): void;
    StartLoopGroup(startTime: number, repeatCount: number): void;
    StartTriggerGroup(triggerName: string, startTime: number, endTime: number, group?: number): void;
    EndGroup(): void;
}

declare class ScriptedAnimation extends ScriptedSprite {}

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

declare class Color {
    R: number;
    G: number;
    B: number;

    constructor(r: number, g: number, b: number);
}

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

declare enum Layer {
    Background,
    Foreground,
    Passing,
    Failing,
    Overlay
}

declare enum AnimationLoopType {
    LoopForever,
    LoopOnce
}
