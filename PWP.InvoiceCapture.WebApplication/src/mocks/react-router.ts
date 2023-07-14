import * as router from "react-router-dom";

jest.mock("react-router-dom", () => ({
    useHistory: () => ({
        push: jest.fn()
    })
}));

export const routerMock = router as jest.Mocked<typeof router>;
