import { ApiModelMapper } from "./ApiModelMapper";

describe("ApiModelMapper", () => {
    test("throws exception when empty constructor arguments are set", () => {
        let errorReturned = null;
        let mapper = null;

        try {
            mapper = new ApiModelMapper({} as any, {} as any);
        } catch (error) {
            errorReturned = error;
        }

        expect(mapper).toBeNull();
        expect(errorReturned).toBeDefined();
    });
});
