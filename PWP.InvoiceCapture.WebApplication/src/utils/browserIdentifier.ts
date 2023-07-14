export class BrowserIdentifier {
    public static IsSafari(): boolean {
        return /^((?!chrome|android).)*safari/i.test(navigator.userAgent);
    }

    public static IsEdge(): boolean {
        return /Edge\/\d./i.test(navigator.userAgent);
    }
}
