export function safeParseInt(text: string): number {
    try {
        return parseInt(text, 10);
    } catch {
        return 0;
    }
}

export function isNumber(text: string): boolean {
    try {
        parseInt(text, 10);
        return true;
    } catch {
        return false;
    }
}
