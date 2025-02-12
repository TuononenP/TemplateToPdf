import { Admin, Resource, CustomRoutes, defaultTheme } from 'react-admin';
import polyglotI18nProvider from 'ra-i18n-polyglot';
import englishMessages from 'ra-language-english';
import finnishMessages from './i18n/fi';
import swedishMessages from './i18n/sv';
import { TemplateList, TemplateEdit, TemplateCreate } from './components/templates';
import { SettingsPage } from './components/settings';
import { dataProvider } from './dataProvider';
import { Layout } from './layout';
import Dashboard from './components/Dashboard';
import DescriptionIcon from '@mui/icons-material/Description';
import { Route } from 'react-router-dom';

// Extend English messages with settings translations
const extendedEnglishMessages = {
    ...englishMessages,
    settings: {
        title: 'Settings',
        language: 'Language',
        theme: 'Theme',
        themes: {
            light: 'Light',
            dark: 'Dark',
        },
    },
};

// Initialize i18nProvider with all translations
const i18nProvider = polyglotI18nProvider(
    () => extendedEnglishMessages,
    localStorage.getItem('language') || 'en',
    {
        en: extendedEnglishMessages,
        fi: finnishMessages,
        sv: swedishMessages,
    }
);

const darkTheme = {
    ...defaultTheme,
    palette: {
        mode: 'dark',
        primary: {
            main: '#90caf9',
        },
        background: {
            default: '#121212',
            paper: '#1e1e1e',
        },
    },
    components: {
        ...defaultTheme.components,
        MuiAppBar: {
            styleOverrides: {
                colorSecondary: {
                    backgroundColor: '#1e1e1e',
                    color: '#ffffff',
                },
            },
        },
        MuiDrawer: {
            styleOverrides: {
                paper: {
                    backgroundColor: '#121212',
                    color: '#ffffff',
                },
            },
        },
    },
};

const lightTheme = {
    ...defaultTheme,
    palette: {
        mode: 'light',
        primary: {
            main: '#1976d2',
        },
        background: {
            default: '#fafafb',
        },
    },
    components: {
        ...defaultTheme.components,
        MuiAppBar: {
            styleOverrides: {
                colorSecondary: {
                    backgroundColor: '#0380fc',
                    color: '#ffffff',
                },
            },
        },
    },
};

const App = () => {
    const savedTheme = localStorage.getItem('theme') || 'light';
    
    return (
        <Admin 
            dataProvider={dataProvider}
            i18nProvider={i18nProvider}
            layout={Layout}
            dashboard={Dashboard}
            title="Dashboard"
            theme={savedTheme === 'dark' ? darkTheme : lightTheme}
            requireAuth={false}
        >
            <CustomRoutes>
                <Route path="/settings" element={<SettingsPage />} />
            </CustomRoutes>
            <Resource
                name="templates"
                options={{ label: 'Templates' }}
                list={TemplateList}
                edit={TemplateEdit}
                create={TemplateCreate}
                icon={DescriptionIcon}
            />
        </Admin>
    );
};

export default App; 