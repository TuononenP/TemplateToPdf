import polyglotI18nProvider from 'ra-i18n-polyglot';
import en from 'ra-language-english';
import fi from './fi';
import sv from './sv';
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
            name: 'Template |||| Templates',
            fields: {
                id: 'ID',
                name: 'Name',
                content: 'Content',
                createdAt: 'Created At',
                updatedAt: 'Updated At',
            },
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
    fi: {
        ...fi,
        settings: {
            title: 'Asetukset',
            language: 'Kieli',
            theme: 'Teema',
            themes: {
                light: 'Vaalea',
                dark: 'Tumma',
            },
        },
        templates: {
            name: 'Mallipohja |||| Mallipohjat',
            fields: {
                id: 'ID',
                name: 'Nimi',
                content: 'Sisältö',
                createdAt: 'Luotu',
                updatedAt: 'Päivitetty',
            },
            test: {
                title: 'Testaa mallipohjaa: %{name}',
                action: 'Testaa',
                template: 'Mallipohja',
                model: 'Tietomalli',
                success: 'PDF luotu onnistuneesti',
                error: 'PDF:n luonti epäonnistui',
                invalidJson: 'Virheellinen JSON tietomallissa',
            },
        },
    },
    sv: {
        ...sv,
        settings: {
            title: 'Inställningar',
            language: 'Språk',
            theme: 'Tema',
            themes: {
                light: 'Ljust',
                dark: 'Mörkt',
            },
        },
        templates: {
            name: 'Mall |||| Mallar',
            fields: {
                id: 'ID',
                name: 'Namn',
                content: 'Innehåll',
                createdAt: 'Skapad',
                updatedAt: 'Uppdaterad',
            },
            test: {
                title: 'Testa mall: %{name}',
                action: 'Testa',
                template: 'Mall',
                model: 'Datamodell',
                success: 'PDF skapad',
                error: 'Kunde inte skapa PDF',
                invalidJson: 'Ogiltig JSON i datamodellen',
            },
        },
    },
};

// Initialize i18nProvider with all translations
export const i18nProvider = polyglotI18nProvider(
    locale => messages[locale] as any,
    localStorage.getItem('language') || 'en'
);
