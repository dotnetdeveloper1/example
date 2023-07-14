import { StateModelMapper } from "./StateModelMapper";

describe("StateModelMapper", () => {
    test("throws exception when empty constructor arguments are set", () => {
        let errorReturned = null;
        let mapper = null;

        try {
            mapper = new StateModelMapper(null as any);
        } catch (error) {
            errorReturned = error;
        }

        expect(mapper).toBeNull();
        expect(errorReturned).toBeDefined();
    });
});
