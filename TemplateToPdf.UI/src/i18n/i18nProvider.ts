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
                model: 'Data Model',
                modelForPreview: 'Data Model for Preview',
            },
            notifications: {
                created: 'Template created',
                updated: 'Template updated',
                deleted: 'Template deleted',
                error: {
                    create: 'Error creating template',
                    update: 'Error updating template',
                    delete: 'Error deleting template'
                }
            },
            preview: {
                title: 'Preview',
                refresh: 'Refresh preview',
                error: 'Preview error'
            },
            test: {
                title: 'Test template: %{name}',
                action: 'Test',
                download: 'Download PDF',
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
                model: 'Tietomalli',
                modelForPreview: 'Esikatselun tietomalli',
            },
            notifications: {
                created: 'Mallipohja luotu',
                updated: 'Mallipohja päivitetty',
                deleted: 'Mallipohja poistettu',
                error: {
                    create: 'Virhe mallipohjan luonnissa',
                    update: 'Virhe mallipohjan päivityksessä',
                    delete: 'Virhe mallipohjan poistossa'
                }
            },
            preview: {
                title: 'Esikatselu',
                refresh: 'Päivitä esikatselu',
                error: 'Esikatselu-virhe'
            },
            test: {
                title: 'Testaa mallipohjaa: %{name}',
                action: 'Testaa',
                download: 'Lataa PDF',
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
                model: 'Datamodell',
                modelForPreview: 'Förhandsgranskningens datamodell',
            },
            notifications: {
                created: 'Mall skapad',
                updated: 'Mall uppdaterad',
                deleted: 'Mall raderad',
                error: {
                    create: 'Fel vid skapande av mall',
                    update: 'Fel vid uppdatering av mall',
                    delete: 'Fel vid borttagning av mall'
                }
            },
            preview: {
                title: 'Förhandsgranskning',
                refresh: 'Uppdatera förhandsgranskning',
                error: 'Förhandsgranskning-fel'
            },
            test: {
                title: 'Testa mall: %{name}',
                action: 'Testa',
                download: 'Ladda ner PDF',
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
