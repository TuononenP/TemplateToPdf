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
        assets: {
            name: 'Asset |||| Assets',
            fields: {
                id: 'ID',
                name: 'Name',
                referenceName: 'Reference Name',
                type: 'Type',
                content: 'Content',
                binaryContent: 'File',
                description: 'Description',
                createdAt: 'Created',
                updatedAt: 'Updated'
            },
            types: {
                image: 'Image',
                css: 'CSS',
                font: 'Font',
                partialTemplate: 'Partial Template'
            },
            preview: {
                action: 'Preview',
                title: 'Preview: %{name}',
                fontPreviewTitle: 'Font Preview',
                fontPreviewText: 'The quick brown fox jumps over the lazy dog. 1234567890',
                unsupportedType: 'Preview not available for this asset type'
            },
            create: {
                title: 'Create Asset',
                fontUploaded: 'Font file uploaded'
            },
            edit: {
                title: 'Edit Asset',
                fontUploaded: 'Font file uploaded'
            },
            notifications: {
                created: 'Asset created',
                updated: 'Asset updated',
                deleted: 'Asset deleted',
                error: {
                    create: 'Error creating asset',
                    update: 'Error updating asset',
                    delete: 'Error deleting asset'
                }
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
        assets: {
            name: 'Resurssi |||| Resurssit',
            fields: {
                name: 'Nimi',
                referenceName: 'Viitenimi',
                type: 'Tyyppi',
                mimeType: 'MIME-tyyppi',
                content: 'Sisältö',
                binaryContent: 'Tiedosto',
                description: 'Kuvaus',
                createdAt: 'Luotu',
                updatedAt: 'Päivitetty'
            },
            preview: {
                title: 'Esikatselu: %{name}',
                action: 'Esikatsele',
                fontPreviewText: 'Nopea ruskea kettu hyppää laiskan koiran yli. 1234567890',
                unsupportedType: 'Esikatselu ei ole saatavilla tälle resurssityypille'
            },
            create: {
                title: 'Luo resurssi',
                fontUploaded: 'Fonttitiedosto ladattu onnistuneesti'
            },
            edit: {
                title: 'Muokkaa resurssia',
                fontUploaded: 'Fonttitiedosto ladattu onnistuneesti'
            },
            notifications: {
                created: 'Resurssi luotu',
                updated: 'Resurssi päivitetty',
                deleted: 'Resurssi poistettu',
                error: {
                    create: 'Virhe resurssin luomisessa',
                    update: 'Virhe resurssin päivityksessä',
                    delete: 'Virhe resurssin poistamisessa'
                }
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
        assets: {
            name: 'Resurs |||| Resurser',
            fields: {
                name: 'Namn',
                referenceName: 'Referensnamn',
                type: 'Typ',
                mimeType: 'MIME-typ',
                content: 'Innehåll',
                binaryContent: 'Fil',
                description: 'Beskrivning',
                createdAt: 'Skapad',
                updatedAt: 'Uppdaterad'
            },
            preview: {
                title: 'Förhandsgranska: %{name}',
                action: 'Förhandsgranska',
                fontPreviewText: 'Den snabba bruna räven hoppar över den lata hunden. 1234567890',
                unsupportedType: 'Förhandsgranskning är inte tillgänglig för denna resurstyp'
            },
            create: {
                title: 'Skapa resurs',
                fontUploaded: 'Typsnittsfil uppladdad'
            },
            edit: {
                title: 'Redigera resurs',
                fontUploaded: 'Typsnittsfil uppladdad'
            },
            notifications: {
                created: 'Resurs skapad',
                updated: 'Resurs uppdaterad',
                deleted: 'Resurs borttagen',
                error: {
                    create: 'Fel vid skapande av resurss',
                    update: 'Fel vid uppdatering av resurss',
                    delete: 'Fel vid borttagning av resurss'
                }
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
