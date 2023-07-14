import i18n from "i18next";
import LanguageDetector from "i18next-browser-languagedetector";
import Backend from "i18next-xhr-backend";
import { initReactI18next } from "react-i18next";
import { settings } from "../settings/SettingsProvider";

export const i18next = i18n
    .use(Backend)
    .use(LanguageDetector)
    .use(initReactI18next)
    .init({
        fallbackLng: "en",
        debug: settings.isDevelopment,
        lng: settings.language,
        react: {
            useSuspense: true
        },
        interpolation: {
            escapeValue: false // not needed for react as it escapes by default
        },
        keySeparator: "->",
        nsSeparator: "::"
    });
