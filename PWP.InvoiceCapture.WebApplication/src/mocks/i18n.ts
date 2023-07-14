export const reactI18NextMock = jest.setMock("react-i18next", {
    useTranslation: () => ({
        t: jest.fn((text) => text)
    })
});
