import polyglotI18nProvider from 'ra-i18n-polyglot';
import en from 'ra-language-english';
import fi from './i18n/fi';
import sv from './i18n/sv';
import { TranslationMessages } from 'ra-core';

const messages: { [key: string]: TranslationMessages } = {
    en: {
        ...en,
        settings: {
            title: 'Settings',
            language: 'Language',
            theme: 'Theme',
            themes: {
                light: 'Light',
                dark: 'Dark',
            },
        },
        templates: {
            test: {
                title: 'Test template: %{name}',
                action: 'Test',
                template: 'Template',
                model: 'Data Model',
                success: 'PDF generated successfully',
                error: 'Failed to generate PDF',
                invalidJson: 'Invalid JSON in data model',
            },
        },
    },
    fi: fi,
    sv: sv,
};

// Initialize i18nProvider with all translations
export const i18nProvider = polyglotI18nProvider(
    locale => messages[locale] as any,
    localStorage.getItem('language') || 'en'
);
