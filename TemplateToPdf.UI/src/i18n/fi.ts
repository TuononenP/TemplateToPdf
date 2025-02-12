import { TranslationMessages } from 'ra-core';

const finnishMessages: TranslationMessages = {
    ra: {
        action: {
            add: 'Lisää',
            add_filter: 'Lisää suodatin',
            back: 'Takaisin',
            bulk_actions: '%{smart_count} valittu',
            cancel: 'Peruuta',
            clear_array_input: 'Tyhjennä lista',
            clear_input_value: 'Tyhjennä',
            clone: 'Kopioi',
            confirm: 'Vahvista',
            create: 'Luo',
            create_item: 'Luo %{item}',
            delete: 'Poista',
            edit: 'Muokkaa',
            export: 'Vie',
            list: 'Listaa',
            refresh: 'Päivitä',
            remove: 'Poista',
            remove_filter: 'Poista suodatin',
            remove_all_filters: 'Poista kaikki suodattimet',
            save: 'Tallenna',
            search: 'Hae',
            select_all: 'Valitse kaikki',
            select_all_button: 'Valitse kaikki',
            show: 'Näytä',
            sort: 'Järjestä',
            undo: 'Kumoa',
            unselect: 'Poista valinta',
            expand: 'Laajenna',
            close: 'Sulje',
            open_menu: 'Avaa valikko',
            close_menu: 'Sulje valikko',
            update: 'Päivitä',
            move_up: 'Siirrä ylös',
            move_down: 'Siirrä alas',
            open: 'Avaa',
            toggle_theme: 'Vaihda teema',
            select_row: 'Valitse rivi',
            select_columns: 'Valitse sarakkeet',
            update_application: 'Päivitä sovellus'
        },
        boolean: {
            true: 'Kyllä',
            false: 'Ei',
            null: ' ',
        },
        page: {
            create: 'Luo %{name}',
            dashboard: 'Kojelauta',
            edit: '%{name} #%{id}',
            error: 'Jotain meni pieleen',
            list: '%{name}',
            loading: 'Ladataan',
            not_found: 'Ei löytynyt',
            show: '%{name} #%{id}',
            empty: 'Ei vielä yhtään %{name}.',
            invite: 'Haluatko lisätä yhden?',
        },
        navigation: {
            no_results: 'Ei tuloksia',
            no_more_results: 'Sivu %{page} ei ole käytettävissä. Yritä edellistä.',
            page_out_of_boundaries: 'Sivu %{page} rajojen ulkopuolella',
            page_out_from_end: 'Ei voi mennä viimeisen sivun jälkeen',
            page_out_from_begin: 'Ei voi mennä ennen sivua 1',
            page_range_info: '%{offsetBegin}-%{offsetEnd} / %{total}',
            partial_page_range_info: '%{offsetBegin}-%{offsetEnd} / yli %{offsetEnd}',
            page_rows_per_page: 'Rivejä per sivu:',
            current_page: 'Sivu %{page}',
            page: 'Siirry sivulle %{page}',
            first: 'Ensimmäinen sivu',
            last: 'Viimeinen sivu',
            next: 'Seuraava',
            prev: 'Edellinen',
            previous: 'Edellinen',
            skip_nav: 'Siirry sisältöön'
        },
        auth: {
            auth_check_error: 'Kirjaudu sisään jatkaaksesi',
            user_menu: 'Profiili',
            username: 'Käyttäjätunnus',
            password: 'Salasana',
            sign_in: 'Kirjaudu',
            sign_in_error: 'Todentaminen epäonnistui, yritä uudelleen',
            logout: 'Kirjaudu ulos',
        },
        notification: {
            updated: 'Elementti päivitetty |||| %{smart_count} elementtiä päivitetty',
            created: 'Elementti luotu',
            deleted: 'Elementti poistettu |||| %{smart_count} elementtiä poistettu',
            bad_item: 'Virheellinen elementti',
            item_doesnt_exist: 'Elementtiä ei ole olemassa',
            http_error: 'Palvelinyhteysvirhe',
            data_provider_error: 'dataProvider virhe. Tarkista konsoli.',
            i18n_error: 'Käännöksiä ei voitu ladata määritetylle kielelle',
            canceled: 'Toiminto peruttu',
            logged_out: 'Istuntosi on päättynyt, yhdistä uudelleen.',
            not_authorized: 'Sinulla ei ole oikeuksia tähän resurssiin.',
            application_update_available: 'Uusi versio on saatavilla.'
        },
        input: {
            file: {
                upload_several: 'Pudota tiedostoja ladataksesi, tai klikkaa valitaksesi.',
                upload_single: 'Pudota tiedosto ladataksesi, tai klikkaa valitaksesi.'
            },
            image: {
                upload_several: 'Pudota kuvia ladataksesi, tai klikkaa valitaksesi.',
                upload_single: 'Pudota kuva ladataksesi, tai klikkaa valitaksesi.'
            },
            references: {
                all_missing: 'Viitetietoja ei löytynyt.',
                many_missing: 'Vähintään yksi liittyvä viite ei ole enää saatavilla.',
                single_missing: 'Liittyvä viite ei ole enää saatavilla.'
            },
            password: {
                toggle_visible: 'Piilota salasana',
                toggle_hidden: 'Näytä salasana'
            }
        },
        message: {
            yes: 'Kyllä',
            no: 'Ei',
            are_you_sure: 'Oletko varma?',
            about: 'Tietoja',
            not_found: 'Kirjoitit väärän URL-osoitteen tai seurasit virheellistä linkkiä.',
            auth_error: 'Virhe käyttöoikeustunnuksen vahvistamisessa.',
            bulk_delete_content: 'Haluatko varmasti poistaa tämän %{name}? |||| Haluatko varmasti poistaa nämä %{smart_count} kohdetta?',
            bulk_delete_title: 'Poista %{name} |||| Poista %{smart_count} %{name}',
            bulk_update_content: 'Haluatko varmasti päivittää tämän %{name}? |||| Haluatko varmasti päivittää nämä %{smart_count} kohdetta?',
            bulk_update_title: 'Päivitä %{name} |||| Päivitä %{smart_count} %{name}',
            clear_array_input: 'Tyhjennä lista',
            delete_content: 'Haluatko varmasti poistaa tämän kohteen?',
            delete_title: 'Poista %{name} #%{id}',
            details: 'Yksityiskohdat',
            error: 'Tapahtui virhe eikä pyyntöäsi voitu suorittaa.',
            invalid_form: 'Lomake ei ole kelvollinen. Tarkista virheet.',
            loading: 'Sivu latautuu, odota hetki',
            no_results: 'Tuloksia ei löytynyt.',
            unsaved_changes: 'Joitain muutoksiasi ei ole tallennettu. Oletko varma, että haluat hylätä ne?'
        },
        sort: {
            sort_by: 'Järjestä %{field} %{order}',
            ASC: 'nousevasti',
            DESC: 'laskevasti'
        },
        validation: {
            required: 'Pakollinen',
            minLength: 'Vähintään %{min} merkkiä',
            maxLength: 'Enintään %{max} merkkiä',
            minValue: 'Vähintään %{min}',
            maxValue: 'Korkeintaan %{max}',
            number: 'Täytyy olla numero',
            email: 'Täytyy olla kelvollinen sähköpostiosoite',
            oneOf: 'Täytyy olla yksi seuraavista: %{options}',
            regex: 'Täytyy noudattaa tiettyä muotoa (regexp): %{pattern}'
        },
        saved_queries: {
            label: 'Tallennetut haut',
            query_name: 'Haun nimi',
            new_label: 'Tallenna nykyinen haku...',
            new_dialog_title: 'Tallenna nykyinen haku nimellä',
            remove_label: 'Poista tallennettu haku',
            remove_label_with_name: 'Poista haku "%{name}"',
            remove_dialog_title: 'Poista tallennettu haku?',
            remove_message: 'Haluatko varmasti poistaa tämän tallennetun haun?',
            help: 'Suodata listaa ja tallenna haku myöhempää käyttöä varten'
        }
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
    settings: {
        title: 'Asetukset',
        language: 'Kieli',
        theme: 'Teema',
        themes: {
            light: 'Vaalea',
            dark: 'Tumma',
        },
    },
};

export default finnishMessages; 