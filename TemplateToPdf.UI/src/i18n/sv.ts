import { TranslationMessages } from 'ra-core';

const swedishMessages: TranslationMessages = {
    ra: {
        action: {
            add: 'Lägg till',
            add_filter: 'Lägg till filter',
            back: 'Tillbaka',
            bulk_actions: '%{smart_count} vald',
            cancel: 'Avbryt',
            clear_array_input: 'Rensa lista',
            clear_input_value: 'Rensa',
            clone: 'Klona',
            confirm: 'Bekräfta',
            create: 'Skapa',
            create_item: 'Skapa %{item}',
            delete: 'Radera',
            edit: 'Redigera',
            export: 'Exportera',
            list: 'Lista',
            refresh: 'Uppdatera',
            remove: 'Ta bort',
            remove_filter: 'Ta bort filter',
            remove_all_filters: 'Ta bort alla filter',
            save: 'Spara',
            search: 'Sök',
            select_all: 'Välj alla',
            select_all_button: 'Välj alla',
            show: 'Visa',
            sort: 'Sortera',
            undo: 'Ångra',
            expand: 'Expandera',
            close: 'Stäng',
            open_menu: 'Öppna meny',
            close_menu: 'Stäng meny',
            update: 'Uppdatera',
            move_up: 'Flytta upp',
            move_down: 'Flytta ner',
            open: 'Öppna',
            toggle_theme: 'Växla tema',
            select_row: 'Välj rad',
            update_application: 'Uppdatera applikation',
            unselect: 'Avmarkera',
            select_columns: 'Välj kolumner'
        },
        boolean: {
            true: 'Ja',
            false: 'Nej',
            null: ' ',
        },
        page: {
            create: 'Skapa %{name}',
            dashboard: 'Dashboard',
            edit: '%{name} #%{id}',
            error: 'Något gick fel',
            list: '%{name}',
            loading: 'Laddar',
            not_found: 'Hittades inte',
            show: '%{name} #%{id}',
            empty: 'Ingen %{name} ännu.',
            invite: 'Vill du lägga till en?',
        },
        navigation: {
            no_results: 'Inga resultat',
            no_more_results: 'Sidan %{page} finns inte. Försök föregående.',
            page_out_of_boundaries: 'Sida %{page} utanför gränserna',
            page_out_from_end: 'Kan inte gå efter sista sidan',
            page_out_from_begin: 'Kan inte gå före sida 1',
            page_range_info: '%{offsetBegin}-%{offsetEnd} av %{total}',
            partial_page_range_info: '%{offsetBegin}-%{offsetEnd} av fler än %{offsetEnd}',
            page_rows_per_page: 'Rader per sida:',
            current_page: 'Sida %{page}',
            page: 'Gå till sida %{page}',
            first: 'Gå till första sidan',
            last: 'Gå till sista sidan',
            next: 'Nästa',
            prev: 'Föregående',
            previous: 'Föregående',
            skip_nav: 'Gå till innehåll'
        },
        auth: {
            auth_check_error: 'Logga in för att fortsätta',
            user_menu: 'Profil',
            username: 'Användarnamn',
            password: 'Lösenord',
            sign_in: 'Logga in',
            sign_in_error: 'Autentisering misslyckades, försök igen',
            logout: 'Logga ut',
        },
        notification: {
            updated: 'Element uppdaterat |||| %{smart_count} element uppdaterade',
            created: 'Element skapat',
            deleted: 'Element raderat |||| %{smart_count} element raderade',
            bad_item: 'Felaktigt element',
            item_doesnt_exist: 'Elementet finns inte',
            http_error: 'Serveranslutningsfel',
            data_provider_error: 'dataProvider fel. Kontrollera konsolen.',
            i18n_error: 'Kunde inte ladda översättningar för angivet språk',
            canceled: 'Åtgärd avbruten',
            logged_out: 'Din session har avslutats, vänligen återanslut.',
            not_authorized: 'Du har inte behörighet till denna resurs.',
            application_update_available: 'En ny version är tillgänglig.'
        },
        input: {
            file: {
                upload_several: 'Släpp filer att ladda upp, eller klicka för att välja.',
                upload_single: 'Släpp en fil att ladda upp, eller klicka för att välja.'
            },
            image: {
                upload_several: 'Släpp bilder att ladda upp, eller klicka för att välja.',
                upload_single: 'Släpp en bild att ladda upp, eller klicka för att välja.'
            },
            references: {
                all_missing: 'Det gick inte att hitta referensdata.',
                many_missing: 'Minst en av de associerade referenserna verkar inte längre vara tillgänglig.',
                single_missing: 'Den associerade referensen verkar inte längre vara tillgänglig.'
            },
            password: {
                toggle_visible: 'Dölj lösenord',
                toggle_hidden: 'Visa lösenord'
            }
        },
        message: {
            yes: 'Ja',
            no: 'Nej',
            are_you_sure: 'Är du säker?',
            about: 'Om',
            not_found: 'Antingen skrev du fel URL eller följde en felaktig länk.',
            auth_error: 'Ett fel uppstod vid validering av åtkomsttoken.',
            bulk_delete_content: 'Är du säker på att du vill ta bort denna %{name}? |||| Är du säker på att du vill ta bort dessa %{smart_count} objekt?',
            bulk_delete_title: 'Ta bort %{name} |||| Ta bort %{smart_count} %{name}',
            bulk_update_content: 'Är du säker på att du vill uppdatera denna %{name}? |||| Är du säker på att du vill uppdatera dessa %{smart_count} objekt?',
            bulk_update_title: 'Uppdatera %{name} |||| Uppdatera %{smart_count} %{name}',
            clear_array_input: 'Rensa listan',
            delete_content: 'Är du säker på att du vill ta bort detta objekt?',
            delete_title: 'Ta bort %{name} #%{id}',
            details: 'Detaljer',
            error: 'Ett klientfel uppstod och din förfrågan kunde inte slutföras.',
            invalid_form: 'Formuläret är inte giltigt. Kontrollera felen.',
            loading: 'Sidan laddas, vänta ett ögonblick',
            no_results: 'Inga resultat hittades.',
            unsaved_changes: 'Vissa av dina ändringar har inte sparats. Är du säker på att du vill ignorera dem?'
        },
        sort: {
            sort_by: 'Sortera efter %{field} %{order}',
            ASC: 'stigande',
            DESC: 'fallande'
        },
        validation: {
            required: 'Obligatorisk',
            minLength: 'Måste vara minst %{min} tecken',
            maxLength: 'Kan vara högst %{max} tecken',
            minValue: 'Måste vara minst %{min}',
            maxValue: 'Kan vara högst %{max}',
            number: 'Måste vara ett nummer',
            email: 'Måste vara en giltig e-postadress',
            oneOf: 'Måste vara en av: %{options}',
            regex: 'Måste matcha ett specifikt format (regexp): %{pattern}'
        },
        saved_queries: {
            label: 'Sparade sökningar',
            query_name: 'Sökningens namn',
            new_label: 'Spara nuvarande sökning...',
            new_dialog_title: 'Spara nuvarande sökning som',
            remove_label: 'Ta bort sparad sökning',
            remove_label_with_name: 'Ta bort sökning "%{name}"',
            remove_dialog_title: 'Ta bort sparad sökning?',
            remove_message: 'Är du säker på att du vill ta bort den här sparade sökningen?',
            help: 'Filtrera listan och spara sökningen för senare'
        }
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
    settings: {
        title: 'Inställningar',
        language: 'Språk',
        theme: 'Tema',
        themes: {
            light: 'Ljust',
            dark: 'Mörkt',
        },
    },
};

export default swedishMessages; 