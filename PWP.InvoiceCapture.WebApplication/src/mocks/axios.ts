import axios from "axios";

jest.mock("axios");
export const axiosMock = axios as jest.Mocked<typeof axios>;

axiosMock.create.mockReturnValue(axiosMock);
axiosMock.interceptors = {
    request: {
        use: jest.fn(),
        eject: jest.fn()
    },
    response: {
        use: jest.fn(),
        eject: jest.fn()
    }
};
