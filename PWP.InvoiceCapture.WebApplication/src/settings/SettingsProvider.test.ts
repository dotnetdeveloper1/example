import { settings } from "./SettingsProvider";

describe("SettingsProvider", () => {
    test("settings contain values from test environment config", () => {
        expect(settings.environment).toBe("test");
        expect(settings.version).toContain("test");
    });
});
